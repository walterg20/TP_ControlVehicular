// walterg20
using System.Windows;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Interfaces
{
    public interface IDriveBackupService
    {
        bool TieneCredencialesGuardadas();
        Task<bool> AutenticarAsync(Window owner);
        Task<DriveUploadResultDto> SubirUltimoAsync();
    }
}
