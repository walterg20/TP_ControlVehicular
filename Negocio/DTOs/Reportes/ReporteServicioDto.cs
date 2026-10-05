using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ReporteServicioDto
    {
        public DateTime Fecha { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
    }
}
