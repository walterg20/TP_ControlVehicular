namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ReporteModeloReparadoDto
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadReparaciones { get; set; }
        
        public string MarcaModelo => $"{Marca} {Modelo}";
    }
}
