using System.Collections.Generic;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface ITallerRepository : IRepository<Taller>
    {
        public Task<List<Taller>> GetActivosAsync();
    }
}
