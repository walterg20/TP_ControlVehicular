using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ModificarUsuarioHandler
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public ModificarUsuarioHandler(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<UsuarioDto> HandleAsync(Usuario usuario)
        {
            await _usuarioRepository.UpdateAsync(usuario);
            var reloaded = await _usuarioRepository.GetByNombreAsync(usuario.Nombre) ?? usuario;
            return _mapper.Map<UsuarioDto>(reloaded);
        }
    }
}
