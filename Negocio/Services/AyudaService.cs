// walterg20
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// Implementación del servicio de ayudas contextuales.
    /// Carga datos desde JSON y persiste preferencias en AppData.
    /// </summary>
    public class AyudaService : IAyudaService
    {
        private readonly string _rutaJson;
        private readonly string _rutaPreferencias;
        private Dictionary<int, AyudaPorRolDto>? _cache;

        public AyudaService(string? rutaJsonPersonalizada = null)
        {
            // Ruta por defecto: carpeta Datos/Configuracion en el directorio de ejecución
            _rutaJson = rutaJsonPersonalizada ?? 
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    "Datos", "Configuracion", "ayudas_ordenes_servicio.json");

            // Carpeta de preferencias en AppData del usuario
            _rutaPreferencias = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TP_ControlVehicular", "Ayudas");
        }

        /// <summary>
        /// Obtiene la ayuda para un control específico en una pantalla, según el rol del usuario.
        /// </summary>
        public async Task<AyudaContextoDto?> ObtenerAyudaAsync(int idRol, string nombrePantalla, string nombreControl)
        {
            try
            {
                var datos = await CargarDatosAsync();

                if (!datos.ContainsKey(idRol))
                    return null;

                var ayudaPorRol = datos[idRol];
                var ayuda = ayudaPorRol.Ayudas.FirstOrDefault(a =>
                    a.NombrePantalla == nombrePantalla &&
                    a.NombreControl == nombreControl);

                if (ayuda != null)
                {
                    // Cargar estado guardado de secciones expandidas
                    await AplicarPreferenciasAsync(idRol, ayuda.IdAyuda, ayuda.Secciones);
                }

                return ayuda;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerAyudaAsync: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene todas las ayudas disponibles para una pantalla según el rol del usuario.
        /// </summary>
        public async Task<IEnumerable<AyudaContextoDto>> ObtenerAyudasPantallaAsync(int idRol, string nombrePantalla)
        {
            try
            {
                var datos = await CargarDatosAsync();

                if (!datos.ContainsKey(idRol))
                    return Enumerable.Empty<AyudaContextoDto>();

                var ayudaPorRol = datos[idRol];
                return ayudaPorRol.Ayudas
                    .Where(a => a.NombrePantalla == nombrePantalla)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerAyudasPantallaAsync: {ex.Message}");
                return Enumerable.Empty<AyudaContextoDto>();
            }
        }

        /// <summary>
        /// Guarda el estado de expansión de una sección de ayuda.
        /// </summary>
        public async Task<bool> GuardarEstadoExpandidoAsync(int idRol, int idAyuda, int idSeccion, bool expandida)
        {
            try
            {
                // Crear carpeta si no existe
                Directory.CreateDirectory(_rutaPreferencias);

                var rutaArchivo = Path.Combine(_rutaPreferencias, $"rol_{idRol}_ayuda_{idAyuda}.json");

                var preferencias = new Dictionary<int, bool>();
                if (File.Exists(rutaArchivo))
                {
                    var contenido = await File.ReadAllTextAsync(rutaArchivo);
                    preferencias = JsonSerializer.Deserialize<Dictionary<int, bool>>(contenido) 
                        ?? new Dictionary<int, bool>();
                }

                preferencias[idSeccion] = expandida;

                var json = JsonSerializer.Serialize(preferencias, 
                    new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(rutaArchivo, json);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GuardarEstadoExpandidoAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Carga los datos desde el archivo JSON (con caché).
        /// </summary>
        private async Task<Dictionary<int, AyudaPorRolDto>> CargarDatosAsync()
        {
            // Si ya está en caché, devolverlo
            if (_cache != null)
                return _cache;

            if (!File.Exists(_rutaJson))
                return new Dictionary<int, AyudaPorRolDto>();

            try
            {
                var contenido = await File.ReadAllTextAsync(_rutaJson);
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var raiz = JsonSerializer.Deserialize<JsonElement>(contenido, opciones);

                _cache = new Dictionary<int, AyudaPorRolDto>();

                if (raiz.TryGetProperty("roles", out var rolesElement) && 
                    rolesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var rolParam in rolesElement.EnumerateArray())
                    {
                        var rolDto = JsonSerializer.Deserialize<AyudaPorRolDto>(
                            rolParam.GetRawText(), opciones);

                        if (rolDto != null && rolDto.IdRol > 0)
                        {
                            _cache[rolDto.IdRol] = rolDto;
                        }
                    }
                }

                return _cache;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar JSON: {ex.Message}");
                return new Dictionary<int, AyudaPorRolDto>();
            }
        }

        /// <summary>
        /// Aplica las preferencias guardadas (estado expandido) a las secciones.
        /// </summary>
        private async Task AplicarPreferenciasAsync(int idRol, int idAyuda, IEnumerable<AyudaSeccionDto> secciones)
        {
            try
            {
                var rutaArchivo = Path.Combine(_rutaPreferencias, $"rol_{idRol}_ayuda_{idAyuda}.json");

                if (!File.Exists(rutaArchivo))
                    return;

                var contenido = await File.ReadAllTextAsync(rutaArchivo);
                var preferencias = JsonSerializer.Deserialize<Dictionary<int, bool>>(contenido) 
                    ?? new Dictionary<int, bool>();

                foreach (var seccion in secciones)
                {
                    if (preferencias.TryGetValue(seccion.IdSeccion, out var expandida))
                    {
                        seccion.Expandida = expandida;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al aplicar preferencias: {ex.Message}");
            }
        }
    }
}
