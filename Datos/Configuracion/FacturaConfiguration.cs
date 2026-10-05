using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Datos.Configuracion
{
    public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
    {
        public void Configure(EntityTypeBuilder<Factura> builder)
        {
            builder.ToTable("FACTURA");
            builder.HasKey(f => f.Id);
            
            // Usar la secuencia creada en BD para generar el número sin usar Identity
            builder.Property(f => f.Numero)
                   .HasDefaultValueSql("NEXT VALUE FOR Seq_Factura_Numero")
                   .ValueGeneratedOnAdd();
                   
            builder.Property(f => f.Total).HasPrecision(10, 2);
            
            builder.HasOne(f => f.RegistroServicio)
                   .WithMany()
                   .HasForeignKey(f => f.RegistroServicioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
