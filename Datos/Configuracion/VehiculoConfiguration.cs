
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;


namespace TP_ControlVehicular.Datos.Configuracion
{
    public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
    {
        public void Configure(EntityTypeBuilder<Vehiculo> builder)
        {
            builder.ToTable("Vehiculo");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Patente)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(v => v.Anio)
                   .IsRequired();

            builder.Property(v => v.KmActual)
                   .IsRequired();

            builder.HasOne(v => v.Cliente)
                   .WithMany(c => c.Vehiculos)
                   .HasForeignKey(v => v.ClienteId);

            builder.HasOne(v => v.Modelo)
                   .WithMany(m => m.Vehiculos)
                   .HasForeignKey(v => v.ModeloId);
        }
    }
}
