// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IModeloRepository : IRepository<Modelo>
    {
        Task<IEnumerable<Modelo>> GetByMarcaAsync(int idMarca);

        /// <summary>
        /// Recupera una pagina de modelos (con su marca) aplicando la busqueda textual
        /// en la base de datos.
        /// </summary>
        Task<(IEnumerable<Modelo> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina);
    }
}
