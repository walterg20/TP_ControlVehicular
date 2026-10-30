// walterg20
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs;
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
        Task<List<HistorialClinicoVehiculoDto>> ObtenerHistorialClinicoVehiculoAsync(int vehiculoId);
        Task<List<HojaTrabajoDiariaDto>> ObtenerHojaTrabajoDiariaAsync(int mecanicoId, DateTime? fecha);

        /// <summary>
        /// Historial de un vehiculo con paginacion a nivel de base de datos.
        /// </summary>
        Task<(List<HistorialVehiculoDto> Items, int TotalRegistros)> ObtenerHistorialVehiculoPaginadoAsync(
            int? vehiculoId, string? patente, int pagina, int tamanoPagina);

        /// <summary>
        /// Vehiculos actuales de un cliente con paginacion a nivel de base de datos.
        /// </summary>
        Task<(List<VehiculoClienteDto> Items, int TotalRegistros)> ObtenerVehiculosPorClientePaginadoAsync(
            int? clienteId, string? dni, int pagina, int tamanoPagina);

        /// <summary>
        /// Reporte de ordenes con paginacion a nivel de base de datos.
        /// Con <paramref name="tamanoPagina"/> en 0 devuelve el conjunto completo
        /// (lo usa la exportacion a PDF).
        /// </summary>
        Task<(List<OrdenTrabajoReporteDto> Items, int TotalRegistros)> ObtenerReporteOrdenesPaginadoAsync(
            string? textoBusqueda, string? estado, DateTime? fechaDesde, DateTime? fechaHasta,
            int? mecanicoId, int pagina, int tamanoPagina);
    }
}
