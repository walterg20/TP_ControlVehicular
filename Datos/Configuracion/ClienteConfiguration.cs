using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;


namespace TP_ControlVehicular.Datos.Configuracion
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Cliente");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Apellido).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Dni).IsRequired().HasMaxLength(8);
            builder.Property(c => c.Direccion).IsRequired().HasMaxLength(200);
            builder.Property(c => c.FechaNacimiento).IsRequired();
            builder.Property(c => c.Activo).IsRequired();
            builder.HasMany(c => c.Vehiculos)
                  .WithOne(v => v.Cliente)
                  .HasForeignKey(v => v.Id);
        }
    }
}
