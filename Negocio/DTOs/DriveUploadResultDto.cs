namespace TP_ControlVehicular.Negocio.DTOs
{
    public class DriveUploadResultDto
    {
        public bool Subido { get; set; }
        public string Archivo { get; set; } = string.Empty;
        public string IdDrive { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}
