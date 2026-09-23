using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        private readonly CVDbContext _cvDbContext;

        public ServicioRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<List<Servicio>> GetActivosAsync() =>
            await _cvDbContext.Servicios
                .Where(s => s.Activo)
                .ToListAsync();

        public async Task<Servicio?> GetByNombreAsync(string nombre) =>
            await _cvDbContext.Servicios
                .FirstOrDefaultAsync(s => s.Nombre == nombre);
    }
}
