using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Errors;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    public class UsuarioController : BaseApiController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ITokenService _TokenService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IGenericSeguridadRepository<Usuario> _seguridadRepository;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ITokenService TokenService, IMapper mapper, IPasswordHasher<Usuario> passwordHasher, IGenericSeguridadRepository<Usuario> seguridadRepository, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _TokenService = TokenService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _seguridadRepository = seguridadRepository;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Login(LoginDTO LoginDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(LoginDTO.Email);

            if (usuario == null)
            {
                return Unauthorized(new CodeErrorResponse(401));
            }

            var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, LoginDTO.Password, false);

            if (!resultado.Succeeded)
            {
                return Unauthorized(new CodeErrorResponse(401));
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Username = usuario.UserName,
                Token = _TokenService.CreateToken(usuario, roles),
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false
            };
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioDTO>> Registrar(RegistrarDTO registrarDTO)
        {
            var usuario = new Usuario
            {
                Email = registrarDTO.Email,
                UserName = registrarDTO.Username,
                Nombre = registrarDTO.Nombre,
                Apellido = registrarDTO.Apellido
            };

            var resultado = await _userManager.CreateAsync(usuario, registrarDTO.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(new CodeErrorResponse(400, resultado.Errors.FirstOrDefault().Description));
            }

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Token = _TokenService.CreateToken(usuario, null),
                Email = usuario.Email,
                Username = usuario.UserName,
                Admin = false
            };
        }

        [Authorize]
        [HttpPut("actualizar/{id}")]
        public async Task<ActionResult<UsuarioDTO>> Actualizar(string id, RegistrarDTO registrarDTO)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new CodeErrorResponse(404, "El usuario no existe"));
            }

            usuario.Nombre = registrarDTO.Nombre;
            usuario.Apellido = registrarDTO.Apellido;
            usuario.Imagen = registrarDTO.Imagen;

            if (!string.IsNullOrEmpty(registrarDTO.Password))
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, registrarDTO.Password);
            }

            var resultado = await _userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                return BadRequest(new CodeErrorResponse(400, "No se pudo actualizar el usuario. " + resultado.Errors.FirstOrDefault().Description));
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.UserName,
                Token = _TokenService.CreateToken(usuario, roles),
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false
            };            
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("pagination")]
        public async Task<ActionResult<Pagination<UsuarioDTO>>> GetUsuarios([FromQuery] UsuarioSpecificationParams usuarioParams)
        {
            var spec = new UsuarioSpecification(usuarioParams);
            var usuarios = await _seguridadRepository.GetAllWithSpec(spec);

            var specCount = new UsuarioForCountingSpecification(usuarioParams);
            var totalUsuarios = await _seguridadRepository.CountAsync(specCount);
            
            var rounded = Math.Ceiling(Convert.ToDecimal(totalUsuarios) / Convert.ToDecimal(usuarioParams.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            var data = _mapper.Map<IReadOnlyList<Usuario>, IReadOnlyList<UsuarioDTO>>(usuarios);

            return Ok(
                new Pagination<UsuarioDTO>
                {
                    Count = totalUsuarios,
                    Data = data,
                    PageCount = totalPages,
                    PageIndex = usuarioParams.PageIndex,
                    PageSize = usuarioParams.PageSize
                }
            );
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("role/{id}")]
        public async Task<ActionResult<UsuarioDTO>> UpdateRole(string id, RoleDTO roleParam)
        {
            // validar que el rol exista
            var role = await _roleManager.FindByNameAsync(roleParam.Nombre);
            if (role == null)
            {
                return NotFound(new CodeErrorResponse(404, "El role no existe"));
            }

            // valdiar que el usuario exista
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new CodeErrorResponse(404, "El usuario no existe"));
            }

            var usuarioDTO = _mapper.Map<Usuario, UsuarioDTO>(usuario);

            // si se debe agregar el role
            if (roleParam.Status)
            {
                var resultado = await _userManager.AddToRoleAsync(usuario, roleParam.Nombre);
                if (resultado.Succeeded)
                {
                    usuarioDTO.Admin = true;
                }

                if (resultado.Errors.Any())
                {
                    // si el usuario ya es administrador
                    if (resultado.Errors.Where(x => x.Code == "UserAlreadyInRole").Any())
                    {
                        usuarioDTO.Admin = true;
                    }
                }
            }
            else // si se debe remover el role
            {               
                var resultado = await _userManager.RemoveFromRoleAsync(usuario, roleParam.Nombre);
                if (resultado.Succeeded)
                {
                    usuarioDTO.Admin = false;
                }
            }
            
            // agregar roles a token
            if (usuarioDTO.Admin)
            {
                var roles = new List<string>();
                roles.Add("ADMIN");
                usuarioDTO.Token = _TokenService.CreateToken(usuario, roles);
            }
            else
            {
                usuarioDTO.Token = _TokenService.CreateToken(usuario, null);
            }

            return usuarioDTO;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("account/{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuarioBy(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new CodeErrorResponse(404, "El usuario no existe"));
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.UserName,
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false
            };
        }

        /// <summary>
        /// Obtiene la sesión del usuario actual
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario()
        {
            // buscar usuario que se ingresó por el header
            var usuario = await _userManager.BuscarUsuarioAsync(HttpContext.User);

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.UserName,
                Imagen = usuario.Imagen,
                Token = _TokenService.CreateToken(usuario, roles),
                Admin = roles.Contains("ADMIN") ? true : false
            };
        }

        [HttpGet("emailvalido")]
        public async Task<ActionResult<bool>> ValidarEmail([FromQuery]string email)
        {
            var usuario = await _userManager.FindByEmailAsync(email);

            if (usuario == null) return false;

            return true;
        }

        [Authorize]
        [HttpGet("direccion")]
        public async Task<ActionResult<DireccionDTO>> GetDireccion()
        {
            var usuario = await _userManager.BuscarUsuarioPorDireccionAsync(HttpContext.User);

            return _mapper.Map<Direccion, DireccionDTO>(usuario.Direccion);
        }

        [Authorize]
        [HttpPut("direccion")]
        public async Task<ActionResult<DireccionDTO>> UpdateDireccion(DireccionDTO direccion)
        {
            var usuario = await _userManager.BuscarUsuarioPorDireccionAsync(HttpContext.User);

            usuario.Direccion = _mapper.Map<DireccionDTO, Direccion>(direccion);

            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded) return Ok(_mapper.Map<Direccion, DireccionDTO>(usuario.Direccion));

            return BadRequest("No se pudo actualizar la dirección del usuario. " + resultado.Errors.FirstOrDefault().Description);
        }
    }
}
