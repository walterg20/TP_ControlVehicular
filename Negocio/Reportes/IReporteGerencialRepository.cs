// walterg20
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Reportes
{
    public interface IReporteGerencialRepository
    {
        Task<IEnumerable<ReporteIngresoDto>> ObtenerIngresosPorFechaAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<(IEnumerable<ReporteIngresoDto> Items, int TotalRegistros)> ObtenerIngresosPorFechaPaginadoAsync(DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina);
        Task<IEnumerable<ReporteIngresoMensualDto>> ObtenerIngresosMensualesAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<IEnumerable<ReporteTopClienteDto>> ObtenerTopClientesAsync(DateTime? fechaDesde, DateTime? fechaHasta, int topN = 10);
        Task<IEnumerable<ReporteModeloReparadoDto>> ObtenerModelosMasReparadosAsync(DateTime? fechaDesde, DateTime? fechaHasta, int topN = 10);
        Task<IEnumerable<ProductividadMecanicoDto>> ObtenerProductividadMecanicosAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<IEnumerable<ReporteServicioCantidadDto>> ObtenerServiciosPorRangoAsync(DateTime? fechaDesde, DateTime? fechaHasta);

        /// <summary>
        /// Ranking de clientes con paginacion a nivel de base de datos.
        /// Con <paramref name="tamanoPagina"/> en 0 devuelve el conjunto completo.
        /// </summary>
        Task<(IEnumerable<ReporteTopClienteDto> Items, int TotalRegistros)> ObtenerTopClientesPaginadoAsync(
            DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina);

        /// <summary>
        /// Ranking de modelos mas reparados con paginacion a nivel de base de datos.
        /// Con <paramref name="tamanoPagina"/> en 0 devuelve el conjunto completo.
        /// </summary>
        Task<(IEnumerable<ReporteModeloReparadoDto> Items, int TotalRegistros)> ObtenerModelosMasReparadosPaginadoAsync(
            DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina);

        /// <summary>
        /// Servicios por cantidad con paginacion a nivel de base de datos.
        /// Con <paramref name="tamanoPagina"/> en 0 devuelve el conjunto completo.
        /// </summary>
        Task<(IEnumerable<ReporteServicioCantidadDto> Items, int TotalRegistros)> ObtenerServiciosPorRangoPaginadoAsync(
            DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina);
    }
}
