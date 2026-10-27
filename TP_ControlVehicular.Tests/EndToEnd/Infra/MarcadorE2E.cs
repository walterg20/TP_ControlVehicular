namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    /// <summary>
    /// Centraliza las fórmulas de marcado que usan <see cref="SemillaE2E"/> (siembra)
    /// y <see cref="LimpiezaE2E"/> (limpieza) para que no se desincronicen. Como ninguna
    /// entidad posee un campo libre dedicado, el marcador se deposita en un campo real
    /// de cada tabla y se calcula aquí de forma determinista.
    /// </summary>
    internal static class MarcadorE2E
    {
        /// <summary>Token corto (6 dígitos hexadecimales) derivado del marcador de la corrida.</summary>
        public static string Token(string marker) => (marker.GetHashCode() & 0xFFFFFF).ToString("X6");

        public static string NombreCliente(string marker) => $"Cliente {marker}";

        /// <summary>Cliente.Dni admite 8 caracteres como máximo.</summary>
        public static string DniCliente(string marker) => "CLI" + Token(marker).Substring(0, 5);

        /// <summary>Vehiculo.Patente admite 15 caracteres como máximo.</summary>
        public static string PatenteVehiculo(string marker) => "E2E" + Token(marker);

        public static string NombreTaller(string marker) => $"Taller {marker}";

        public static string ApellidoUsuario(string marker) => marker;

        /// <summary>Usuario.Dni admite 15 caracteres como máximo.</summary>
        public static string DniUsuario(string marker) => "USR" + Token(marker);

        public static string EmailUsuario(string marker) => $"usr{Token(marker)}@e2e.local";

        public static string ObservacionesDetalle(string marker) => marker;
    }
}
