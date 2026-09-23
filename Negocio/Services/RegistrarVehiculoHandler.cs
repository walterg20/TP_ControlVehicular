using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;


namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarVehiculoHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly IMapper _mapper;

        public RegistrarVehiculoHandler(IVehiculoRepository vehiculoRepository, IMapper mapper)
        {
            _vehiculoRepository = vehiculoRepository;
            _mapper = mapper;
        }

        public async Task<VehiculoDto> HandleAsync(Vehiculo vehiculo)
        {
            await _vehiculoRepository.AddAsync(vehiculo);
            return _mapper.Map<VehiculoDto>(vehiculo);
        }
    }
}
