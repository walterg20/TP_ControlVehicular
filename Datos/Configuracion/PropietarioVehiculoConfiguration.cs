using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class PropietarioVehiculoConfiguration : IEntityTypeConfiguration<PropietarioVehiculo>
    {
        public void Configure(EntityTypeBuilder<PropietarioVehiculo> builder)
        {
            builder.ToTable("PropietarioVehiculo");
            builder.HasKey(pv => pv.Id);

            builder.Property(pv => pv.FechaAdquisicion).IsRequired();
            builder.Property(pv => pv.FechaVenta).IsRequired(false);
            builder.Property(pv => pv.EsActual).IsRequired();

            builder.HasOne(pv => pv.Cliente)
                   .WithMany(c => c.Propietarios)
                   .HasForeignKey(pv => pv.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pv => pv.Vehiculo)
                   .WithMany(v => v.Propietarios)
                   .HasForeignKey(pv => pv.VehiculoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
