using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarVehiculoHandler
    {
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly IMapper _mapper;

        public ModificarVehiculoHandler(IVehiculoRepository vehiculoRepository, IMapper mapper)
        {
            _vehiculoRepository = vehiculoRepository;
            _mapper = mapper;
        }

        public async Task<VehiculoDto> HandleAsync(Vehiculo entity)
        {
            await _vehiculoRepository.UpdateAsync(entity);
            var reloaded = await _vehiculoRepository.GetByIdAsync(entity.Id);
            return _mapper.Map<VehiculoDto>(reloaded ?? entity);
        }
    }
}
