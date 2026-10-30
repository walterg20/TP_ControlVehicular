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

        public async Task<IEnumerable<Modelo>> GetByMarcaAsync(int idMarca)
        {
            await using var ctx = CrearContexto();
            return await ctx.Modelos
                .Include(m => m.Marca)
                .Where(m => m.MarcaId == idMarca)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Modelo> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
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

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtro,
                q => q.Include(m => m.Marca),
                q => q.OrderBy(m => m.Id));
        }
    }
}
