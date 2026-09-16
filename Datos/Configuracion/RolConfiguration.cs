using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;
namespace TP_ControlVehicular.Datos.Configuracion;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Rol");
        builder.HasKey(x => x.IdRol);
        builder.Property(x => x.IdRol).HasColumnName("id_rol");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(250).IsRequired();
        builder.Property(x => x.Estado).HasColumnName("estado").IsRequired();
    }
}
