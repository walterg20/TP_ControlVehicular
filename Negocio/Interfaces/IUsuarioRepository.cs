// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<List<Usuario>> GetActivosAsync();
        Task<Usuario?> GetByNombreAsync(string nombre);
        Task<Usuario?> GetByDniAsync(string dni);
        Task<List<Usuario>> GetWithRolAsync();

        /// <summary>
        /// Recupera una pagina de usuarios (con su rol) aplicando la busqueda textual
        /// en la base de datos.
        /// </summary>
        Task<(IEnumerable<Usuario> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina);
    }
}
