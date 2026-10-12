import io

obtener_handler = '''using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Handlers
{
    public class ObtenerPermisosHandler
    {
        private readonly CVDbContext _context;

        public ObtenerPermisosHandler(CVDbContext context)
        {
            _context = context;
        }

        public async Task<List<PermisoDto>> HandleAsync()
        {
            var permisos = await _context.Permisos
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return permisos.Select(p => new PermisoDto 
            {
                IdPermiso = p.IdPermiso,
                Nombre = p.Nombre,
                Asignado = false
            }).ToList();
        }
    }
}
'''

actualizar_handler = '''using System.Collections.Generic;
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
'''

with io.open('Negocio/Handlers/ObtenerPermisosHandler.cs', 'w', encoding='utf-8') as f:
    f.write(obtener_handler)

with io.open('Negocio/Handlers/ActualizarPermisosRolHandler.cs', 'w', encoding='utf-8') as f:
    f.write(actualizar_handler)
