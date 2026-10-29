// walterg20
namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    /// <summary>
    /// Vehiculo de cliente con el total de registros que cumplen el filtro.
    /// La consume ReporteRepository.ObtenerVehiculosPorClientePaginadoAsync.
    /// </summary>
    public class VehiculoClientePaginadoDto : VehiculoClienteDto
    {
        public int TotalRegistros { get; set; }
    }
}
