// walterg20
namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    /// <summary>
    /// Fila del ranking de modelos con el total de registros que cumplen el filtro.
    /// La consume ReporteGerencialRepository.ObtenerModelosMasReparadosPaginadoAsync.
    /// </summary>
    public class ReporteModeloReparadoPaginadoDto : ReporteModeloReparadoDto
    {
        public int TotalRegistros { get; set; }
    }
}
