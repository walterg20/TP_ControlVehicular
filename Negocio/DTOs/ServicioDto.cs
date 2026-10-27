// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    public class ServicioDto
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Activo { get; set; } = true;
        public override string ToString() => Nombre;
    }
}
