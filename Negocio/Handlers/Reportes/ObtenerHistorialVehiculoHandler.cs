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
    }
}
