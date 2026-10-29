// walterg20
namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    /// <summary>
    /// Historial de vehiculo con el total de registros que cumplen el filtro.
    /// La consume ReporteRepository.ObtenerHistorialVehiculoPaginadoAsync.
    /// </summary>
    public class HistorialVehiculoPaginadoDto : HistorialVehiculoDto
    {
        public int TotalRegistros { get; set; }
    }
}
