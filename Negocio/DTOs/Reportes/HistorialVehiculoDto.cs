using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class HistorialVehiculoDto
    {
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
    }
}
