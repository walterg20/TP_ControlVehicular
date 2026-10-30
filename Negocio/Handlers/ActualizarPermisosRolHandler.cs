// walterg20
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;

namespace TP_ControlVehicular.Negocio.Handlers
{
    public class ActualizarPermisosRolHandler
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ActualizarPermisosRolHandler(IDbContextFactory<CVDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> HandleAsync(int idRol, List<int> permisosIds)
        {
            await using var _context = _contextFactory.CreateDbContext();

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
