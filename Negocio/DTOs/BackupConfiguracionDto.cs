namespace TP_ControlVehicular.Negocio.DTOs
{
    public class BackupConfiguracionDto
    {
        public string NombreBaseDatos { get; set; } = string.Empty;
        public string RutaBackupServer { get; set; } = string.Empty;
        public string RutaBackupUNC { get; set; } = string.Empty;
        public string? DriveFolderId { get; set; }
        public string? UltimaSubidaArchivo { get; set; }
        public DateTime? FechaUltimaSubida { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
