using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class DetalleServicioConfiguration : IEntityTypeConfiguration<DetalleServicio>
    {
        public void Configure(EntityTypeBuilder<DetalleServicio> builder)
        {
            builder.ToTable("DetalleServicio");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Precio)
                   .HasColumnType("decimal(18,2)");

            builder.Property(d => d.Origen)
                   .HasMaxLength(100);

            builder.Property(d => d.Estado)
                   .HasMaxLength(50);

            builder.HasOne(d => d.RegistroServicio)
                   .WithMany(r => r.Detalles)
                   .HasForeignKey(d => d.RegistroServicioId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Usuario)
                   .WithMany()
                   .HasForeignKey(d => d.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Servicio)
                   .WithMany(s => s.Detalles)
                   .HasForeignKey(d => d.ServicioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
