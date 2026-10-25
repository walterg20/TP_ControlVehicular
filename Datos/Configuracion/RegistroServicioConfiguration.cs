using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class RegistroServicioConfiguration : IEntityTypeConfiguration<RegistroServicio>
    {
        public void Configure(EntityTypeBuilder<RegistroServicio> builder)
        {
            builder.ToTable("RegistroServicio", tb => tb.HasTrigger("TR_RegistroServicio_SincronizarKm"));
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Estado)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(r => r.Vehiculo)
                   .WithMany()
                   .HasForeignKey(r => r.VehiculoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Taller)
                   .WithMany()
                   .HasForeignKey(r => r.TallerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Usuario)
                   .WithMany()
                   .HasForeignKey(r => r.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
