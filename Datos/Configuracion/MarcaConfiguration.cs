using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP_ControlVehicular.Entidad;


namespace TP_ControlVehicular.Datos.Configuracion
{
    public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
    {
        public void Configure(EntityTypeBuilder<Marca> builder)
        {
            builder.ToTable("Marca");
            builder.HasKey(ma => ma.Id);

            builder.Property(ma => ma.NombreMarca)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(ma => ma.Modelos)
                   .WithOne(m => m.Marca)
                   .HasForeignKey(m => m.MarcaId);
        }
    }
}
