// walterg20
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly IDbContextFactory<CVDbContext> _contextFactory;

        public ReporteRepository(IDbContextFactory<CVDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>Crea un contexto nuevo, exclusivo de la operacion que lo va a usar.</summary>
        private CVDbContext CrearContexto() => _contextFactory.CreateDbContext();

        public async Task<List<ReporteIngresosDto>> ObtenerReporteIngresosAsync(DateTime startDate, DateTime endDate, int? workshopId)
        {
            await using var ctx = CrearContexto();
            var startDateParam = new SqlParameter("@StartDate", startDate);
            var endDateParam = new SqlParameter("@EndDate", endDate);
            var workshopIdParam = new SqlParameter("@WorkshopId", (object?)workshopId ?? DBNull.Value);

            return await ctx.Database
                .SqlQueryRaw<ReporteIngresosDto>(
                    "EXEC sp_ReporteIngresos @StartDate, @EndDate, @WorkshopId",
                    startDateParam, endDateParam, workshopIdParam)
                .ToListAsync();
        }

        public async Task<List<ReporteTiemposResolucionDto>> ObtenerReporteTiemposResolucionAsync(DateTime startDate, DateTime endDate, int? workshopId)
        {
            await using var ctx = CrearContexto();
            var startDateParam = new SqlParameter("@StartDate", startDate);
            var endDateParam = new SqlParameter("@EndDate", endDate);
            var workshopIdParam = new SqlParameter("@WorkshopId", (object?)workshopId ?? DBNull.Value);

            return await ctx.Database
                .SqlQueryRaw<ReporteTiemposResolucionDto>(
                    "EXEC sp_ReporteTiemposResolucion @StartDate, @EndDate, @WorkshopId",
                    startDateParam, endDateParam, workshopIdParam)
                .ToListAsync();
        }
        public async Task<List<HistorialVehiculoDto>> ObtenerHistorialVehiculoAsync(int? vehiculoId, string? patente)
        {
            await using var ctx = CrearContexto();
            var vehiculoIdParam = new SqlParameter("@VehiculoId", (object?)vehiculoId ?? DBNull.Value);
            var patenteParam = new SqlParameter("@Patente", string.IsNullOrEmpty(patente) ? DBNull.Value : patente);

            return await ctx.Database
                .SqlQueryRaw<HistorialVehiculoDto>(
                    "EXEC sp_HistorialVehiculo @VehiculoId, @Patente",
                    vehiculoIdParam, patenteParam)
                .ToListAsync();
        }

        public async Task<List<VehiculoClienteDto>> ObtenerVehiculosPorClienteAsync(int? clienteId, string? dni)
        {
            await using var ctx = CrearContexto();
            var clienteIdParam = new SqlParameter("@ClienteId", (object?)clienteId ?? DBNull.Value);
            var dniParam = new SqlParameter("@DNI", string.IsNullOrEmpty(dni) ? DBNull.Value : dni);

            return await ctx.Database
                .SqlQueryRaw<VehiculoClienteDto>(
                    "EXEC sp_VehiculosPorCliente @ClienteId, @DNI",
                    clienteIdParam, dniParam)
                .ToListAsync();
        }

        public async Task<List<ReporteServicioDto>> ObtenerServiciosPorMecanicoAsync(DateTime desde, DateTime hasta, int mecanicoId)
        {
            await using var ctx = CrearContexto();
            var fechaDesdeParam = new SqlParameter("@FechaDesde", desde);
            var fechaHastaParam = new SqlParameter("@FechaHasta", hasta);
            var mecanicoIdParam = new SqlParameter("@MecanicoId", mecanicoId);

            return await ctx.Database
                .SqlQueryRaw<ReporteServicioDto>(
                    "EXEC sp_ServiciosPorMecanico @FechaDesde, @FechaHasta, @MecanicoId",
                    fechaDesdeParam, fechaHastaParam, mecanicoIdParam)
                .ToListAsync();
        }

        public async Task<List<HistorialClinicoVehiculoDto>> ObtenerHistorialClinicoVehiculoAsync(int vehiculoId)
        {
            await using var ctx = CrearContexto();
            var vehiculoIdParam = new SqlParameter("@VehiculoId", vehiculoId);

            return await ctx.Database
                .SqlQueryRaw<HistorialClinicoVehiculoDto>(
                    "EXEC sp_HistorialClinicoVehiculo @VehiculoId",
                    vehiculoIdParam)
                .ToListAsync();
        }

        public async Task<List<HojaTrabajoDiariaDto>> ObtenerHojaTrabajoDiariaAsync(int mecanicoId, DateTime? fecha)
        {
            await using var ctx = CrearContexto();
            var mecanicoIdParam = new SqlParameter("@MecanicoId", mecanicoId);
            var fechaParam = new SqlParameter("@Fecha", (object?)fecha ?? DBNull.Value);

            return await ctx.Database
                .SqlQueryRaw<HojaTrabajoDiariaDto>(
                    "EXEC sp_ReporteHojaTrabajoDiaria @MecanicoId, @Fecha",
                    mecanicoIdParam, fechaParam)
                .ToListAsync();
        }

        public async Task<(List<HistorialVehiculoDto> Items, int TotalRegistros)> ObtenerHistorialVehiculoPaginadoAsync(
            int? vehiculoId, string? patente, int pagina, int tamanoPagina)
        {
            await using var ctx = CrearContexto();
            var vehiculoIdParam = new SqlParameter("@VehiculoId", (object?)vehiculoId ?? DBNull.Value);
            var patenteParam = new SqlParameter("@Patente", string.IsNullOrEmpty(patente) ? DBNull.Value : patente);
            var paginaParam = new SqlParameter("@Pagina", pagina);
            var tamanoParam = new SqlParameter("@TamanoPagina", tamanoPagina);

            var rows = (await ctx.Database
                .SqlQueryRaw<HistorialVehiculoPaginadoDto>(
                    "EXEC sp_HistorialVehiculoPaginado @VehiculoId, @Patente, @Pagina, @TamanoPagina",
                    vehiculoIdParam, patenteParam, paginaParam, tamanoParam)
                .ToListAsync());

            int totalRegistros = rows.FirstOrDefault()?.TotalRegistros ?? 0;
            return (rows.Cast<HistorialVehiculoDto>().ToList(), totalRegistros);
        }

        public async Task<(List<VehiculoClienteDto> Items, int TotalRegistros)> ObtenerVehiculosPorClientePaginadoAsync(
            int? clienteId, string? dni, int pagina, int tamanoPagina)
        {
            await using var ctx = CrearContexto();
            var clienteIdParam = new SqlParameter("@ClienteId", (object?)clienteId ?? DBNull.Value);
            var dniParam = new SqlParameter("@DNI", string.IsNullOrEmpty(dni) ? DBNull.Value : dni);
            var paginaParam = new SqlParameter("@Pagina", pagina);
            var tamanoParam = new SqlParameter("@TamanoPagina", tamanoPagina);

            var rows = (await ctx.Database
                .SqlQueryRaw<VehiculoClientePaginadoDto>(
                    "EXEC sp_VehiculosPorClientePaginado @ClienteId, @DNI, @Pagina, @TamanoPagina",
                    clienteIdParam, dniParam, paginaParam, tamanoParam)
                .ToListAsync());

            int totalRegistros = rows.FirstOrDefault()?.TotalRegistros ?? 0;
            return (rows.Cast<VehiculoClienteDto>().ToList(), totalRegistros);
        }

        public async Task<(List<OrdenTrabajoReporteDto> Items, int TotalRegistros)> ObtenerReporteOrdenesPaginadoAsync(
            string? textoBusqueda, string? estado, DateTime? fechaDesde, DateTime? fechaHasta,
            int? mecanicoId, int pagina, int tamanoPagina)
        {
            await using var ctx = CrearContexto();
            var textoParam = new SqlParameter("@TextoBusqueda", string.IsNullOrWhiteSpace(textoBusqueda) ? DBNull.Value : textoBusqueda.Trim());
            var estadoParam = new SqlParameter("@Estado", string.IsNullOrWhiteSpace(estado) ? DBNull.Value : estado);
            var desdeParam = new SqlParameter("@FechaDesde", (object?)fechaDesde?.ToString("yyyy-MM-dd") ?? DBNull.Value);
            var hastaParam = new SqlParameter("@FechaHasta", (object?)fechaHasta?.ToString("yyyy-MM-dd") ?? DBNull.Value);
            var mecanicoParam = new SqlParameter("@MecanicoId", (object?)mecanicoId ?? DBNull.Value);
            var paginaParam = new SqlParameter("@Pagina", pagina);
            var tamanoParam = new SqlParameter("@TamanoPagina", tamanoPagina);

            var rows = (await ctx.Database
                .SqlQueryRaw<OrdenTrabajoReportePaginadoDto>(
                    "EXEC sp_ReporteOrdenesPaginado @TextoBusqueda, @Estado, @FechaDesde, @FechaHasta, @MecanicoId, @Pagina, @TamanoPagina",
                    textoParam, estadoParam, desdeParam, hastaParam, mecanicoParam, paginaParam, tamanoParam)
                .ToListAsync());

            int totalRegistros = rows.FirstOrDefault()?.TotalRegistros ?? 0;
            return (rows.Cast<OrdenTrabajoReporteDto>().ToList(), totalRegistros);
        }

    }
}
