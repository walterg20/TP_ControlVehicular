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
    public class VehiculoRepository : Repository<Vehiculo>, IVehiculoRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public VehiculoRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Vehiculo>> GetByClienteAsync(int idCliente)
        {
            await using var ctx = CrearContexto();
            return await ctx.Vehiculos
                .Include(v => v.Propietarios).ThenInclude(p => p.Cliente)
                .Include(v => v.Modelo)
                .ThenInclude(m => m.Marca)
                .Where(v => v.Propietarios.Any(p => p.ClienteId == idCliente && p.EsActual))
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehiculo>> GetAllWithDetailsAsync(int? mecanicoId = null)
        {
            await using var ctx = CrearContexto();
            var query = ctx.Vehiculos
                .Include(v => v.Propietarios).ThenInclude(p => p.Cliente)
                .Include(v => v.Modelo)
                .ThenInclude(m => m.Marca)
                .AsQueryable();

            if (mecanicoId.HasValue)
            {
                query = query.Where(v => ctx.RegistroServicios
                    .Any(rs => rs.VehiculoId == v.Id && rs.Detalles.Any(ds => ds.UsuarioId == mecanicoId.Value)));
            }

            return await query.ToListAsync();
        }

        public async Task<Vehiculo?> GetByPatenteAsync(string patente)
        {
            await using var ctx = CrearContexto();
            return await ctx.Vehiculos
                .Include(v => v.Propietarios)
                .ThenInclude(p => p.Cliente)
                .Include(v => v.Modelo)
                .ThenInclude(m => m.Marca)
                .FirstOrDefaultAsync(v => v.Patente.ToLower() == patente.ToLower());
        }

        public async Task<(IEnumerable<Vehiculo> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina)
        {
            Expression<Func<Vehiculo, bool>>? filtro = null;

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var patron = CrearPatronLike(busqueda.Trim());
                filtro = v => EF.Functions.Like(v.Patente, patron)
                          || EF.Functions.Like(v.Modelo.NombreModelo, patron)
                          || EF.Functions.Like(v.Modelo.Marca.NombreMarca, patron)
                          || v.Propietarios.Any(p => p.EsActual
                                && (EF.Functions.Like(p.Cliente.Nombre, patron)
                                 || EF.Functions.Like(p.Cliente.Apellido, patron)
                                 || EF.Functions.Like(p.Cliente.Dni, patron)));
            }

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                filtro,
                // AsSplitQuery: pagina primero y despues carga las colecciones relacionadas,
                // evitando que el JOIN 1:N altere el Skip/Take.
                q => q.Include(v => v.Propietarios).ThenInclude(p => p.Cliente)
                      .Include(v => v.Modelo).ThenInclude(m => m.Marca)
                      .AsSplitQuery(),
                q => q.OrderBy(v => v.Id));
        }
    }
}
