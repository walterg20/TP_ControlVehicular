// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    public class BackupResultDto
    {
        public bool Exitoso { get; set; }
        public string Archivo { get; set; } = string.Empty;
        public string Ruta { get; set; } = string.Empty;
        public long TamanoBytes { get; set; }
        public DateTime FechaHora { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
