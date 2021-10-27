using Core.Entities;
using Core.Entities.OrdenCompra;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly ICarritoCompraRepository _carritoCompraRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrdenCompraService(ICarritoCompraRepository carritoCompraRepository, IUnitOfWork unitOfWork)
        {
            _carritoCompraRepository = carritoCompraRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Crear una orden de compra
        /// </summary>
        /// <param name="compradorEmail"></param>
        /// <param name="tipoEnvio"></param>
        /// <param name="carritoId"></param>
        /// <param name="direccion"></param>
        /// <returns></returns>
        public async Task<OrdenCompras> AddOrdenCompraAsync(string compradorEmail, int tipoEnvio, string carritoId, Core.Entities.OrdenCompra.Direccion direccion)
        {
            var carritoCompra = await _carritoCompraRepository.GetCarritoCompraAsync(carritoId);

            // lista de productos del carrito de compras
            var items = new List<OrdenItem>();
            foreach (var item in carritoCompra.Items)
            {
                // buscar producto
                var productoItem = await _unitOfWork.Repository<Producto>().GetByIdAsync(item.Id);

                // adicionar producto
                var itemOrdenado = new ProductoItemOrdenado(productoItem.Id, productoItem.Nombre, productoItem.Imagen);
                var ordenItem = new OrdenItem(itemOrdenado, productoItem.Precio, item.Cantidad);
                items.Add(ordenItem);
            }

            // tipo de envío
            var tipoEnvioEntity = await _unitOfWork.Repository<TipoEnvio>().GetByIdAsync(tipoEnvio);

            // subtotal
            var subtotal = items.Sum(item => item.Precio * item.Cantidad);

            // crear orden de compra
            var ordenCompra = new OrdenCompras(compradorEmail, direccion, tipoEnvioEntity, items, subtotal);

            //guardar la orden de compra en la base de datos
            _unitOfWork.Repository<OrdenCompras>().AddEntity(ordenCompra);
            var resultado = await _unitOfWork.Complete();
            if (resultado <= 0)
            {
                return null;
            }

            // borrar el carrito de compra
            await _carritoCompraRepository.DeleteCarritoCompraSync(carritoId);

            return ordenCompra;
        }

        /// <summary>
        /// Obtener una orden de compra por Id
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<OrdenCompras>> GetOrdenComprasByUserEmailAsync(string email)
        {
            var spec = new OrdenCompraWithItemsSpecification(email);

            return await _unitOfWork.Repository<OrdenCompras>().GetAllWithSpec(spec);
        }

        /// <summary>
        /// Obtener una orden de compra por el email del usuario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<OrdenCompras> GetOrdenComprasByIdAsync(int id, string email)
        {
            var spec = new OrdenCompraWithItemsSpecification(id, email);

            return await _unitOfWork.Repository<OrdenCompras>().GetByIdWithSpec(spec);
        }

        /// <summary>
        /// Obtener la lista de tipos de envío
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<TipoEnvio>> GetTipoEnvios()
        {
            return await _unitOfWork.Repository<TipoEnvio>().GetAllAsync();
        }
    }
}
