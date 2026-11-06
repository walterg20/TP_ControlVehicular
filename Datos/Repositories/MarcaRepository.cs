// walterg20
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Datos.Repositories
{
    public class MarcaRepository : Repository<Marca>, IMarcaRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public MarcaRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Marca>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Marcas.AsNoTracking()
                .Where(m => m.Activo)
                .OrderBy(m => m.NombreMarca)
                .ToListAsync();
        }

        public async Task<Marca?> GetByNombreAsync(string nombre)
        {
            await using var ctx = CrearContexto();
            return await ctx.Marcas
                .Include(m => m.Modelos)
                .FirstOrDefaultAsync(m => m.NombreMarca == nombre);
        }

        public async Task<(IEnumerable<Marca> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Marca, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = m => EF.Functions.Like(m.NombreMarca, patron);
            }

            // Filtro por estado
            Expression<Func<Marca, bool>>? filtroEstadoExpr = null;
            var estado = (filtroEstado ?? "Todos").Trim();
            if (string.Equals(estado, "Activos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = m => m.Activo;
            else if (string.Equals(estado, "Inactivos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = m => !m.Activo;

            // Combinar filtros
            Expression<Func<Marca, bool>>? filtroFinal = null;
            if (filtro != null && filtroEstadoExpr != null)
            {
                var param = Expression.Parameter(typeof(Marca), "m");
                var body = Expression.AndAlso(
                    Expression.Invoke(filtro, param),
                    Expression.Invoke(filtroEstadoExpr, param));
                filtroFinal = Expression.Lambda<Func<Marca, bool>>(body, param);
            }
            else if (filtro != null)
                filtroFinal = filtro;
            else if (filtroEstadoExpr != null)
                filtroFinal = filtroEstadoExpr;

            return await GetPaginadoAsync(pagina, tamanoPagina, filtroFinal, null, q => q.OrderBy(m => m.Id));
        }
    }
}
