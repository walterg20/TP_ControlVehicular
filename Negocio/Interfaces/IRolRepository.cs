using System.Collections.Generic;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IRolRepository : IRepository<Rol>
    {
        Task<List<Rol>> GetActivosAsync();
    }
}
