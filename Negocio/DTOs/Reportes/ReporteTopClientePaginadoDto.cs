// walterg20
namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    /// <summary>
    /// Fila del ranking de clientes con el total de registros que cumplen el filtro.
    /// La consume ReporteGerencialRepository.ObtenerTopClientesPaginadoAsync.
    /// </summary>
    public class ReporteTopClientePaginadoDto : ReporteTopClienteDto
    {
        public int TotalRegistros { get; set; }
    }
}
