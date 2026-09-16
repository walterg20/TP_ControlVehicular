using System.Collections.ObjectModel;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using RolEntidad = TP_ControlVehicular.Entidad.Rol;
namespace TP_ControlVehicular.Presentacion.ViewModels;

public class RolViewModel(RegistrarRolHandler handler, IRolRepository repository, AutoMapper.IMapper mapper) : BaseViewModel
{
    public ObservableCollection<RolDto> Roles { get; } = new();
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;

    public async Task LoadAsync()
    {
        Roles.Clear();
        foreach (var rol in await repository.GetActivosAsync())
            Roles.Add(mapper.Map<RolDto>(rol));
    }

    public async Task GuardarAsync()
    {
        var rol = new RolEntidad { IdRol = IdRol, Nombre = Nombre, Descripcion = Descripcion, Estado = Estado };
        var dto = await handler.HandleAsync(rol);
        var existente = Roles.FirstOrDefault(x => x.IdRol == dto.IdRol);

        if (existente is null)
            Roles.Add(dto);
        else
            Roles[Roles.IndexOf(existente)] = dto;
    }
}
