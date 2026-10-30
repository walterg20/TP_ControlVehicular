// walterg20
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class RegistroServicioRepository : Repository<RegistroServicio>, IRegistroServicioRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public RegistroServicioRepository(IDbContextFactory<CVDbContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public new async Task UpdateAsync(RegistroServicio entity)
        {
            await using var ctx = CrearContexto();
            ctx.ChangeTracker.Clear();

            var existente = await ctx.RegistroServicios
                .Include(r => r.Detalles)
                .FirstOrDefaultAsync(r => r.Id == entity.Id);

            if (existente != null)
            {
                ctx.Entry(existente).CurrentValues.SetValues(entity);

                // Eliminar detalles que ya no están
                foreach (var detalleExistente in existente.Detalles.ToList())
                {
                    if (!entity.Detalles.Any(d => d.Id == detalleExistente.Id))
                    {
                        ctx.DetalleServicios.Remove(detalleExistente);
                    }
                }

                // Asegurarse de que el Id del registro de servicio coincide
                foreach (var detalle in entity.Detalles)
                {
                    detalle.RegistroServicioId = existente.Id;
                    existente.Detalles.Add(detalle);
                }

                await ctx.SaveChangesAsync();
            }
            else
            {
                ctx.RegistroServicios.Update(entity);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<RegistroServicio>> GetReporteCompletoAsync(int? mecanicoId = null)
        {
            await using var ctx = CrearContexto();
            var query = ctx.RegistroServicios
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Propietarios).ThenInclude(p => p.Cliente)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                        .ThenInclude(m => m.Marca)
                .Include(r => r.Usuario) // Recepcionista
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Servicio)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Usuario) // Mecánico
                .AsNoTracking().AsQueryable();

            if (mecanicoId.HasValue)
            {
                query = query.Where(r => r.Detalles.Any(d => d.UsuarioId == mecanicoId.Value));
            }

            return await query.OrderByDescending(r => r.Fecha).ToListAsync();
        }

        /// <summary>
        /// Include común de las consultas de órdenes de servicio.
        /// AsSplitQuery evita que el JOIN 1:N de las colecciones altere el Skip/Take.
        /// </summary>
        private static IQueryable<RegistroServicio> BasePaginado(IQueryable<RegistroServicio> query) =>
            query.Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Propietarios).ThenInclude(p => p.Cliente)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                        .ThenInclude(m => m.Marca)
                .Include(r => r.Usuario) // Recepcionista
                .Include(r => r.Taller)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Servicio)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Usuario) // Mecánico
                .AsSplitQuery();

        /// <summary>
        /// Aplica los filtros comunes (texto, estado y rango de fechas) de los listados de órdenes.
        /// </summary>
        private static IQueryable<RegistroServicio> AplicarFiltros(
            IQueryable<RegistroServicio> query,
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            var texto = busqueda?.Trim();
            var patron = string.IsNullOrWhiteSpace(texto) ? null : CrearPatronLike(texto);
            var estadoFiltro = string.IsNullOrWhiteSpace(estado) || estado == "Todos" ? null : estado.Trim();

            if (patron != null)
            {
                var p = patron;
                query = query.Where(r => EF.Functions.Like(r.Vehiculo.Patente, p)
                                     || EF.Functions.Like(r.Vehiculo.Modelo.NombreModelo, p)
                                     || EF.Functions.Like(r.Vehiculo.Modelo.Marca.NombreMarca, p)
                                     || EF.Functions.Like(r.Usuario.Nombre, p)
                                     || EF.Functions.Like(r.Taller.Nombre, p)
                                     || EF.Functions.Like(r.Estado, p)
                                     || r.Vehiculo.Propietarios.Any(pv => pv.EsActual && pv.Cliente != null
                                          && (EF.Functions.Like(pv.Cliente.Nombre, p)
                                           || EF.Functions.Like(pv.Cliente.Apellido, p)
                                           || EF.Functions.Like(pv.Cliente.Dni, p))));
            }

            if (estadoFiltro != null)
            {
                var e = estadoFiltro;
                query = query.Where(r => r.Estado == e);
            }

            if (fechaDesde.HasValue)
            {
                var fd = fechaDesde.Value.Date;
                query = query.Where(r => r.Fecha.Date >= fd);
            }

            if (fechaHasta.HasValue)
            {
                var fh = fechaHasta.Value.Date;
                query = query.Where(r => r.Fecha.Date <= fh);
            }

            return query;
        }

        /// <summary>
        /// Recupera todas las órdenes que cumplen los filtros, sin paginar.
        /// Lo usa la exportación a PDF del listado de órdenes.
        /// </summary>
        public async Task<List<RegistroServicio>> GetFiltradoAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            await using var ctx = CrearContexto();
            var query = AplicarFiltros(
                BasePaginado(ctx.RegistroServicios.AsNoTracking().AsQueryable()),
                busqueda, estado, fechaDesde, fechaHasta);

            return await query.OrderByDescending(r => r.Id).ToListAsync();
        }

        public async Task<(IEnumerable<RegistroServicio> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina,
            int tamanoPagina)
        {
            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                null,
                q => AplicarFiltros(BasePaginado(q), busqueda, estado, fechaDesde, fechaHasta),
                q => q.OrderByDescending(r => r.Id));
        }

        public async Task<(IEnumerable<RegistroServicio> Items, int TotalRegistros)> GetPaginadoPorMecanicoAsync(
            int usuarioId,
            string? busqueda,
            string? filtroTareas,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina,
            int tamanoPagina)
        {
            var texto = busqueda?.Trim();
            var patron = string.IsNullOrWhiteSpace(texto) ? null : CrearPatronLike(texto);

            return await GetPaginadoAsync(
                pagina,
                tamanoPagina,
                null,
                q =>
                {
                    // Solo ordenes con tareas del mecánico y no canceladas.
                    var query = BasePaginado(q)
                        .Where(r => r.Estado != RegistroServicio.EstadoCancelada)
                        .Where(r => r.Detalles.Any(d => d.UsuarioId == usuarioId));

                    if (filtroTareas == "Pendientes")
                    {
                        query = query.Where(r => r.Detalles.Any(d => d.UsuarioId == usuarioId && d.Estado != "Finalizada"));
                    }
                    else if (filtroTareas == "Finalizados")
                    {
                        query = query.Where(r => !r.Detalles.Any(d => d.UsuarioId == usuarioId && d.Estado != "Finalizada"));
                    }

                    if (patron != null)
                    {
                        var p = patron;
                        query = query.Where(r => EF.Functions.Like(r.Vehiculo.Patente, p)
                                             || EF.Functions.Like(r.Vehiculo.Modelo.NombreModelo, p)
                                             || EF.Functions.Like(r.Vehiculo.Modelo.Marca.NombreMarca, p)
                                             || r.Vehiculo.Propietarios.Any(pv => pv.EsActual && pv.Cliente != null
                                                  && (EF.Functions.Like(pv.Cliente.Nombre, p)
                                                   || EF.Functions.Like(pv.Cliente.Apellido, p))));
                    }

                    // El número de orden se busca como texto; se compara como entero cuando el
                    // término es numérico (r.Id.ToString() no es traducible a SQL).
                    if (texto != null && int.TryParse(texto, out int idOrden))
                    {
                        query = query.Where(r => r.Id == idOrden);
                    }

                    if (fechaDesde.HasValue)
                    {
                        var fd = fechaDesde.Value.Date;
                        query = query.Where(r => r.Fecha.Date >= fd);
                    }

                    if (fechaHasta.HasValue)
                    {
                        var fh = fechaHasta.Value.Date;
                        query = query.Where(r => r.Fecha.Date <= fh);
                    }

                    return query;
                },
                q => q.OrderByDescending(r => r.Id));
        }
    }
}
