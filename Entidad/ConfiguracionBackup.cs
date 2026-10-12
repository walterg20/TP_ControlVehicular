namespace TP_ControlVehicular.Entidad
{
    public class ConfiguracionBackup
    {
        public int Id { get; set; }
        public string NombreBaseDatos { get; set; } = "ControlVehicular";
        public string RutaBackupServer { get; set; } = string.Empty;
        public string RutaBackupUNC { get; set; } = string.Empty;
        public string? DriveFolderId { get; set; }
        public string? UltimaSubidaArchivo { get; set; }
        public DateTime? FechaUltimaSubida { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
