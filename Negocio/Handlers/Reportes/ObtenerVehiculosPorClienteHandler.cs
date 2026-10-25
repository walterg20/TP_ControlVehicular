using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Handlers.Reportes
{
    public class ObtenerVehiculosPorClienteHandler
    {
        private readonly IReporteRepository _repository;

        public ObtenerVehiculosPorClienteHandler(IReporteRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<VehiculoClienteDto>> HandleAsync(int? clienteId, string? dni)
        {
            if (clienteId == null && string.IsNullOrWhiteSpace(dni))
            {
                throw new ArgumentException("Debe proporcionar un ID de cliente o un DNI.");
            }

            return await _repository.ObtenerVehiculosPorClienteAsync(clienteId, dni);
        }
    }
}
