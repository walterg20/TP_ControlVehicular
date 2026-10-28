// walterg20
using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ReporteIngresoMensualDto
    {
        public string PeriodoMes { get; set; } = string.Empty;
        public DateTime FechaMes { get; set; }
        public int CantidadFacturas { get; set; }
        public decimal IngresosTotales { get; set; }
    }
}
