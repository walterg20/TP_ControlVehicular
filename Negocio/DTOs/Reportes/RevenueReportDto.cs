using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ReporteIngresosDto
    {
        public DateTime Fecha { get; set; }
        public string TallerNombre { get; set; } = string.Empty;
        public decimal TotalIngresos { get; set; }
    }
}
