using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ReporteIngresoDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadFacturas { get; set; }
        public decimal IngresosTotales { get; set; }
    }
}
