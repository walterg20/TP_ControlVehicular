using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Interfaces;
namespace TP_ControlVehicular.Datos.Repositories;

public class RolRepository(CVDbContext context) : Repository<Rol>(context), IRolRepository
{
    public Task<IEnumerable<Rol>> GetActivosAsync() =>
        Task.FromResult<IEnumerable<Rol>>(context.Roles.Where(x => x.Estado).ToList());
}
