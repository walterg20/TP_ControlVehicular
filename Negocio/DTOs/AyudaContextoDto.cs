// walterg20
using System.Collections.Generic;

namespace TP_ControlVehicular.Negocio.DTOs
{
    /// <summary>
    /// Ayuda contextual asociada a una pantalla y un control específico.
    /// </summary>
    public class AyudaContextoDto
    {
        /// <summary>
        /// Identificador único de la ayuda.
        /// </summary>
        public int IdAyuda { get; set; }

        /// <summary>
        /// Nombre de la pantalla (ej: "OrdenServicio").
        /// </summary>
        public string NombrePantalla { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del control (ej: "TxtCliente", "CmbServicio").
        /// </summary>
        public string NombreControl { get; set; } = string.Empty;

        /// <summary>
        /// Secciones de ayuda para este contexto.
        /// </summary>
        public IEnumerable<AyudaSeccionDto> Secciones { get; set; } = new List<AyudaSeccionDto>();
    }
}
