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
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ServicioRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Servicio>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Servicios
                .Where(s => s.Activo)
                .ToListAsync();
        }

        public async Task<Servicio?> GetByNombreAsync(string nombre)
        {
            await using var ctx = CrearContexto();
            return await ctx.Servicios
                .FirstOrDefaultAsync(s => s.Nombre == nombre);
        }

        public async Task<(IEnumerable<Servicio> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Servicio, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = s => EF.Functions.Like(s.Nombre, patron);
            }

            Expression<Func<Servicio, bool>>? filtroEstadoExpr = null;
            var estado = (filtroEstado ?? "Todos").Trim();
            if (string.Equals(estado, "Activos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = s => s.Activo;
            else if (string.Equals(estado, "Inactivos", StringComparison.OrdinalIgnoreCase))
                filtroEstadoExpr = s => !s.Activo;

            Expression<Func<Servicio, bool>>? filtroFinal = null;
            if (filtro != null && filtroEstadoExpr != null)
            {
                var param = Expression.Parameter(typeof(Servicio), "s");
                var body = Expression.AndAlso(
                    Expression.Invoke(filtro, param),
                    Expression.Invoke(filtroEstadoExpr, param));
                filtroFinal = Expression.Lambda<Func<Servicio, bool>>(body, param);
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
                q => q.OrderBy(s => s.Id));
        }
    }
}
