// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IServicioRepository : IRepository<Servicio>
    {
        Task<List<Servicio>> GetActivosAsync();
        Task<Servicio?> GetByNombreAsync(string nombre);

        /// <summary>
        /// Recupera una pagina de servicios aplicando la busqueda textual en la base de datos.
        /// </summary>
        Task<(IEnumerable<Servicio> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            bool incluirInactivos,
            int pagina,
            int tamanoPagina);
    }
}
