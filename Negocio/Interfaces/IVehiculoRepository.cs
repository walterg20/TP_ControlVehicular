using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IVehiculoRepository : IRepository<Vehiculo>
    {
        Task<IEnumerable<Vehiculo>> GetByClienteAsync(int idCliente);
    }
}
