using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IServicioRepository : IRepository<Servicio>
    {
        Task<List<Servicio>> GetActivosAsync();
        Task<Servicio?> GetByNombreAsync(string nombre);
    }
}
