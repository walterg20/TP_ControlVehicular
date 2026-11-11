// walterg20
using System;
using System.Collections.Generic;
using System.IO;

namespace TP_ControlVehicular.Presentacion.Helpers
{
    /// <summary>
    /// Carga opcional de un archivo .env (KEY=VALUE) ubicado junto al ejecutable
    /// o en la raiz del proyecto, e inyecta sus valores en las variables de
    /// entorno del proceso ANTES de construir la configuracion de la aplicacion.
    /// Permite que cada desarrollador configure su conexion a SQL Server sin
    /// editar appsettings.json (que esta ignorado por git).
    /// </summary>
    public static class EnvLoader
    {
        /// <summary>
        /// Busca y carga un archivo .env en las rutas candidatas. Si no existe,
        /// no hace nada (el comportamiento por defecto de appsettings.json aplica).
        /// </summary>
        public static void CargarSiExiste()
        {
            var candidatos = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, ".env"),
            };

            // Raiz del proyecto (util en desarrollo: el .env del repo)
            try
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (dir != null)
                {
                    var candidate = Path.Combine(dir.FullName, ".env");
                    if (File.Exists(candidate))
                    {
                        candidatos.Insert(0, candidate);
                        break;
                    }
                    dir = dir.Parent;
                }
            }
            catch
            {
                // Ignorar: la carga de .env es best-effort.
            }

            foreach (var ruta in candidatos)
            {
                if (!File.Exists(ruta)) continue;
                CargarArchivo(ruta);
                // Una vez cargado el primero que existe, no seguir buscando.
                break;
            }
        }

        private static void CargarArchivo(string ruta)
        {
            foreach (var lineaRaw in File.ReadAllLines(ruta))
            {
                var linea = lineaRaw.Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;
                if (linea.StartsWith("#")) continue;

                var idx = linea.IndexOf('=');
                if (idx <= 0) continue;

                var clave = linea.Substring(0, idx).Trim();
                var valor = linea.Substring(idx + 1).Trim();

                // Quitar comillas envolventes opcionales.
                if (valor.Length >= 2 &&
                    ((valor.StartsWith("\"") && valor.EndsWith("\"")) ||
                     (valor.StartsWith("'") && valor.EndsWith("'"))))
                {
                    valor = valor.Substring(1, valor.Length - 2);
                }

                if (string.IsNullOrWhiteSpace(clave)) continue;

                // No sobrescribir variables ya definidas por el sistema/CI.
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(clave)))
                    Environment.SetEnvironmentVariable(clave, valor);
            }
        }
    }
}