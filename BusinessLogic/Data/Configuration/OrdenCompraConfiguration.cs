using Core.Entities.OrdenCompra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data.Configuration
{
    public class OrdenCompraConfiguration : IEntityTypeConfiguration<OrdenCompras>
    {
        public void Configure(EntityTypeBuilder<OrdenCompras> builder)
        {
            // relación entre la Orden de compra y la Dirección
            builder.OwnsOne(o => o.DireccionEnvio, x =>
            {
                x.WithOwner();
            });

            // valores por defecto del status de la orden de compra
            builder.Property(s => s.Status)
                .HasConversion(
                    o => o.ToString(),
                    o => (OrdenStatus)Enum.Parse(typeof(OrdenStatus), o)
                );

            // si se borra la orden de compra también se borran todos los items asociados a esta
            builder.HasMany(o => o.OrdenItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)");
        }
    }
}
