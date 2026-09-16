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
            builder.HasKey(s => s.IdServicio);
            builder.Property(s => s.IdServicio).HasColumnName("id_servicio");
            builder.Property(s => s.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(100);
            builder.Property(s => s.Precio).HasColumnName("precio").HasPrecision(18, 2).IsRequired();
            builder.Property(s => s.Activo).HasColumnName("activo").IsRequired();
        }
    }
}
