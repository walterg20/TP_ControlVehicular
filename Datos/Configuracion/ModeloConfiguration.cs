

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;


namespace TP_ControlVehicular.Datos.Configuracion
{
    public class ModeloConfiguration : IEntityTypeConfiguration<Modelo>
    {
        public void Configure(EntityTypeBuilder<Modelo> builder)
        {
            builder.ToTable("Modelo");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.NombreModelo)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasOne(m => m.Marca)
                   .WithMany(ma => ma.Modelos)
                   .HasForeignKey(m => m.MarcaId);
        }
    }
}
