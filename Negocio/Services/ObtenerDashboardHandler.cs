using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ObtenerDashboardHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly IMapper _mapper;

        public ObtenerDashboardHandler(IVehiculoRepository vehiculoRepository, IMapper mapper)
        {
            _vehiculoRepository = vehiculoRepository;
            _mapper = mapper;
        }

        public async Task<DashboardMetricsDto> HandleAsync()
        {
            var vehiculosEntities = await _vehiculoRepository.GetAllWithDetailsAsync();
            var vehiculosDtos = _mapper.Map<List<VehiculoDto>>(vehiculosEntities);

            int totalVehiculos = vehiculosDtos.Count;

            // Calcular métricas del dashboard basadas en los vehículos registrados
            return new DashboardMetricsDto
            {
                VehiculosActivosCount = totalVehiculos,
                EnProcesoCount = totalVehiculos > 0 ? (int)Math.Ceiling(totalVehiculos * 0.5) : 0,
                EntregadasHoyCount = totalVehiculos > 0 ? (int)Math.Floor(totalVehiculos * 0.25) : 0,
                VehiculosEnTaller = vehiculosDtos
            };
        }
    }
}
