using System;
using System.IO;
using System.Text.Json;

namespace TP_ControlVehicular.Negocio.Reportes.Documentos
{
    /// <summary>
    /// Punto unico de resolucion del branding de la empresa (logo + datos).
    /// Fuente efectiva para logo: la carpeta de override por usuario; si no existe, el logo
    /// empaquetado con la aplicacion. Datos de empresa (Nombre/Direccion/Telefono) se
    /// cachean en %LocalAppData%\TP_ControlVehicular\Empresa\empresa.json y pueden
    /// aplicarse desde la configuracion de empresa.
    /// </summary>
    public static class EmpresaBranding
    {
        private static readonly object _lock = new object();
        private static string _nombre = "TALLER PRO";
        private static string _direccion = "Ruta Nacional 11 Km 1005 - Resistencia, Chaco";
        private static string _telefono = "+54 362 4123456";
        private static string _email = "contacto@taller.com";

        /// <summary>Carpeta de override por usuario: %LocalAppData%\TP_ControlVehicular\Empresa.</summary>
        public static string CarpetaEmpresa => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TP_ControlVehicular", "Empresa");

        /// <summary>Logo subido por el taller (override).</summary>
        public static string RutaLogoOverride => Path.Combine(CarpetaEmpresa, "logo.png");

        /// <summary>Logo empaquetado con la aplicacion (copiado al output como recurso).</summary>
        public static string RutaLogoCompartido => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Presentacion", "Assets", "logo.png");

        /// <summary>Ruta efectiva del logo: override si existe, sino el compartido.</summary>
        public static string RutaLogo => File.Exists(RutaLogoOverride) ? RutaLogoOverride : RutaLogoCompartido;

        public static string Nombre
        {
            get
            {
                lock (_lock)
                {
                    return _nombre;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _nombre = value;
                }
            }
        }

        public static string Direccion
        {
            get
            {
                lock (_lock)
                {
                    return _direccion;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _direccion = value;
                }
            }
        }

        public static string Telefono
        {
            get
            {
                lock (_lock)
                {
                    return _telefono;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _telefono = value;
                }
            }
        }

        public static string Email
        {
            get
            {
                lock (_lock)
                {
                    return _email;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _email = value;
                }
            }
        }

        /// <summary>Se asegura de que la carpeta de override exista.</summary>
        public static void AsegurarCarpeta()
        {
            try
            {
                if (!Directory.Exists(CarpetaEmpresa))
                    Directory.CreateDirectory(CarpetaEmpresa);
            }
            catch
            {
                // No bloquear la ejecucion de reportes si no se puede crear la carpeta
            }
        }

        public static void Aplicar(string nombre, string direccion, string telefono, byte[]? logo)
        {
            AsegurarCarpeta();

            lock (_lock)
            {
                Nombre = string.IsNullOrWhiteSpace(nombre) ? "TALLER PRO" : nombre.Trim();
                Direccion = string.IsNullOrWhiteSpace(direccion) ? "Ruta Nacional 11 Km 1005 - Resistencia, Chaco" : direccion.Trim();
                Telefono = string.IsNullOrWhiteSpace(telefono) ? "+54 362 4123456" : telefono.Trim();
                Email = "contacto@taller.com";
            }

            try
            {
                var rutaJson = Path.Combine(CarpetaEmpresa, "empresa.json");
                var meta = new
                {
                    Nombre = Nombre,
                    Direccion = Direccion,
                    Telefono = Telefono,
                    Email = Email,
                    FechaActualizacion = DateTime.Now
                };
                File.WriteAllText(rutaJson, JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch
            {
                // No bloquear si no se puede escribir metadata
            }

            try
            {
                if (logo != null && logo.Length > 0)
                {
                    File.WriteAllBytes(RutaLogoOverride, logo);
                }
            }
            catch
            {
                // No bloquear si no se puede escribir logo
            }

            CargarCache();
        }

        public static void CargarCache()
        {
            try
            {
                var rutaJson = Path.Combine(CarpetaEmpresa, "empresa.json");
                if (File.Exists(rutaJson))
                {
                    var json = File.ReadAllText(rutaJson);
                    using (var doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        lock (_lock)
                        {
                            if (root.TryGetProperty("Nombre", out var n) && !string.IsNullOrWhiteSpace(n.GetString()))
                                _nombre = n.GetString()!.Trim();
                            if (root.TryGetProperty("Direccion", out var d) && !string.IsNullOrWhiteSpace(d.GetString()))
                                _direccion = d.GetString()!.Trim();
                            if (root.TryGetProperty("Telefono", out var t) && !string.IsNullOrWhiteSpace(t.GetString()))
                                _telefono = t.GetString()!.Trim();
                            if (root.TryGetProperty("Email", out var em) && !string.IsNullOrWhiteSpace(em.GetString()))
                                _email = em.GetString()!.Trim();
                        }
                    }
                }
            }
            catch
            {
                // Mantener valores por defecto si falla la carga
            }
        }

        static EmpresaBranding()
        {
            CargarCache();
        }
    }
}