using Core.Entities.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrdenCompraService
    {
        /// <summary>
        /// Crea una orden de compra
        /// </summary>
        /// <param name="compradorEmail"></param>
        /// <param name="tipoEnvio"></param>
        /// <param name="carritoId"></param>
        /// <param name="direccion"></param>
        /// <returns></returns>
        Task<OrdenCompras> AddOrdenCompraAsync(string compradorEmail, int tipoEnvio, string carritoId, Direccion direccion);

        /// <summary>
        /// Obtiene la lista de ordenes de compra por el email de usuario
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<IReadOnlyList<OrdenCompras>> GetOrdenComprasByUserEmailAsync(string email);

        /// <summary>
        /// Obtiene una orden de compra por el id y el email de usuario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<OrdenCompras> GetOrdenComprasByIdAsync(int id, string email);

        /// <summary>
        /// Obtiene la lista de tipos de envío
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<TipoEnvio>> GetTipoEnvios();
    }
}
