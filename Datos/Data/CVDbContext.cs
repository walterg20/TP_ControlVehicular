using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Entidad;


namespace TP_ControlVehicular.Datos.Data
{
    public class CVDbContext : DbContext
    {
        public CVDbContext(DbContextOptions<CVDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Vehiculo> Vehiculos { get; set; } = null!;
        public DbSet<Modelo> Modelos { get; set; } = null!;
        public DbSet<Marca> Marcas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CVDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
