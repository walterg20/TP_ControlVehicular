using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarRolHandler
    {
        private readonly IRolRepository _rolRepository;
        private readonly IMapper _mapper;

        public ListarRolHandler(IRolRepository rolRepository, IMapper mapper)
        {
            _rolRepository = rolRepository;
            _mapper = mapper;
        }

        public async Task<List<RolDto>> HandleAsync()
        {
            var entities = await _rolRepository.GetActivosAsync();
            return _mapper.Map<List<RolDto>>(entities);
        }
    }
}
