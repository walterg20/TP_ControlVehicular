// walterg20
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Apellido).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Dni).IsRequired().HasMaxLength(15);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Telefono).HasMaxLength(20);
            builder.Property(u => u.Domicilio).HasMaxLength(200);

            // Restricciones únicas
            builder.HasIndex(u => u.Dni).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasOne(u => u.Rol)
                   .WithMany(r => r.Usuarios)
                   .HasForeignKey(u => u.RolId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
