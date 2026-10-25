using System;

namespace TP_ControlVehicular.Negocio.DTOs
{
    public class FacturaDto
    {
        public int Id { get; set; }
        public int RegistroServicioId { get; set; }
        public decimal Total { get; set; }
        public decimal MontoRecibido { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
