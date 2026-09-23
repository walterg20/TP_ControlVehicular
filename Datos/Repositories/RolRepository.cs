using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class RolRepository : Repository<Rol>, IRolRepository
    {
        private readonly CVDbContext _cvDbContext;

        public RolRepository(CVDbContext cvDbContext) : base(cvDbContext)
        {
            _cvDbContext = cvDbContext;
        }

        public async Task<List<Rol>> GetActivosAsync() =>
            await _cvDbContext.Roles.Where(r => r.Estado).ToListAsync();
    }
}
