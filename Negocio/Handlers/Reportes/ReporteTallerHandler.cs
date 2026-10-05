using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Handlers.Reportes
{
    public class ReporteTallerHandler
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteTallerHandler(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<List<ReporteServicioDto>> ObtenerServiciosPorMecanicoAsync(DateTime desde, DateTime hasta, int mecanicoIdSeleccionado)
        {
            if (UserSession.CurrentUser == null)
                throw new UnauthorizedAccessException("Usuario no autenticado.");

            int idMecanico = mecanicoIdSeleccionado;

            // Rol 3 es Mecánico. Si es mecánico, se ignora el parámetro de entrada y se fuerza el de la sesión.
            if (UserSession.CurrentUser.IdRol == 3)
            {
                idMecanico = UserSession.CurrentUser.IdUsuario;
            }

            return await _reporteRepository.ObtenerServiciosPorMecanicoAsync(desde, hasta, idMecanico);
        }
    }
}
