using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarRolHandler
    {
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;

        public RegistrarRolHandler(IRolRepository rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        public async Task<RolDto> HandleAsync(Rol rol)
        {
            await _rolRepository.AddAsync(rol);
            return _mapper.Map<RolDto>(rol);
        }
    }
}
