using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarRolHandler
    {
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;

        public ModificarRolHandler(IRolRepository rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        public async Task<RolDto> HandleAsync(Rol rol)
        {
            await _rolRepository.UpdateAsync(rol);
            return _mapper.Map<RolDto>(rol);
        }
    }
}
