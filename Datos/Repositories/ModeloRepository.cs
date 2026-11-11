// walterg20
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ModeloRepository : Repository<Modelo>, IModeloRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ModeloRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public override async Task<IEnumerable<Modelo>> GetAllAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Modelos
                .Include(m => m.Marca)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<IEnumerable<Modelo>> GetTodosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Modelos
                .Include(m => m.Marca)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Modelo?> GetByIdAsync(int id)
        {
            await using var ctx = CrearContexto();
            return await ctx.Modelos
                .Include(m => m.Marca)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Modelo>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Modelos.AsNoTracking()
                .Include(m => m.Marca)
                .Where(m => m.Activo)
                .OrderBy(m => m.NombreModelo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Modelo>> GetByMarcaAsync(int idMarca)
        {
            await using var ctx = CrearContexto();
            return await ctx.Modelos
                .Include(m => m.Marca)
                .Where(m => m.MarcaId == idMarca && m.Activo)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Modelo> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Modelo, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = m => EF.Functions.Like(m.NombreModelo, patron)
                          || EF.Functions.Like(m.Marca.NombreMarca, patron);
            }

            Expression<Func<Modelo, bool>>? filtroEstadoExpr = null;
            var estado = (filtroEstado ?? "Todos").Trim();
            if (string.Equals(estado, "Activos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = m => m.Activo;
            else if (string.Equals(estado, "Inactivos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = m => !m.Activo;

            Expression<Func<Modelo, bool>>? filtroFinal = null;
            if (filtro != null && filtroEstadoExpr != null)
            {
                var param = Expression.Parameter(typeof(Modelo), "m");
                var body = Expression.AndAlso(
                    Expression.Invoke(filtro, param),
                    Expression.Invoke(filtroEstadoExpr, param));
                filtroFinal = Expression.Lambda<Func<Modelo, bool>>(body, param);
            }
            else if (filtro != null)
                filtroFinal = filtro;
            else if (filtroEstadoExpr != null)
                filtroFinal = filtroEstadoExpr;

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtroFinal,
                q => q.Include(m => m.Marca),
                q => q.OrderBy(m => m.Id));
        }
    }
}
