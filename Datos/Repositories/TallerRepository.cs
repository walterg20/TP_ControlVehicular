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
    public class TallerRepository : Repository<Taller>, ITallerRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public TallerRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Taller>> GetActivosAsync()
        {
            await using var ctx = CrearContexto();
            return await ctx.Talleres.Where(t => t.Activo).ToListAsync();
        }

        public async Task<(IEnumerable<Taller> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Taller, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = t => EF.Functions.Like(t.Nombre, patron);
            }

            Expression<Func<Taller, bool>>? soloActivos = null;
            if (!incluirInactivos)
            {
                soloActivos = t => t.Activo;
            }

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtro,
                q => soloActivos is null ? q : q.Where(soloActivos),
                q => q.OrderBy(t => t.Id));
        }
    }
}
