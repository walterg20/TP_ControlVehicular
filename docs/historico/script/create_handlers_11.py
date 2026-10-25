import io
import os

handler_historial = '''using System.Collections.Generic;
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
'''
with io.open('Negocio/Services/ObtenerHistorialClinicoVehiculoHandler.cs', 'w', encoding='utf-8') as f:
    f.write(handler_historial)

handler_hoja = '''using System;
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
'''
with io.open('Negocio/Services/ObtenerHojaTrabajoDiariaHandler.cs', 'w', encoding='utf-8') as f:
    f.write(handler_hoja)