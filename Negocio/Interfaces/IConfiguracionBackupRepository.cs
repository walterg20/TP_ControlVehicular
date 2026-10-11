using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IConfiguracionBackupRepository : IRepository<ConfiguracionBackup>
    {
        Task<ConfiguracionBackup?> ObtenerAsync();
    }
}
