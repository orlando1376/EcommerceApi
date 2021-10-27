using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Errors;

namespace WebApi.Controllers
{
    public class ProductoController : BaseApiController
    {
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IMapper _mapper;

        public ProductoController(IGenericRepository<Producto> productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductoDTO>>> GetProductos([FromQuery]ProductoSpecificationParams productoParams)
        {
            var spec = new ProductoWithCategoriaAndMarcaSpecification(productoParams);
            var productos = await _productoRepository.GetAllWithSpec(spec);

            // cantidad de productos
            var specCount = new ProductoForCountingSpecification(productoParams);
            var totalProductos = await _productoRepository.CountAsync(specCount);

            // cantidad de páginas
            var rounded = Math.Ceiling(Convert.ToDecimal(totalProductos / productoParams.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            var data = _mapper.Map<IReadOnlyList<Producto>, IReadOnlyList<ProductoDTO>>(productos);

            return Ok(
                new Pagination<ProductoDTO>
                {
                    Count = totalProductos,
                    Data = data,
                    PageCount = totalPages,
                    PageIndex = productoParams.PageIndex,
                    PageSize = productoParams.PageSize
                }
            );

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            // spec = debe incluir la logica de la condición  de la consulta y las realciones entre las entidades, la relación entre Producto, Marca y Categoría
            var spec = new ProductoWithCategoriaAndMarcaSpecification(id);
            var producto = await _productoRepository.GetByIdWithSpec(spec);

            if (producto == null)
            {
                return NotFound(new CodeErrorResponse(404, "El Producto no existe."));
            }
            return _mapper.Map<Producto, ProductoDTO>(producto);
        }

        /// <summary>
        /// Crear un producto
        /// </summary>
        /// <param name="producto"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult<Producto>> Post(Producto producto)
        {
            var resultado = await _productoRepository.Add(producto);

            if (resultado == 0)
            {
                throw new Exception("No se pudo insertar el producto.");
            }

            return Ok(producto);
        }

        /// <summary>
        /// Actualizar un producto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="producto"></param>
        /// <returns></returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Producto>> Put(int id, Producto producto)
        {
            producto.Id = id;
            var resultado = await _productoRepository.Update(producto);

            if (resultado == 0)
            {
                throw new Exception("No se pudo actualizar el producto.");
            }

            return Ok(producto);
        }

    }
}
