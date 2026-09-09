using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IRegistroServicioRepository : IRepository<RegistroServicio>
    {
        Task<List<RegistroServicio>> GetReporteCompletoAsync();
    }
}
