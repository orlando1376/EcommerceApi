using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    public class CarritoCompraController : BaseApiController
    {
        private readonly ICarritoCompraRepository _carritoCompra;

        public CarritoCompraController(ICarritoCompraRepository carritoCompra)
        {
            _carritoCompra = carritoCompra;
        }

        [HttpGet]
        public async Task<ActionResult<CarritoCompra>> GetCarritoById(string id)
        {
            var carrito = await _carritoCompra.GetCarritoCompraAsync(id);

            return Ok(carrito ?? new CarritoCompra(id));
        }

        [HttpPost]
        public async Task<ActionResult<CarritoCompra>> UpdateCarritoCompra(CarritoCompra carritoParametro)
        {
            var carritoActualizado = await _carritoCompra.UpdateCarritoCompraAsynx(carritoParametro);

            return Ok(carritoActualizado);
        }

        [HttpDelete]
        public async Task DeleteCarritoCompra(string id)
        {
            await _carritoCompra.DeleteCarritoCompraSync(id);
        }
    }
}
