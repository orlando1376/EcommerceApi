using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    public class SeguridadDbContextData
    {
        public static async Task SeedUserAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            // si la tabla de usuarios está vacía
            if (!userManager.Users.Any())
            {
                var usuario = new Usuario
                {
                    Nombre = "Orlando",
                    Apellido = "Franco Cespedes",
                    UserName = "orlo",
                    Email = "orlo@gmail.com",
                    Direccion = new Direccion
                    {
                        Calle = "Calle 80 nro 100-20",
                        Ciudad = "Bogota",
                        CodigoPostal = "1100011",
                        Departamento = "Cundinamarca"
                    }
                };
                await userManager.CreateAsync(usuario, "Orlo2021.");
            }

            // si la tabla de roles está vacía
            if (!roleManager.Roles.Any())
            {
                var role = new IdentityRole
                {
                    Name = "ADMIN"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
