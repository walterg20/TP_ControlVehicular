using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ObtenerHojaTrabajoDiariaHandler
    {
        private readonly IReporteRepository _reporteRepository;

        public ObtenerHojaTrabajoDiariaHandler(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<List<HojaTrabajoDiariaDto>> HandleAsync(int mecanicoId, DateTime? fecha)
        {
            return await _reporteRepository.ObtenerHojaTrabajoDiariaAsync(mecanicoId, fecha);
        }
    }
}
