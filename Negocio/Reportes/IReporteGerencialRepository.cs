using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;

namespace TP_ControlVehicular.Negocio.Reportes
{
    public interface IReporteGerencialRepository
    {
        Task<IEnumerable<ReporteIngresoDto>> ObtenerIngresosPorFechaAsync(DateTime? fechaDesde, DateTime? fechaHasta);
        Task<IEnumerable<ReporteTopClienteDto>> ObtenerTopClientesAsync(DateTime? fechaDesde, DateTime? fechaHasta, int topN = 10);
        Task<IEnumerable<ReporteModeloReparadoDto>> ObtenerModelosMasReparadosAsync(DateTime? fechaDesde, DateTime? fechaHasta, int topN = 10);
    }
}
