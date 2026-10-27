// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    public class BackupArchivoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string RutaCompleta { get; set; } = string.Empty;
        public long TamanoBytes { get; set; }
        public string TamanoLegible { get; set; } = string.Empty;
        public DateTime FechaModificacion { get; set; }
    }
}
