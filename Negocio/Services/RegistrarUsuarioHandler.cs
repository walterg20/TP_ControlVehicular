using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class RegistrarUsuarioHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public RegistrarUsuarioHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<UsuarioDto> HandleAsync(Usuario usuario)
        {
            await _usuarioRepository.AddAsync(usuario);
            var reloaded = await _usuarioRepository.GetByNombreAsync(usuario.Nombre) ?? usuario;
            return _mapper.Map<UsuarioDto>(reloaded);
        }
    }
}
