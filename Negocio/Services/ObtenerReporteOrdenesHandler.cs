// walterg20
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Context;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ObtenerReporteOrdenesHandler
    {
        private readonly IReporteRepository _reporteRepository;

        public ObtenerReporteOrdenesHandler(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        /// <summary>
        /// Devuelve el reporte completo de ordenes (sin paginar). Lo usa la exportacion a PDF.
        /// </summary>
        public async Task<List<OrdenTrabajoReporteDto>> HandleAsync()
        {
            return await HandleCompletoAsync(null, null, null, null);
        }

        /// <summary>
        /// Devuelve el reporte completo de ordenes que cumplen los filtros, sin paginar.
        /// Lo usa la exportacion a PDF.
        /// </summary>
        public async Task<List<OrdenTrabajoReporteDto>> HandleCompletoAsync(
            string? textoBusqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            try
            {
                int? mecanicoId = null;
                var currentUser = TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser;
                if (currentUser != null && currentUser.IdRol == (int)TP_ControlVehicular.Negocio.Context.RolesSistema.Mecanico)
                {
                    mecanicoId = currentUser.IdUsuario;
                }

                // tamanoPagina = 0 devuelve el conjunto completo desde el procedimiento almacenado.
                var (items, _) = await _reporteRepository.ObtenerReporteOrdenesPaginadoAsync(
                    textoBusqueda, estado, fechaDesde, fechaHasta, mecanicoId, 1, 0);

                return items;
            }
            catch
            {
                // Sin datos demostrativos: ante un error el reporte se muestra vacío.
                return new List<OrdenTrabajoReporteDto>();
            }
        }

        /// <summary>
        /// Devuelve una pagina del reporte de ordenes con los filtros aplicados en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<OrdenTrabajoReporteDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            string? textoBusqueda,
            string? estado,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pagina,
            int tamanoPagina)
        {
            int? mecanicoId = null;
            var currentUser = TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser;
            if (currentUser != null && currentUser.IdRol == (int)TP_ControlVehicular.Negocio.Context.RolesSistema.Mecanico)
            {
                mecanicoId = currentUser.IdUsuario;
            }

            var (items, total) = await _reporteRepository.ObtenerReporteOrdenesPaginadoAsync(
                textoBusqueda, estado, fechaDesde, fechaHasta, mecanicoId, pagina, tamanoPagina);

            return (items, total);
        }
    }
}
