using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        private readonly CVDbContext _context;

        public ServicioRepository(CVDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Servicio>> GetActivosAsync() =>
            await _context.Servicios.Where(s => s.Activo).ToListAsync();
    }
}
