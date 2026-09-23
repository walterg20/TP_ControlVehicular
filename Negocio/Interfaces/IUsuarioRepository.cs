using System.Collections.Generic;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<List<Usuario>> GetActivosAsync();
        Task<Usuario?> GetByNombreAsync(string nombre);
        Task<Usuario?> GetByDniAsync(string dni);
        Task<List<Usuario>> GetWithRolAsync();
    }
}
