using System.Collections.Generic;
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
