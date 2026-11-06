// walterg20
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class RolRepository : Repository<Rol>, IRolRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public RolRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Rol>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Roles.Include(r => r.Permisos).Where(r => r.Activo).ToListAsync();
        }

        public async Task<(IEnumerable<Rol> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Rol, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = r => EF.Functions.Like(r.Nombre, patron);
            }

            Expression<Func<Rol, bool>>? filtroEstadoExpr = null;
            var estado = (filtroEstado ?? "Todos").Trim();
            if (string.Equals(estado, "Activos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = r => r.Activo;
            else if (string.Equals(estado, "Inactivos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = r => !r.Activo;

            Expression<Func<Rol, bool>>? filtroFinal = null;
            if (filtro != null && filtroEstadoExpr != null)
            {
                var param = Expression.Parameter(typeof(Rol), "r");
                var body = Expression.AndAlso(
                    Expression.Invoke(filtro, param),
                    Expression.Invoke(filtroEstadoExpr, param));
                filtroFinal = Expression.Lambda<Func<Rol, bool>>(body, param);
            }
            else if (filtro != null)
                filtroFinal = filtro;
            else if (filtroEstadoExpr != null)
                filtroFinal = filtroEstadoExpr;

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtroFinal,
                // Los permisos del rol los usa FrmRolPermiso al abrir el rol seleccionado.
                q => q.Include(r => r.Permisos).AsSplitQuery(),
                q => q.OrderBy(r => r.Id));
        }
    }
}
