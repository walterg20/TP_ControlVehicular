using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class VehiculoClienteDto
    {
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public int VehiculoId { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadServicios { get; set; }
    }
}
