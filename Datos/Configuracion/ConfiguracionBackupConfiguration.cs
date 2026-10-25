using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class ConfiguracionBackupConfiguration : IEntityTypeConfiguration<ConfiguracionBackup>
    {
        public void Configure(EntityTypeBuilder<ConfiguracionBackup> builder)
        {
            builder.ToTable("ConfiguracionBackup");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.NombreBaseDatos).IsRequired().HasMaxLength(128);
            builder.Property(c => c.RutaBackupServer).IsRequired().HasMaxLength(260);
            builder.Property(c => c.RutaBackupUNC).IsRequired().HasMaxLength(260);
            builder.Property(c => c.DriveFolderId).HasMaxLength(260);
            builder.Property(c => c.UltimaSubidaArchivo).HasMaxLength(260);
            builder.Property(c => c.FechaUltimaSubida);
            builder.Property(c => c.FechaActualizacion).IsRequired();
        }
    }
}
