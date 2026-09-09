using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class RegistroServicioRepository : Repository<RegistroServicio>, IRegistroServicioRepository
    {
        private readonly CVDbContext _cvDbContext;

        public RegistroServicioRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<List<RegistroServicio>> GetReporteCompletoAsync()
        {
            return await _cvDbContext.RegistroServicios
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Cliente)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                        .ThenInclude(m => m.Marca)
                .Include(r => r.Usuario) // Recepcionista
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Servicio)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Usuario) // Mecánico
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }
    }
}
