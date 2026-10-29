// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IMarcaRepository : IRepository<Marca>
    {
        Task<Marca?> GetByNombreAsync(string nombre);

        /// <summary>
        /// Recupera una pagina de marcas aplicando la busqueda textual en la base de datos.
        /// </summary>
        Task<(IEnumerable<Marca> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina);
    }
}
