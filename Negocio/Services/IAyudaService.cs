// walterg20
using System.Collections.Generic;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// Servicio para gestionar ayudas contextuales filtradas por rol y contexto.
    /// </summary>
    public interface IAyudaService
    {
        /// <summary>
        /// Obtiene la ayuda para un control específico en una pantalla, según el rol del usuario.
        /// </summary>
        /// <param name="idRol">Identificador del rol del usuario.</param>
        /// <param name="nombrePantalla">Nombre de la pantalla (ej: "OrdenServicio").</param>
        /// <param name="nombreControl">Nombre del control (ej: "TxtCliente").</param>
        /// <returns>Ayuda contextual con sus secciones, o null si no existe.</returns>
        Task<AyudaContextoDto?> ObtenerAyudaAsync(int idRol, string nombrePantalla, string nombreControl);

        /// <summary>
        /// Obtiene todas las ayudas disponibles para una pantalla según el rol del usuario.
        /// </summary>
        /// <param name="idRol">Identificador del rol del usuario.</param>
        /// <param name="nombrePantalla">Nombre de la pantalla.</param>
        /// <returns>Colección de ayudas contextuales para esa pantalla.</returns>
        Task<IEnumerable<AyudaContextoDto>> ObtenerAyudasPantallaAsync(int idRol, string nombrePantalla);

        /// <summary>
        /// Guarda el estado de expansión de una sección de ayuda para persistir preferencias del usuario.
        /// </summary>
        /// <param name="idRol">Identificador del rol.</param>
        /// <param name="idAyuda">Identificador de la ayuda.</param>
        /// <param name="idSeccion">Identificador de la sección.</param>
        /// <param name="expandida">True si está expandida, false si colapsada.</param>
        /// <returns>True si se guardó exitosamente.</returns>
        Task<bool> GuardarEstadoExpandidoAsync(int idRol, int idAyuda, int idSeccion, bool expandida);
    }
}
