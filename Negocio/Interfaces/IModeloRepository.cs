
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IModeloRepository : IRepository<Modelo>
    {
        Task<IEnumerable<Modelo>> GetByMarcaAsync(int idMarca);
    }
}
