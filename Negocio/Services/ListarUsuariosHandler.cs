using AutoMapper;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ListarUsuariosHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public ListarUsuariosHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<List<UsuarioDto>> HandleAsync()
        {
            var entities = await _usuarioRepository.GetActivosAsync();
            return _mapper.Map<List<UsuarioDto>>(entities);
        }
    }
}
