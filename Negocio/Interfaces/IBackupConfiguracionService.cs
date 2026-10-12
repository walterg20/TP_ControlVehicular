using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IBackupConfiguracionService
    {
        Task<BackupConfiguracionDto> ObtenerAsync();
        Task<BackupConfiguracionDto> GuardarAsync(BackupConfiguracionDto dto);
        Task RegistrarSubidaAsync(string nombreArchivo, DateTime fecha);
    }
}
