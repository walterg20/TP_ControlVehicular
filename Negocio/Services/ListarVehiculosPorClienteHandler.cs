using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarVehiculosPorClienteHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly IMapper _mapper;

        public ListarVehiculosPorClienteHandler(IVehiculoRepository vehiculoRepository, IMapper mapper)
        {
            _vehiculoRepository = vehiculoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VehiculoDto>> HandleAsync(int idCliente)
        {
            var vehiculos = await _vehiculoRepository.GetByClienteAsync(idCliente);
            return _mapper.Map<IEnumerable<VehiculoDto>>(vehiculos);
        }
    }
}
