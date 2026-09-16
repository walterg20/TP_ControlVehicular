using System.Collections.ObjectModel;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using UsuarioEntidad = TP_ControlVehicular.Entidad.Usuario;
namespace TP_ControlVehicular.Presentacion.ViewModels;

public class UsuarioViewModel(RegistrarUsuarioHandler handler, IUsuarioRepository usuarioRepository, IRolRepository rolRepository, AutoMapper.IMapper mapper) : BaseViewModel
{
    public ObservableCollection<UsuarioDto> Usuarios { get; } = new();
    public ObservableCollection<RolDto> Roles { get; } = new();
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;

    public async Task LoadAsync()
    {
        Usuarios.Clear();
        Roles.Clear();

        foreach (var usuario in await usuarioRepository.GetActivosAsync())
            Usuarios.Add(mapper.Map<UsuarioDto>(usuario));

        foreach (var rol in await rolRepository.GetActivosAsync())
            Roles.Add(mapper.Map<RolDto>(rol));
    }

    public async Task GuardarAsync()
    {
        var usuario = new UsuarioEntidad
        {
            IdUsuario = IdUsuario,
            IdRol = IdRol,
            Nombre = Nombre,
            Contrasena = Contrasena,
            Estado = Estado
        };
        var dto = await handler.HandleAsync(usuario);
        var existente = Usuarios.FirstOrDefault(x => x.IdUsuario == dto.IdUsuario);

        if (existente is null)
            Usuarios.Add(dto);
        else
            Usuarios[Usuarios.IndexOf(existente)] = dto;
    }
}
