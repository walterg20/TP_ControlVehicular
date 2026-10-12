using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Windows;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class DriveBackupService : IDriveBackupService
    {
        // Las credenciales OAuth del proyecto de Google Cloud se leen de un archivo
        // "credenciales.json" (tipo Desktop app) copiado junto al ejecutable. No van
        // embebidas en el codigo ni viajan al repositorio (ver .gitignore).
        private const string ArchivoCredenciales = "credenciales.json";

        // Nombre de la carpeta en Drive que se crea automaticamente la primera vez.
        private const string NombreCarpetaDrive = "ControlVehicularBackups";

        // Identificador con el que se guarda el token de refresco por-PC.
        private const string UsuarioCredencial = "usuario";

        private readonly IBackupConfiguracionService _configuracionService;
        private readonly IDatabaseBackupService _databaseBackupService;
        private readonly IDataStore _dataStore;

        public DriveBackupService(
            IBackupConfiguracionService configuracionService,
            IDatabaseBackupService databaseBackupService)
        {
            _configuracionService = configuracionService;
            _databaseBackupService = databaseBackupService;
            _dataStore = new DpapiFileDataStore();
        }

        public bool TieneCredencialesGuardadas()
        {
            try
            {
                var token = _dataStore.GetAsync<TokenResponse>(UsuarioCredencial).GetAwaiter().GetResult();
                return token != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AutenticarAsync(Window owner)
        {
            // El broker abre el navegador del sistema para el consentimiento y
            // guarda el token (cifrado con DPAPI) en el almacen por-PC.
            var credencial = await ObtenerCredencialAsync(CancellationToken.None);
            return credencial?.Token != null;
        }

        public async Task<DriveUploadResultDto> SubirUltimoAsync()
        {
            var config = await _configuracionService.ObtenerAsync();
            var respaldos = await _databaseBackupService.ListarAsync();

            if (respaldos.Count == 0)
                throw new InvalidOperationException("No se encontraron respaldos (.bak) para resguardar.");

            var ultimo = respaldos[0];

            if (YaResguardado(config, ultimo.Nombre))
            {
                return new DriveUploadResultDto
                {
                    Subido = false,
                    Archivo = ultimo.Nombre,
                    Mensaje = "El ultimo respaldo ya fue resguardado hoy; no se vuelve a subir."
                };
            }

            if (!TieneCredencialesGuardadas())
                throw new InvalidOperationException("Debe conectar con Google Drive antes de resguardar el respaldo.");

            if (!File.Exists(ultimo.RutaCompleta))
                throw new InvalidOperationException($"No se pudo acceder al archivo de respaldo: {ultimo.RutaCompleta}");

            var credencial = await ObtenerCredencialAsync(CancellationToken.None);

            using var servicio = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credencial,
                ApplicationName = "TP_ControlVehicular"
            });

            var nombreDrive = $"{ultimo.FechaModificacion:yyyyMMdd}_FULL.bak";
            var carpetaId = await ObtenerOCrearCarpetaAsync(servicio, config);

            var idDrive = await BuscarArchivoAsync(servicio, carpetaId, nombreDrive);

            await using (var contenido = File.OpenRead(ultimo.RutaCompleta))
            {
                if (string.IsNullOrEmpty(idDrive))
                {
                    var metadatos = new Google.Apis.Drive.v3.Data.File { Name = nombreDrive };

                    if (carpetaId != null)
                        metadatos.Parents = new List<string> { carpetaId };

                    var creacion = servicio.Files.Create(metadatos, contenido, "application/octet-stream");
                    creacion.Fields = "id";
                    await creacion.UploadAsync();

                    idDrive = creacion.ResponseBody?.Id ?? string.Empty;
                }
                else
                {
                    var metadatos = new Google.Apis.Drive.v3.Data.File { Name = nombreDrive };
                    var actualizacion = servicio.Files.Update(metadatos, idDrive, contenido, "application/octet-stream");
                    actualizacion.Fields = "id";
                    await actualizacion.UploadAsync();
                }
            }

            await _configuracionService.RegistrarSubidaAsync(ultimo.Nombre, DateTime.Now);

            return new DriveUploadResultDto
            {
                Subido = true,
                Archivo = ultimo.Nombre,
                IdDrive = idDrive,
                Mensaje = $"Respaldo resguardado en Google Drive como {nombreDrive}."
            };
        }

        private static bool YaResguardado(BackupConfiguracionDto config, string nombre)
        {
            return string.Equals(config.UltimaSubidaArchivo, nombre, StringComparison.OrdinalIgnoreCase)
                && config.FechaUltimaSubida.HasValue
                && config.FechaUltimaSubida.Value.Date == DateTime.Now.Date;
        }

        private async Task<UserCredential> ObtenerCredencialAsync(CancellationToken cancellationToken)
        {
            var rutaCredenciales = Path.Combine(AppContext.BaseDirectory, ArchivoCredenciales);

            if (!File.Exists(rutaCredenciales))
            {
                throw new InvalidOperationException(
                    $"No se encontró el archivo de credenciales de Google Drive ({rutaCredenciales}). " +
                    "Copie credenciales.json (descargado de Google Cloud) junto al ejecutable.");
            }

            using var stream = File.OpenRead(rutaCredenciales);
            var secretos = GoogleClientSecrets.FromStream(stream).Secrets;

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secretos,
                new[] { DriveService.Scope.DriveFile },
                UsuarioCredencial,
                cancellationToken,
                _dataStore);
        }

        // Resuelve la carpeta destino en Drive: usa la configurada si todavía existe; si no,
        // busca o crea "ControlVehicularBackups" y persiste su id real en la configuración.
        private async Task<string?> ObtenerOCrearCarpetaAsync(DriveService servicio, BackupConfiguracionDto config)
        {
            var carpetaId = string.IsNullOrWhiteSpace(config.DriveFolderId) ? null : config.DriveFolderId.Trim();

            if (!string.IsNullOrEmpty(carpetaId) && await EsCarpetaValidaAsync(servicio, carpetaId))
                return carpetaId;

            // El Id configurado ya no existe o no es una carpeta (p. ej. se escribió el
            // nombre en vez del Id): se re-resuelve la carpeta y se persiste el Id real.
            return await BuscarOCrearCarpetaAsync(servicio, config);
        }

        // Valida que un Id de Drive siga existiendo y sea una carpeta.
        private static async Task<bool> EsCarpetaValidaAsync(DriveService servicio, string carpetaId)
        {
            try
            {
                var peticion = servicio.Files.Get(carpetaId);
                peticion.Fields = "id, mimeType";
                var archivo = await peticion.ExecuteAsync();

                return string.Equals(
                    archivo?.MimeType,
                    "application/vnd.google-apps.folder",
                    StringComparison.OrdinalIgnoreCase);
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // La carpeta se eliminó en Drive o pertenece a otra cuenta: Id obsoleto.
                return false;
            }
        }

        // Busca la carpeta "ControlVehicularBackups" por nombre o la crea; persiste su Id.
        private async Task<string> BuscarOCrearCarpetaAsync(DriveService servicio, BackupConfiguracionDto config)
        {
            var consulta =
                $"mimeType = 'application/vnd.google-apps.folder' and name = '{Escapar(NombreCarpetaDrive)}' and trashed = false";

            var busqueda = servicio.Files.List();
            busqueda.Q = consulta;
            busqueda.Fields = "files(id, name)";
            busqueda.Spaces = "drive";

            var resultado = await busqueda.ExecuteAsync();
            var existente = resultado.Files?.FirstOrDefault();

            if (existente != null)
            {
                config.DriveFolderId = existente.Id;
                await _configuracionService.GuardarAsync(config);
                return existente.Id;
            }

            var metadatos = new Google.Apis.Drive.v3.Data.File
            {
                Name = NombreCarpetaDrive,
                MimeType = "application/vnd.google-apps.folder"
            };

            var creacion = servicio.Files.Create(metadatos);
            creacion.Fields = "id";
            var creada = await creacion.ExecuteAsync();

            config.DriveFolderId = creada.Id;
            await _configuracionService.GuardarAsync(config);
            return creada.Id;
        }

        private static async Task<string> BuscarArchivoAsync(DriveService servicio, string? carpetaId, string nombre)
        {
            var consulta = $"name = '{Escapar(nombre)}' and trashed = false";

            if (!string.IsNullOrWhiteSpace(carpetaId))
                consulta += $" and '{Escapar(carpetaId)}' in parents";

            var peticion = servicio.Files.List();
            peticion.Q = consulta;
            peticion.Fields = "files(id, name)";
            peticion.Spaces = "drive";

            var resultado = await peticion.ExecuteAsync();
            return resultado.Files?.FirstOrDefault()?.Id ?? string.Empty;
        }

        private static string Escapar(string valor) => valor.Replace("'", "\\'");
    }
}
