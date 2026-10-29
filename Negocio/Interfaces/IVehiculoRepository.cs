// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IVehiculoRepository : IRepository<Vehiculo>
    {
        Task<IEnumerable<Vehiculo>> GetByClienteAsync(int idCliente);
        Task<IEnumerable<Vehiculo>> GetAllWithDetailsAsync(int? mecanicoId = null);
        Task<Vehiculo?> GetByPatenteAsync(string patente);

        /// <summary>
        /// Recupera una pagina de vehiculos (con modelo, marca y propietario actual)
        /// aplicando la busqueda textual en la base de datos.
        /// </summary>
        Task<(IEnumerable<Vehiculo> Items, int TotalRegistros)> GetPaginadoAsync(
            string? busqueda,
            int pagina,
            int tamanoPagina);
    }
}
