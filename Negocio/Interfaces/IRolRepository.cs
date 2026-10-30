// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IRolRepository : IRepository<Rol>
    {
        Task<List<Rol>> GetActivosAsync();

        /// <summary>
        /// Recupera una pagina de roles aplicando la busqueda textual en la base de datos.
        /// </summary>
        Task<(IEnumerable<Rol> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina);
    }
}
