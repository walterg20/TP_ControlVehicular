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

        /// <summary>
        /// Lista los archivos de respaldo (.bak) presentes en la carpeta de Drive configurada.
        /// </summary>
        Task<IReadOnlyList<DriveArchivoDto>> ListarArchivosAsync();

        /// <summary>
        /// Descarga un archivo de Drive a la carpeta efectiva de backup local.
        /// Devuelve la ruta local completa del archivo descargado.
        /// </summary>
        Task<string> DescargarAsync(string fileId, string nombreArchivo);
    }
}
