// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<IEnumerable<Cliente>> GetActivosAsync();

        /// <summary>
        /// Recupera una pagina de clientes aplicando la busqueda textual en la base de datos.
        /// </summary>
        /// <param name="busqueda">Termino a buscar sobre Nombre, Apellido y Dni. Vacio o null devuelve todos.</param>
        /// <param name="incluirInactivos">Si es true, incluye tambien los clientes dados de baja.</param>
        /// <param name="pagina">Pagina solicitada (base 1).</param>
        /// <param name="tamanoPagina">Cantidad de registros por pagina.</param>
        Task<(IEnumerable<Cliente> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina);
    }
}
