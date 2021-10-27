using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.DTOs
{
    public class OrdenCompraDTO
    {
        public string CarritoCompraId { get; set; }
        public int TipoEnvio { get; set; }
        public DireccionDTO DireccionEnvio { get; set; }
    }
}
