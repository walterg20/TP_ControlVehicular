using System;

namespace TP_ControlVehicular.Negocio.DTOs
{
    public class DriveArchivoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaModificacion { get; set; }
        public long TamanoBytes { get; set; }
        public string TamanoLegible { get; set; } = "-";
    }
}