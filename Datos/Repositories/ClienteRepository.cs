// walterg20
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ClienteRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Cliente>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Clientes.Where(c => c.Activo).ToListAsync();
        }

        public async Task<(IEnumerable<Cliente> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Cliente, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = c => EF.Functions.Like(c.Nombre, patron)
                          || EF.Functions.Like(c.Apellido, patron)
                          || EF.Functions.Like(c.Dni, patron);
            }

            Expression<Func<Cliente, bool>>? filtroEstadoExpr = null;
            var estado = (filtroEstado ?? "Todos").Trim();
            if (string.Equals(estado, "Activos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = c => c.Activo;
            else if (string.Equals(estado, "Inactivos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = c => !c.Activo;

            Expression<Func<Cliente, bool>>? filtroFinal = null;
            if (filtro != null && filtroEstadoExpr != null)
            {
                var param = Expression.Parameter(typeof(Cliente), "c");
                var body = Expression.AndAlso(
                    Expression.Invoke(filtro, param),
                    Expression.Invoke(filtroEstadoExpr, param));
                filtroFinal = Expression.Lambda<Func<Cliente, bool>>(body, param);
            }
            else if (filtro != null)
                filtroFinal = filtro;
            else if (filtroEstadoExpr != null)
                filtroFinal = filtroEstadoExpr;

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtroFinal,
                null,
                q => q.OrderBy(c => c.Id));
        }
    }
}
