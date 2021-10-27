using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<Usuario> BuscarUsuarioPorDireccionAsync(this UserManager<Usuario> input, ClaimsPrincipal usr)
        {
            var email = usr?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            // relacionar la tabla Users con la tabla Direccion
            var usuario = await input.Users.Include(x => x.Direccion).SingleOrDefaultAsync(x => x.Email == email);

            return usuario;
        }

        public static async Task<Usuario> BuscarUsuarioAsync(this UserManager<Usuario> input, ClaimsPrincipal usr)
        {
            var email = usr?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            // relacionar la tabla Users con la tabla Direccion
            var usuario = await input.Users.SingleOrDefaultAsync(x => x.Email == email);

            return usuario;
        }
    }
}
