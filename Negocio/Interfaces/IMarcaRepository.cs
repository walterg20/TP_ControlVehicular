// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IMarcaRepository : IRepository<Marca>
    {
        Task<List<Marca>> GetActivosAsync();
        Task<Marca?> GetByNombreAsync(string nombre);

        /// <summary>
        /// Recupera una pagina de marcas aplicando la busqueda textual y el filtro de estado
        /// (Todos / Activos / Inactivos) en la base de datos.
        /// </summary>
        Task<(IEnumerable<Marca> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina);
    }
}
