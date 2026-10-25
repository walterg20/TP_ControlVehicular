using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ObtenerReporteIngresosHandler
    {
        private readonly IReporteRepository _reporteRepository;

        public ObtenerReporteIngresosHandler(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<List<ReporteIngresosDto>> HandleAsync(DateTime startDate, DateTime endDate, int? workshopId)
        {
            return await _reporteRepository.ObtenerReporteIngresosAsync(startDate, endDate, workshopId);
        }
    }
}
