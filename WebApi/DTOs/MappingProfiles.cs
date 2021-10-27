using AutoMapper;
using Core.Entities;
using Core.Entities.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.DTOs
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Alternativa 1: Relación automática
            // CreateMap<Producto, ProductoDTO>();

            // Alternativa 2: Especificando la relación entre los campos
            CreateMap<Producto, ProductoDTO>()
                // destino, origen
                .ForMember(p => p.CategoriaNombre, x => x.MapFrom(a => a.Categoria.Nombre))
                .ForMember(p => p.MarcaNombre, x => x.MapFrom(a => a.Marca.Nombre));

            CreateMap<Core.Entities.Direccion, DireccionDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();

            CreateMap<DireccionDTO, Core.Entities.OrdenCompra.Direccion>();

            CreateMap<OrdenCompras, OrdenCompraResponseDTO>()
                // destino, origen
                .ForMember(o => o.TipoEnvio, x => x.MapFrom(y => y.TipoEnvio.Nombre))
                .ForMember(o => o.TipoEnvioPrecio, x => x.MapFrom(y => y.TipoEnvio.Precio));

            CreateMap<OrdenItem, OrdenItemResponseDTO>()
                // destino, origen
                .ForMember(o => o.ProductoId, x => x.MapFrom(y => y.ItemOrdenado.ProductoItemId))
                .ForMember(o => o.ProductoNombre, x => x.MapFrom(y => y.ItemOrdenado.ProductoNombre))
                .ForMember(o => o.ProductoImagen, x => x.MapFrom(y => y.ItemOrdenado.ImagenUrl));

        }
    }
}
