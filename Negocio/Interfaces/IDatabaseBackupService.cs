// walterg20
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IDatabaseBackupService
    {
        Task<BackupResultDto> HacerBackupAsync();
        Task<BackupTestResultDto> ProbarBackupAsync(string archivoBak);
        Task<IReadOnlyList<BackupArchivoDto>> ListarAsync();

        /// <summary>
        /// Restaura la base de datos productiva a partir de un respaldo (.bak).
        /// Requiere autorización de un segundo usuario (doble factor) cuyas
        /// credenciales se validan antes de ejecutar la restauración.
        /// </summary>
        Task<BackupResultDto> RestaurarAsync(string archivoBak, string dniAutorizador, string contrasenaAutorizador);
    }
}
