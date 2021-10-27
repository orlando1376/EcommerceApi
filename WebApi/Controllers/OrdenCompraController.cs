using AutoMapper;
using Core.Entities.OrdenCompra;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Errors;

namespace WebApi.Controllers
{
    [Authorize]
    public class OrdenCompraController : BaseApiController
    {
        private readonly IOrdenCompraService _ordenCompraService;
        private readonly IMapper _mapper;

        public OrdenCompraController(IOrdenCompraService ordenCompra, IMapper mapper)
        {
            _ordenCompraService = ordenCompra;
            _mapper = mapper;
        }
        
        /// <summary>
        /// Crea una orden de compra
        /// </summary>
        /// <param name="ordenCompraDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<OrdenCompraResponseDTO>> AddOrdenCompra(OrdenCompraDTO ordenCompraDTO)
        {
            // buscar email del usuario desde toquen
            var email = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            // mapper.Map<origen, destino>(data)
            var direccion = _mapper.Map<DireccionDTO, Direccion>(ordenCompraDTO.DireccionEnvio);

            var ordenCompra = await _ordenCompraService.AddOrdenCompraAsync(email, ordenCompraDTO.TipoEnvio, ordenCompraDTO.CarritoCompraId, direccion);

            if (ordenCompra == null) BadRequest(new CodeErrorResponse(400, "Error creando orden de compra."));

            // mapper.Map<origen, destino>(data)
            return Ok(_mapper.Map<OrdenCompras, OrdenCompraResponseDTO>(ordenCompra));
        }

        /// <summary>
        /// Obtiene la lista de ordenes de compra por el email que está en el token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrdenCompraResponseDTO>>> GetOrdenCompras()
        {
            // buscar email del usuario desde toquen
            var email = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var ordenCompras = await _ordenCompraService.GetOrdenComprasByUserEmailAsync(email);

            // mapper.Map<origen, destino>(data)
            return Ok(_mapper.Map<IReadOnlyList<OrdenCompras>, IReadOnlyList<OrdenCompraResponseDTO>>(ordenCompras));
        }

        /// <summary>
        /// Obtiene una orden de compra por el id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraResponseDTO>> GetOrdenComprasById(int id)
        {
            // buscar email del usuario desde toquen
            var email = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var ordenCompra = await _ordenCompraService.GetOrdenComprasByIdAsync(id, email);

            if (ordenCompra == null)
            {
                return NotFound(new CodeErrorResponse(404, "No se encontró la Orden de compra."));
            }

            // mapper.Map<origen, destino>(data)
            return _mapper.Map<OrdenCompras, OrdenCompraResponseDTO>(ordenCompra);
        }

        /// <summary>
        /// Obtiene la lista de Tipos de envío
        /// </summary>
        /// <returns></returns>
        [HttpGet("tipoEnvio")]
        public async Task<ActionResult<IReadOnlyList<TipoEnvio>>> GetTipoEnvios()
        {
            return Ok(await _ordenCompraService.GetTipoEnvios());
        }

    }
}
