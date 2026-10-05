using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IReporteRepository
    {
        Task<List<ReporteIngresosDto>> ObtenerReporteIngresosAsync(DateTime startDate, DateTime endDate, int? workshopId);
        Task<List<ReporteTiemposResolucionDto>> ObtenerReporteTiemposResolucionAsync(DateTime startDate, DateTime endDate, int? workshopId);
        Task<List<HistorialVehiculoDto>> ObtenerHistorialVehiculoAsync(int? vehiculoId, string? patente);
        Task<List<VehiculoClienteDto>> ObtenerVehiculosPorClienteAsync(int? clienteId, string? dni);
        Task<List<ReporteServicioDto>> ObtenerServiciosPorMecanicoAsync(DateTime desde, DateTime hasta, int mecanicoId);
    }
}
