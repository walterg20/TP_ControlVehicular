// walterg20
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class DatabaseBackupService : IDatabaseBackupService
    {
        private const string NombreBasePrueba = "ControlVehicular_Test";

        private readonly IBackupConfiguracionService _configuracionService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly string _connectionString;

        public DatabaseBackupService(IBackupConfiguracionService configuracionService, IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _configuracionService = configuracionService;
            _usuarioRepository = usuarioRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public async Task<BackupResultDto> HacerBackupAsync()
        {
            var config = await _configuracionService.ObtenerAsync();

            if (string.IsNullOrWhiteSpace(config.RutaBackupServer))
                throw new InvalidOperationException("Debe configurar la Ruta de Backup del servidor antes de generar un respaldo.");

            var nombreBase = string.IsNullOrWhiteSpace(config.NombreBaseDatos) ? "ControlVehicular" : config.NombreBaseDatos.Trim();
            var carpeta = config.RutaBackupServer.Trim();
            var fecha = DateTime.Now;
            var archivo = $"{nombreBase}_{fecha:yyyyMMdd_HHmmss}_FULL.bak";
            var rutaCompleta = Path.Combine(carpeta, archivo);

            await using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();

            var sqlBackup = $"BACKUP DATABASE [{nombreBase}] TO DISK = N'{Escapar(rutaCompleta)}' WITH INIT, CHECKSUM, STATS = 10;";
            await using (var comando = new SqlCommand(sqlBackup, conexion) { CommandTimeout = 0 })
            {
                await comando.ExecuteNonQueryAsync();
            }

            var sqlVerificar = $"RESTORE VERIFYONLY FROM DISK = N'{Escapar(rutaCompleta)}' WITH CHECKSUM;";
            await using (var comando = new SqlCommand(sqlVerificar, conexion) { CommandTimeout = 0 })
            {
                await comando.ExecuteNonQueryAsync();
            }

            return new BackupResultDto
            {
                Exitoso = true,
                Archivo = archivo,
                Ruta = rutaCompleta,
                TamanoBytes = LeerTamanoSiAccesible(rutaCompleta),
                FechaHora = fecha,
                Mensaje = $"Respaldo generado y verificado correctamente: {archivo}"
            };
        }

        public async Task<BackupTestResultDto> ProbarBackupAsync(string archivoBak)
        {
            if (string.IsNullOrWhiteSpace(archivoBak))
                throw new InvalidOperationException("Debe seleccionar un archivo de respaldo (.bak) para probar.");

            var config = await _configuracionService.ObtenerAsync();
            var rutaBak = Path.IsPathRooted(archivoBak)
                ? archivoBak
                : Path.Combine(ObtenerCarpetaEfectiva(config), archivoBak);

            await using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();

            try
            {
                // 1. Descubrir los nombres logicos (datos y log) del respaldo.
                var (logicoDatos, logicoLog) = await ObtenerNombresLogicosAsync(conexion, rutaBak);

                if (string.IsNullOrEmpty(logicoDatos) || string.IsNullOrEmpty(logicoLog))
                    throw new InvalidOperationException("No se pudieron determinar los nombres logicos de datos/log del respaldo.");

                // 2. Carpeta del servidor donde se crearan los archivos de la base de prueba.
                var carpetaDatos = await ObtenerCarpetaDatosServidorAsync(conexion);
                var rutaMdf = Path.Combine(carpetaDatos, $"{NombreBasePrueba}.mdf");
                var rutaLdf = Path.Combine(carpetaDatos, $"{NombreBasePrueba}_log.ldf");

                // 3. Dropear la base de prueba si ya existe (no toca la base productiva).
                var sqlDrop = $@"
IF DB_ID('{NombreBasePrueba}') IS NOT NULL
BEGIN
    ALTER DATABASE [{NombreBasePrueba}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [{NombreBasePrueba}];
END";
                await using (var comando = new SqlCommand(sqlDrop, conexion) { CommandTimeout = 0 })
                {
                    await comando.ExecuteNonQueryAsync();
                }

                // 4. Restaurar el respaldo en la base de prueba.
                var sqlRestore = $@"RESTORE DATABASE [{NombreBasePrueba}] FROM DISK = N'{Escapar(rutaBak)}'
WITH MOVE N'{Escapar(logicoDatos)}' TO N'{Escapar(rutaMdf)}',
     MOVE N'{Escapar(logicoLog)}' TO N'{Escapar(rutaLdf)}',
     REPLACE, RECOVERY;";
                await using (var comando = new SqlCommand(sqlRestore, conexion) { CommandTimeout = 0 })
                {
                    await comando.ExecuteNonQueryAsync();
                }

                // 5. Verificar que la base de prueba quedo ONLINE y consultable.
                var estado = await ObtenerEstadoAsync(conexion);
                var cantidadTablas = await ContarTablasAsync(conexion);

                var detalle = new StringBuilder();
                detalle.AppendLine($"Base de prueba: {NombreBasePrueba}");
                detalle.AppendLine($"Estado: {estado}");
                detalle.AppendLine($"Tablas detectadas: {cantidadTablas}");
                detalle.AppendLine("El respaldo se restauro y se pudo consultar correctamente.");

                var exitoso = string.Equals(estado, "ONLINE", StringComparison.OrdinalIgnoreCase) && cantidadTablas > 0;

                return new BackupTestResultDto
                {
                    Exitoso = exitoso,
                    NombreBasePrueba = NombreBasePrueba,
                    Detalle = detalle.ToString().Trim()
                };
            }
            catch
            {
                await IntentarLimpiarBasePruebaAsync(conexion);
                throw;
            }
        }

        public async Task<BackupResultDto> RestaurarAsync(string archivoBak, string dniAutorizador, string contrasenaAutorizador)
        {
            if (string.IsNullOrWhiteSpace(archivoBak))
                throw new InvalidOperationException("Debe seleccionar un archivo de respaldo (.bak) para restaurar.");

            if (string.IsNullOrWhiteSpace(dniAutorizador))
                throw new InvalidOperationException("Debe ingresar el DNI del usuario autorizador.");

            if (string.IsNullOrWhiteSpace(contrasenaAutorizador))
                throw new InvalidOperationException("Debe ingresar la contraseña del usuario autorizador.");

            // 1. Validar segundo factor.
            var autorizador = await _usuarioRepository.GetByDniAsync(dniAutorizador.Trim());
            if (autorizador == null)
                throw new InvalidOperationException("El usuario autorizador no se encuentra registrado.");

            if (!autorizador.Activo)
                throw new InvalidOperationException("El usuario autorizador se encuentra inactivo.");

            if (autorizador.Contrasena != contrasenaAutorizador)
                throw new InvalidOperationException("La contraseña del usuario autorizador es incorrecta.");

            var usuarioActual = UserSession.CurrentUser;
            if (usuarioActual != null && autorizador.Id == usuarioActual.IdUsuario)
                throw new InvalidOperationException("El usuario autorizador debe ser distinto al usuario de la sesión actual.");

            var tienePermiso = autorizador.Rol?.Permisos != null
                && autorizador.Rol.Permisos.Any(p => string.Equals(p.Nombre, "Configuracion.Backup.Gestionar", StringComparison.OrdinalIgnoreCase));
            if (!tienePermiso)
                throw new InvalidOperationException("El usuario autorizador no tiene permiso para gestionar los respaldos.");

            // 2. Resolver ruta del respaldo.
            var config = await _configuracionService.ObtenerAsync();
            var nombreBase = string.IsNullOrWhiteSpace(config.NombreBaseDatos) ? "ControlVehicular" : config.NombreBaseDatos.Trim();
            var rutaBak = Path.IsPathRooted(archivoBak)
                ? archivoBak
                : Path.Combine(ObtenerCarpetaEfectiva(config), archivoBak);

            // 3. Conectar a master para poder restaurar la BD productiva.
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };
            var connStrMaster = builder.ConnectionString;

            await using var conexion = new SqlConnection(connStrMaster);
            await conexion.OpenAsync();

            try
            {
                // 4. Descubrir nombres logicos del respaldo.
                var (logicoDatos, logicoLog) = await ObtenerNombresLogicosAsync(conexion, rutaBak);
                if (string.IsNullOrEmpty(logicoDatos) || string.IsNullOrEmpty(logicoLog))
                    throw new InvalidOperationException("No se pudieron determinar los nombres lógicos de datos/log del respaldo.");

                // 5. Carpeta del servidor para los archivos de la BD.
                var carpetaDatos = await ObtenerCarpetaDatosServidorAsync(conexion);
                var rutaMdf = Path.Combine(carpetaDatos, $"{nombreBase}.mdf");
                var rutaLdf = Path.Combine(carpetaDatos, $"{nombreBase}_log.ldf");

                // 6. Poner la BD productiva en SINGLE_USER.
                var sqlSingle = $@"
IF DB_ID('{Escapar(nombreBase)}') IS NOT NULL
BEGIN
    ALTER DATABASE [{nombreBase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END";
                await using (var comando = new SqlCommand(sqlSingle, conexion) { CommandTimeout = 0 })
                {
                    await comando.ExecuteNonQueryAsync();
                }

                // 7. Restaurar la BD productiva.
                var sqlRestore = $@"RESTORE DATABASE [{nombreBase}] FROM DISK = N'{Escapar(rutaBak)}'
WITH MOVE N'{Escapar(logicoDatos)}' TO N'{Escapar(rutaMdf)}',
     MOVE N'{Escapar(logicoLog)}' TO N'{Escapar(rutaLdf)}',
     REPLACE, RECOVERY;";
                await using (var comando = new SqlCommand(sqlRestore, conexion) { CommandTimeout = 0 })
                {
                    await comando.ExecuteNonQueryAsync();
                }

                // 8. Volver a MULTI_USER.
                var sqlMulti = $@"
IF DB_ID('{Escapar(nombreBase)}') IS NOT NULL
BEGIN
    ALTER DATABASE [{nombreBase}] SET MULTI_USER;
END";
                await using (var comando = new SqlCommand(sqlMulti, conexion) { CommandTimeout = 0 })
                {
                    await comando.ExecuteNonQueryAsync();
                }

                return new BackupResultDto
                {
                    Exitoso = true,
                    Archivo = Path.GetFileName(rutaBak),
                    Ruta = rutaBak,
                    FechaHora = DateTime.Now,
                    Mensaje = "Restauración completada correctamente. Reinicie la aplicación para que los cambios tengan efecto."
                };
            }
            catch
            {
                // Intentar volver a MULTI_USER si la BD quedó en SINGLE_USER.
                try
                {
                    var sqlMulti = $@"
IF DB_ID('{Escapar(nombreBase)}') IS NOT NULL
BEGIN
    ALTER DATABASE [{nombreBase}] SET MULTI_USER;
END";
                    await using var comando = new SqlCommand(sqlMulti, conexion) { CommandTimeout = 0 };
                    await comando.ExecuteNonQueryAsync();
                }
                catch
                {
                    // Ignorar errores de limpieza.
                }
                throw;
            }
        }

        public async Task<IReadOnlyList<BackupArchivoDto>> ListarAsync()
        {
            var config = await _configuracionService.ObtenerAsync();
            var carpeta = ObtenerCarpetaEfectiva(config);

            var lista = new List<BackupArchivoDto>();

            if (string.IsNullOrWhiteSpace(carpeta) || !Directory.Exists(carpeta))
                return lista;

            foreach (var ruta in Directory.EnumerateFiles(carpeta, "*.bak"))
            {
                var info = new FileInfo(ruta);
                lista.Add(new BackupArchivoDto
                {
                    Nombre = info.Name,
                    RutaCompleta = info.FullName,
                    TamanoBytes = info.Length,
                    TamanoLegible = FormatearTamano(info.Length),
                    FechaModificacion = info.LastWriteTime
                });
            }

            return lista.OrderByDescending(b => b.FechaModificacion).ToList();
        }

        private static string ObtenerCarpetaEfectiva(BackupConfiguracionDto config)
        {
            return !string.IsNullOrWhiteSpace(config.RutaBackupUNC)
                ? config.RutaBackupUNC.Trim()
                : config.RutaBackupServer.Trim();
        }

        private static string Escapar(string valor) => valor.Replace("'", "''");

        private static long LeerTamanoSiAccesible(string rutaCompleta)
        {
            try
            {
                var info = new FileInfo(rutaCompleta);
                return info.Exists ? info.Length : 0;
            }
            catch
            {
                // La ruta pertenece al servidor de base de datos y puede no ser accesible desde la app.
                return 0;
            }
        }

        private static string FormatearTamano(long bytes)
        {
            if (bytes <= 0) return "-";
            if (bytes >= 1024L * 1024L * 1024L) return $"{bytes / (1024d * 1024d * 1024d):N2} GB";
            if (bytes >= 1024L * 1024L) return $"{bytes / (1024d * 1024d):N2} MB";
            if (bytes >= 1024L) return $"{bytes / 1024d:N2} KB";
            return $"{bytes} B";
        }

        private async Task<(string logicoDatos, string logicoLog)> ObtenerNombresLogicosAsync(SqlConnection conexion, string rutaBak)
        {
            string logicoDatos = string.Empty;
            string logicoLog = string.Empty;

            var sql = $"RESTORE FILELISTONLY FROM DISK = N'{Escapar(rutaBak)}';";
            await using var comando = new SqlCommand(sql, conexion) { CommandTimeout = 0 };
            await using var reader = await comando.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var tipo = reader.GetString(reader.GetOrdinal("Type"));
                var nombreLogico = reader.GetString(reader.GetOrdinal("LogicalName"));

                if (tipo == "D" && string.IsNullOrEmpty(logicoDatos))
                    logicoDatos = nombreLogico;
                else if (tipo == "L" && string.IsNullOrEmpty(logicoLog))
                    logicoLog = nombreLogico;
            }

            return (logicoDatos, logicoLog);
        }

        private async Task<string> ObtenerCarpetaDatosServidorAsync(SqlConnection conexion)
        {
            var sql = "SELECT CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS nvarchar(4000));";
            await using var comando = new SqlCommand(sql, conexion) { CommandTimeout = 0 };
            var resultado = await comando.ExecuteScalarAsync();

            var carpeta = resultado as string;

            if (string.IsNullOrWhiteSpace(carpeta))
                throw new InvalidOperationException("No se pudo determinar la carpeta de datos por defecto del servidor.");

            return carpeta;
        }

        private async Task<string> ObtenerEstadoAsync(SqlConnection conexion)
        {
            var sql = $"SELECT CAST(DATABASEPROPERTYEX('{NombreBasePrueba}', 'Status') AS nvarchar(50));";
            await using var comando = new SqlCommand(sql, conexion) { CommandTimeout = 0 };
            var resultado = await comando.ExecuteScalarAsync();
            return resultado as string ?? string.Empty;
        }

        private async Task<int> ContarTablasAsync(SqlConnection conexion)
        {
            var sql = $"SELECT COUNT(*) FROM [{NombreBasePrueba}].sys.tables;";
            await using var comando = new SqlCommand(sql, conexion) { CommandTimeout = 0 };
            var resultado = await comando.ExecuteScalarAsync();
            return resultado == null || resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
        }

        private async Task IntentarLimpiarBasePruebaAsync(SqlConnection conexion)
        {
            try
            {
                var sqlDrop = $@"
IF DB_ID('{NombreBasePrueba}') IS NOT NULL
BEGIN
    ALTER DATABASE [{NombreBasePrueba}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [{NombreBasePrueba}];
END";
                await using var comando = new SqlCommand(sqlDrop, conexion) { CommandTimeout = 0 };
                await comando.ExecuteNonQueryAsync();
            }
            catch
            {
                // Si la limpieza falla, se propaga la excepcion original.
            }
        }
    }
}
