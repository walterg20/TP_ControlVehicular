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
        // En apps de escritorio el "secreto" del cliente es publico por definicion.
        // Reemplazar por los valores reales del proyecto de Google Cloud.
        private const string ClientId = "PENDIENTE_CLIENT_ID";
        private const string ClientSecret = "PENDIENTE_CLIENT_SECRET";

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
            var carpetaId = string.IsNullOrWhiteSpace(config.DriveFolderId) ? null : config.DriveFolderId.Trim();

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
            var secretos = new ClientSecrets
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret
            };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secretos,
                new[] { DriveService.Scope.DriveFile },
                UsuarioCredencial,
                cancellationToken,
                _dataStore);
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
