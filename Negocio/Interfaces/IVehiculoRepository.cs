// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IVehiculoRepository : IRepository<Vehiculo>
    {
        Task<List<Vehiculo>> GetActivosAsync();
        Task<IEnumerable<Vehiculo>> GetByClienteAsync(int idCliente);
        Task<IEnumerable<Vehiculo>> GetAllWithDetailsAsync(int? mecanicoId = null);
        Task<Vehiculo?> GetByPatenteAsync(string patente);

        /// <summary>
        /// Recupera una pagina de vehiculos (con modelo, marca y propietario actual)
        /// aplicando la busqueda textual y el filtro de estado (Todos / Activos / Inactivos)
        /// en la base de datos.
        /// </summary>
        Task<(IEnumerable<Vehiculo> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            string? filtroEstado,
            int pagina,
            int tamanoPagina);
    }
}
