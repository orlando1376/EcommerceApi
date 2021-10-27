using Core.Entities.OrdenCompra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class OrdenCompraWithItemsSpecification : BaseSpecification<OrdenCompras>
    {
        public OrdenCompraWithItemsSpecification(string email) : base(o => o.CompradorEmail == email)
        {
            // incluir información de las entidades OrdenItems y TipoEnvio
            AddInclude(o => o.OrdenItems);
            AddInclude(o => o.TipoEnvio);

            AddOrderByDesc(o => o.OrdenCompraFecha);
        }

        public OrdenCompraWithItemsSpecification(int id, string email) : base(o => o.CompradorEmail == email && o.Id == id)
        {
            // incluir información de las entidades OrdenItems y TipoEnvio
            AddInclude(o => o.OrdenItems);
            AddInclude(o => o.TipoEnvio);

            AddOrderByDesc(o => o.OrdenCompraFecha);
        }
    }
}
