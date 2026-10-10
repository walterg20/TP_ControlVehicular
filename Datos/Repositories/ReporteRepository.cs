using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Datos.Repositories
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly CVDbContext _dbContext;

        public ReporteRepository(CVDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ReporteIngresosDto>> ObtenerReporteIngresosAsync(DateTime startDate, DateTime endDate, int? workshopId)
        {
            var startDateParam = new SqlParameter("@StartDate", startDate);
            var endDateParam = new SqlParameter("@EndDate", endDate);
            var workshopIdParam = new SqlParameter("@WorkshopId", (object?)workshopId ?? DBNull.Value);

            return await _dbContext.Database
                .SqlQueryRaw<ReporteIngresosDto>(
                    "EXEC sp_ReporteIngresos @StartDate, @EndDate, @WorkshopId",
                    startDateParam, endDateParam, workshopIdParam)
                .ToListAsync();
        }

        public async Task<List<ReporteTiemposResolucionDto>> ObtenerReporteTiemposResolucionAsync(DateTime startDate, DateTime endDate, int? workshopId)
        {
            var startDateParam = new SqlParameter("@StartDate", startDate);
            var endDateParam = new SqlParameter("@EndDate", endDate);
            var workshopIdParam = new SqlParameter("@WorkshopId", (object?)workshopId ?? DBNull.Value);

            return await _dbContext.Database
                .SqlQueryRaw<ReporteTiemposResolucionDto>(
                    "EXEC sp_ReporteTiemposResolucion @StartDate, @EndDate, @WorkshopId",
                    startDateParam, endDateParam, workshopIdParam)
                .ToListAsync();
        }
        public async Task<List<HistorialVehiculoDto>> ObtenerHistorialVehiculoAsync(int? vehiculoId, string? patente)
        {
            var vehiculoIdParam = new SqlParameter("@VehiculoId", (object?)vehiculoId ?? DBNull.Value);
            var patenteParam = new SqlParameter("@Patente", string.IsNullOrEmpty(patente) ? DBNull.Value : patente);

            return await _dbContext.Database
                .SqlQueryRaw<HistorialVehiculoDto>(
                    "EXEC sp_HistorialVehiculo @VehiculoId, @Patente",
                    vehiculoIdParam, patenteParam)
                .ToListAsync();
        }

        public async Task<List<VehiculoClienteDto>> ObtenerVehiculosPorClienteAsync(int? clienteId, string? dni)
        {
            var clienteIdParam = new SqlParameter("@ClienteId", (object?)clienteId ?? DBNull.Value);
            var dniParam = new SqlParameter("@DNI", string.IsNullOrEmpty(dni) ? DBNull.Value : dni);

            return await _dbContext.Database
                .SqlQueryRaw<VehiculoClienteDto>(
                    "EXEC sp_VehiculosPorCliente @ClienteId, @DNI",
                    clienteIdParam, dniParam)
                .ToListAsync();
        }

        public async Task<List<ReporteServicioDto>> ObtenerServiciosPorMecanicoAsync(DateTime desde, DateTime hasta, int mecanicoId)
        {
            var fechaDesdeParam = new SqlParameter("@FechaDesde", desde);
            var fechaHastaParam = new SqlParameter("@FechaHasta", hasta);
            var mecanicoIdParam = new SqlParameter("@MecanicoId", mecanicoId);

            return await _dbContext.Database
                .SqlQueryRaw<ReporteServicioDto>(
                    "EXEC sp_ServiciosPorMecanico @FechaDesde, @FechaHasta, @MecanicoId",
                    fechaDesdeParam, fechaHastaParam, mecanicoIdParam)
                .ToListAsync();
        }

        public async Task<List<HistorialClinicoVehiculoDto>> ObtenerHistorialClinicoVehiculoAsync(int vehiculoId)
        {
            var vehiculoIdParam = new SqlParameter("@VehiculoId", vehiculoId);
            return await _dbContext.Database
                .SqlQueryRaw<HistorialClinicoVehiculoDto>(
                    "EXEC sp_HistorialClinicoVehiculo @VehiculoId",
                    vehiculoIdParam)
                .ToListAsync();
        }

        public async Task<List<HojaTrabajoDiariaDto>> ObtenerHojaTrabajoDiariaAsync(int mecanicoId, DateTime? fecha)
        {
            var mecanicoIdParam = new SqlParameter("@MecanicoId", mecanicoId);
            var fechaParam = new SqlParameter("@Fecha", (object?)fecha ?? DBNull.Value);
            return await _dbContext.Database
                .SqlQueryRaw<HojaTrabajoDiariaDto>(
                    "EXEC sp_ReporteHojaTrabajoDiaria @MecanicoId, @Fecha",
                    mecanicoIdParam, fechaParam)
                .ToListAsync();
        }

    }
}
