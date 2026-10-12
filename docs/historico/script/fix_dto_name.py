import io
import os

if os.path.exists('Negocio/DTOs/Reportes/HistorialVehiculoDto.cs'):
    os.remove('Negocio/DTOs/Reportes/HistorialVehiculoDto.cs') # Remove the one I modified

dto_content = '''namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class HistorialClinicoVehiculoDto
    {
        public System.DateTime Fecha { get; set; }
        public int KmIngreso { get; set; }
        public string Servicio { get; set; } = string.Empty;
        public string Mecanico { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
'''
with io.open('Negocio/DTOs/Reportes/HistorialClinicoVehiculoDto.cs', 'w', encoding='utf-8') as f:
    f.write(dto_content)