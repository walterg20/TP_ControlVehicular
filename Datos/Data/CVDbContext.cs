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
        public DbSet<Taller> Talleres { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Servicio> Servicios { get; set; } = null!;
        public DbSet<RegistroServicio> RegistroServicios { get; set; } = null!;
        public DbSet<DetalleServicio> DetalleServicios { get; set; } = null!;
        public DbSet<Factura> Facturas { get; set; } = null!;
        public DbSet<MetodoPago> MetodosPago { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;
        public DbSet<PropietarioVehiculo> PropietariosVehiculos { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CVDbContext).Assembly);
            
            modelBuilder.Entity<TP_ControlVehicular.Negocio.DTOs.Reportes.ReporteIngresosDto>().HasNoKey();
            modelBuilder.Entity<TP_ControlVehicular.Negocio.DTOs.Reportes.ReporteTiemposResolucionDto>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }
    }
}

