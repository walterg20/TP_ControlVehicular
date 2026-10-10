using System;
using System.IO;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    /// <summary>
    /// Punto unico de resolucion del branding de la empresa (logo).
    /// Fuente efectiva: la carpeta de override por usuario; si no existe, el logo
    /// empaquetado con la aplicacion. La carpeta la crea la app de forma perezosa
    /// (<see cref="AsegurarCarpeta"/>); el instalador no la crea.
    /// </summary>
    public static class EmpresaBranding
    {
        /// <summary>Carpeta de override por usuario: %LocalAppData%\TP_ControlVehicular\Empresa.</summary>
        public static string CarpetaEmpresa => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TP_ControlVehicular", "Empresa");

        /// <summary>Logo subido por el taller (override).</summary>
        public static string RutaLogoOverride => Path.Combine(CarpetaEmpresa, "logo.png");

        /// <summary>Logo empaquetado con la aplicacion (copiado al output como Content).</summary>
        public static string RutaLogoCompartido => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Presentacion", "Assets", "logo.png");

        /// <summary>Logo efectivo: override si existe; si no, el empaquetado.</summary>
        public static string RutaLogo =>
            File.Exists(RutaLogoOverride) ? RutaLogoOverride : RutaLogoCompartido;

        /// <summary>Crea la carpeta de override de forma perezosa.</summary>
        public static void AsegurarCarpeta() => Directory.CreateDirectory(CarpetaEmpresa);
    }
}
