// walterg20
namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    /// <summary>
    /// Fila del desglose de servicios por cantidad con el total de registros que cumplen
    /// el filtro. La consume ReporteGerencialRepository.ObtenerServiciosPorRangoPaginadoAsync.
    /// </summary>
    public class ReporteServicioCantidadPaginadoDto : ReporteServicioCantidadDto
    {
        public int TotalRegistros { get; set; }
    }
}
