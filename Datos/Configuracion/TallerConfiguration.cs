
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class TallerConfiguration : IEntityTypeConfiguration<Taller>

    {
        public void Configure(EntityTypeBuilder<Taller> builder)
        {
            builder.ToTable("Taller");
            builder.HasKey(t=> t.Id);
            builder.Property(t => t.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Direccion).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Telefono).IsRequired().HasMaxLength(20);
            builder.Property(t => t.Activo).IsRequired();   
        }
    }
}
