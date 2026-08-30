using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>  
    {
        Task<IEnumerable<Cliente>> GetActivosAsync();
    }
}
