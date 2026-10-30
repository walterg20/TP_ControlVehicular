// walterg20
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Handlers.Reportes
{
    public class ObtenerHistorialVehiculoHandler
    {
        private readonly IReporteRepository _repository;

        public ObtenerHistorialVehiculoHandler(IReporteRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HistorialVehiculoDto>> HandleAsync(int? vehiculoId, string? patente)
        {
            if (vehiculoId == null && string.IsNullOrWhiteSpace(patente))
            {
                throw new ArgumentException("Debe proporcionar un ID de vehículo o una patente.");
            }

            return await _repository.ObtenerHistorialVehiculoAsync(vehiculoId, patente);
        }

        /// <summary>
        /// Recupera una pagina del historial del vehiculo con la paginacion en la base de datos.
        /// </summary>
        public async Task<(IEnumerable<HistorialVehiculoDto> Items, int TotalRegistros)> HandlePaginadoAsync(
            int? vehiculoId, string? patente, int pagina, int tamanoPagina)
        {
            if (vehiculoId == null && string.IsNullOrWhiteSpace(patente))
            {
                throw new ArgumentException("Debe proporcionar un ID de vehículo o una patente.");
            }

            return await _repository.ObtenerHistorialVehiculoPaginadoAsync(vehiculoId, patente, pagina, tamanoPagina);
        }
    }
}
