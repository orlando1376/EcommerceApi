using Core.Entities.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.DTOs
{
    public class OrdenCompraResponseDTO
    {
        public int Id { get; set; }
        public string CompradorEmail { get; set; }
        public DateTimeOffset OrdenCompraFecha { get; set; }
        public Direccion DireccionEnvio { get; set; }
        public string TipoEnvio { get; set; }
        public decimal TipoEnvioPrecio { get; set; }
        public IReadOnlyList<OrdenItemResponseDTO> OrdenItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}
