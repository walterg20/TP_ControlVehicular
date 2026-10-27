// walterg20
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class ConfiguracionEmpresaConfiguration : IEntityTypeConfiguration<ConfiguracionEmpresa>
    {
        public void Configure(EntityTypeBuilder<ConfiguracionEmpresa> builder)
        {
            builder.ToTable("ConfiguracionEmpresa");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Direccion).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Telefono).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Email).HasMaxLength(150);
            builder.Property(c => c.Logo).HasColumnType("varbinary(max)");
            builder.Property(c => c.FechaActualizacion).IsRequired();
        }
    }
}