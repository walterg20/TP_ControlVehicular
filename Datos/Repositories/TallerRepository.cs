using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class TallerRepository: Repository<Taller>, ITallerRepository
    {
        private readonly CVDbContext _context;
        public TallerRepository(CVDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Taller>> GetActivosAsync() =>
            await _context.Talleres.Where(t => t.Activo).ToListAsync();
    
    }
}
