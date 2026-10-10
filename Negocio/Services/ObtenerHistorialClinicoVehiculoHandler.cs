using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ObtenerHistorialClinicoVehiculoHandler
    {
        private readonly IReporteRepository _reporteRepository;

        public ObtenerHistorialClinicoVehiculoHandler(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<List<HistorialClinicoVehiculoDto>> HandleAsync(int vehiculoId)
        {
            return await _reporteRepository.ObtenerHistorialClinicoVehiculoAsync(vehiculoId);
        }
    }
}
