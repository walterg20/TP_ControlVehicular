namespace TP_ControlVehicular.Negocio.DTOs
{
    public class DetalleServicioDto
    {
        public int Id { get; set; }
        public int RegistroServicioId { get; set; }
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; } = string.Empty;
        
        public int UsuarioId { get; set; } // Mecánico asignado
        
        public bool Realizado { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int OrdenEjecucion { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
