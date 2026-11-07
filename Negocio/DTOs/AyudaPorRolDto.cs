// walterg20
using System.Collections.Generic;

namespace TP_ControlVehicular.Negocio.DTOs
{
    /// <summary>
    /// Mapeo de ayudas disponibles para un rol específico.
    /// </summary>
    public class AyudaPorRolDto
    {
        /// <summary>
        /// Identificador del rol.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre del rol (ej: "Administrador", "Mecánico").
        /// </summary>
        public string NombreRol { get; set; } = string.Empty;

        /// <summary>
        /// Lista de ayudas disponibles para este rol.
        /// </summary>
        public List<AyudaContextoDto> Ayudas { get; set; } = new();
    }
}
