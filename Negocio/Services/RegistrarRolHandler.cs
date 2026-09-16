using AutoMapper;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
namespace TP_ControlVehicular.Negocio.Services;

public class RegistrarRolHandler(IRolRepository repo, IMapper mapper)
{
    public async Task<RolDto> HandleAsync(Rol item)
    {
        if (item.IdRol > 0)
            await repo.UpdateAsync(item);
        else
            await repo.AddAsync(item);

        return mapper.Map<RolDto>(item);
    }
}
