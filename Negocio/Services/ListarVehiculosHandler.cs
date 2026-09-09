using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarVehiculosHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly IMapper _mapper;

        public ListarVehiculosHandler(IVehiculoRepository vehiculoRepository, IMapper mapper)
        {
            _vehiculoRepository = vehiculoRepository;
            _mapper = mapper;
        }

        public async Task<List<VehiculoDto>> HandleAsync()
        {
            var entities = await _vehiculoRepository.GetAllAsync();
            return _mapper.Map<List<VehiculoDto>>(entities);
        }
    }
}
