import io
import os

os.makedirs('Negocio/DTOs/Reportes', exist_ok=True)

historial_dto = '''using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class HistorialVehiculoDto
    {
        public DateTime Fecha { get; set; }
        public int KmIngreso { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public string Mecanico { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
'''
with io.open('Negocio/DTOs/Reportes/HistorialVehiculoDto.cs', 'w', encoding='utf-8') as f:
    f.write(historial_dto)

hoja_dto = '''namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class HojaTrabajoDiariaDto
    {
        public string Patente { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public int OrdenEjecucion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
'''
with io.open('Negocio/DTOs/Reportes/HojaTrabajoDiariaDto.cs', 'w', encoding='utf-8') as f:
    f.write(hoja_dto)
