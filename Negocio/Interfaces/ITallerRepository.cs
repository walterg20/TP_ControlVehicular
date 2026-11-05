// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface ITallerRepository : IRepository<Taller>
    {
        public Task<List<Taller>> GetActivosAsync();

        /// <summary>
        /// Recupera una pagina de talleres aplicando la busqueda textual y el filtro de estado (Todos / Activos / Inactivos) en la base de datos.
        /// </summary>
        Task<(IEnumerable<Taller> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina);
    }
}
