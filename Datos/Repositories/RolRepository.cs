// walterg20
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
            await _cvDbContext.Roles.Include(r => r.Permisos).Where(r => r.Activo).ToListAsync();

        public async Task<(IEnumerable<Rol> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Rol, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = r => EF.Functions.Like(r.Nombre, patron);
            }

            Expression<Func<Rol, bool>>? soloActivos = null;
            if (!incluirInactivos)
            {
                soloActivos = r => r.Activo;
            }

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtro,
                // Los permisos del rol los usa FrmRolPermiso al abrir el rol seleccionado.
                q => (soloActivos is null
                        ? q.Include(r => r.Permisos)
                        : q.Include(r => r.Permisos).Where(soloActivos))
                      .AsSplitQuery(),
                q => q.OrderBy(r => r.Id));
        }
    }
}
