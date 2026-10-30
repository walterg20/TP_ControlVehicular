// walterg20
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
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ObtenerPermisosHandler(IDbContextFactory<CVDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PermisoDto>> HandleAsync()
        {
            await using var _context = _contextFactory.CreateDbContext();

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
