// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    /// <summary>
    /// Fila del reporte de ordenes con el total de registros que cumplen el filtro.
    /// La consume ReporteRepository.ObtenerReporteOrdenesPaginadoAsync.
    /// </summary>
    public class OrdenTrabajoReportePaginadoDto : OrdenTrabajoReporteDto
    {
        public int TotalRegistros { get; set; }
    }
}
