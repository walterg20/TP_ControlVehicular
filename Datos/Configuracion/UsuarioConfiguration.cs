using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;
namespace TP_ControlVehicular.Datos.Configuracion;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");
        builder.HasKey(x => x.IdUsuario);
        builder.Property(x => x.IdUsuario).HasColumnName("id_usuario");
        builder.Property(x => x.IdRol).HasColumnName("id_rol");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Contrasena).HasColumnName("contrasena").HasMaxLength(250).IsRequired();
        builder.Property(x => x.Estado).HasColumnName("estado").IsRequired();
        builder.HasOne(x => x.Rol).WithMany(x => x.Usuarios).HasForeignKey(x => x.IdRol).OnDelete(DeleteBehavior.Restrict);
    }
}
