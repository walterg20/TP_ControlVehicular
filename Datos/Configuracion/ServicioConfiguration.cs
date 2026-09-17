using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
    {
        public void Configure(EntityTypeBuilder<Servicio> builder)
        {
            builder.ToTable("Servicio");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Nombre)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(s => s.Precio)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
