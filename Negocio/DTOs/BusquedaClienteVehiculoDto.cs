// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    /// <summary>
    /// Fila de resultado del buscador universal de la orden de servicio.
    /// Representa el par (cliente + vehiculo) junto con la informacion necesaria
    /// para mostrarlo de forma inequivoca en el desplegable.
    /// </summary>
    public class BusquedaClienteVehiculoDto
    {
        /// <summary>Identificador del cliente propietario del vehiculo.</summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Identificador del vehiculo. Es 0 cuando la fila representa a un cliente
        /// con varios vehiculos (fila agrupada) o la fila de accion "registrar nuevo".
        /// </summary>
        public int VehiculoId { get; set; }

        /// <summary>Patente del vehiculo.</summary>
        public string Patente { get; set; } = string.Empty;

        /// <summary>Apellido del cliente.</summary>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>Nombre del cliente.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>DNI del cliente.</summary>
        public string Dni { get; set; } = string.Empty;

        /// <summary>Descripcion del vehiculo (marca y modelo).</summary>
        public string VehiculoDescripcion { get; set; } = string.Empty;

        /// <summary>
        /// Indica que la fila agrupa a un cliente que posee dos o mas vehiculos.
        /// En ese caso el operador deberia elegir el vehiculo en el desplegable de vehiculos.
        /// </summary>
        public bool EsAgrupado { get; set; }

        /// <summary>Cantidad de vehiculos que posee el cliente cuando la fila es agrupada.</summary>
        public int CantidadVehiculos { get; set; }

        /// <summary>
        /// Indica que la fila es la accion que se ofrece cuando la busqueda no arroja
        /// resultados: "No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo".
        /// </summary>
        public bool EsAccionRegistrar { get; set; }

        /// <summary>
        /// Texto completo de la fila, tal como se muestra en el desplegable:
        /// <c>[Patente] - Apellido, Nombre (DNI) - Vehiculo</c>.
        /// </summary>
        public string DisplayText { get; set; } = string.Empty;

        /// <summary>
        /// Texto resumido que queda en la caja del combo al elegir la fila (el nombre
        /// del dueno), para que el operador vea de un vistazo a quien pertenece el auto.
        /// </summary>
        public string TextMostrar { get; set; } = string.Empty;

        public override string ToString() => TextMostrar;
    }
}
