// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    /// <summary>
    /// Sección de ayuda dentro de un contexto de ayuda contextual.
    /// </summary>
    public class AyudaSeccionDto
    {
        /// <summary>
        /// Identificador único de la sección.
        /// </summary>
        public int IdSeccion { get; set; }

        /// <summary>
        /// Título de la sección (ej: "¿Cómo ingresar un cliente?").
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Contenido de la sección (puede incluir explicaciones, pasos, etc.).
        /// </summary>
        public string Contenido { get; set; } = string.Empty;

        /// <summary>
        /// Orden de presentación de la sección.
        /// </summary>
        public int Orden { get; set; }

        /// <summary>
        /// Indica si la sección está expandida inicialmente.
        /// </summary>
        public bool Expandida { get; set; }
    }
}
