// walterg20
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IDatabaseBackupService
    {
        Task<BackupResultDto> HacerBackupAsync();
        Task<BackupTestResultDto> ProbarBackupAsync(string archivoBak);
        Task<IReadOnlyList<BackupArchivoDto>> ListarAsync();
    }
}
