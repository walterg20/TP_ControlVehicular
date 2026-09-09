using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class TallerRepository: Repository<Taller>, ITallerRepository
    {
        private readonly CVDbContext _cvDbContext;
        public TallerRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }
        public async Task<List<Taller>> GetActivosAsync() =>
            await _cvDbContext.Talleres.Where(t => t.Activo).ToListAsync();
    
    }
}
