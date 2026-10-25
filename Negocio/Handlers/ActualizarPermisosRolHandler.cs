using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;

namespace TP_ControlVehicular.Negocio.Handlers
{
    public class ActualizarPermisosRolHandler
    {
        private readonly CVDbContext _context;

        public ActualizarPermisosRolHandler(CVDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HandleAsync(int idRol, List<int> permisosIds)
        {
            var rol = await _context.Roles
                .Include(r => r.Permisos)
                .FirstOrDefaultAsync(r => r.Id == idRol);

            if (rol == null) return false;

            var nuevosPermisos = await _context.Permisos
                .Where(p => permisosIds.Contains(p.IdPermiso))
                .ToListAsync();

            rol.Permisos.Clear();
            foreach (var p in nuevosPermisos)
            {
                rol.Permisos.Add(p);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
