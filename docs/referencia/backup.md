# Referencia — Respaldo de base de datos

Mecanismo de respaldo, verificación y resguardo en la nube. Se enlaza desde `AGENTS.md` → *Referencia por dominio*.

> **Puesta en marcha paso a paso**: ver `docs/referencia/backup-configuracion.md` (mini tutorial: requisitos, Google Cloud, rutas y prueba del flujo).

## Objetivo y alcance
- Despliegue objetivo: **2 PCs de un taller contra una base SQL Server central** (sin LocalDB).
- Esta fase cubre **solo la configuración de la app** para: hacer backup → probarlo (restore) → resguardarlo en Google Drive.
- SQL Server 2019 **Express** no incluye Agente → no hay respaldos automáticos por defecto.

## Scripts (`bd/`, idempotentes)
- `bd/21_ConfiguracionBackup.sql` — tabla singleton `dbo.ConfiguracionBackup` (una sola fila, `Id = 1`) + fila semilla (rutas vacías).
- `bd/22_PermisosBackup.sql` — permiso `Configuracion.Backup.Gestionar` → **Admin (`IdRol = 1`)**. Recepcionista (2) y Mecánico (3) sin acceso.

### Tabla `ConfiguracionBackup`
| Columna | Notas |
|---|---|
| `Id` | Siempre `1` (singleton). |
| `NombreBaseDatos` | Por defecto `ControlVehicular`. |
| `RutaBackupServer` | Ruta **server-side** donde el servicio SQL escribe el `.bak` (absoluta, del servidor). |
| `RutaBackupUNC` | Ruta **UNC** (`\\servidor\...`) que lee la app para listar y subir. |
| `DriveFolderId` | Carpeta destino en Google Drive. |
| `UltimaSubidaArchivo` / `FechaUltimaSubida` | Control de "un archivo por día" (los actualiza el servicio de subida, **no** `GuardarAsync`). |
| `FechaActualizacion` | Marca de última edición de la configuración. |

## Servicios (negocio)
- `IDatabaseBackupService` — `HacerBackupAsync` / `ProbarBackupAsync(string)` / `ListarAsync()`; DTOs `BackupResultDto`, `BackupTestResultDto`, `BackupArchivoDto`.
- `IDriveBackupService` — `TieneCredencialesGuardadas()` / `AutenticarAsync(Window)` / `SubirUltimoAsync()`; DTO `DriveUploadResultDto`.
- `IBackupConfiguracionService` — `ObtenerAsync` / `GuardarAsync` / `RegistrarSubidaAsync(string, DateTime)`.

### Dos rutas de backup
- El `BACKUP`/`RESTORE` los ejecuta el **servicio SQL** vía `Microsoft.Data.SqlClient.SqlConnection` directo (no `ExecuteSqlRaw`); la ruta es **server-side**.
- La app lista/sube leyendo `RutaBackupUNC`. Si la app no puede leer el tamaño del `.bak` → `LeerTamanoSiAccesible` con try/catch.

### Hacer backup
- `BACKUP DATABASE [..] TO DISK=@r WITH INIT, COMPRESSION, CHECKSUM, STATS=10;` + `RESTORE VERIFYONLY`.
- Naming: `ControlVehicular_YYYYMMDD_HHMMSS_FULL.bak`.

### Probar backup (restore a prueba)
- `RESTORE FILELISTONLY` (nombres lógicos dinámicos) → carpeta destino vía `SERVERPROPERTY('InstanceDefaultDataPath')` → `DROP DATABASE ControlVehicular_Test` con `SINGLE_USER WITH ROLLBACK IMMEDIATE` → `RESTORE ... WITH MOVE, REPLACE, RECOVERY`.
- Verifica `DATABASEPROPERTYEX(...,'Status') = 'ONLINE'` y `COUNT(*)` de `ControlVehicular_Test.sys.tables`.
- **Nunca** toca la base productiva.

### Subida a Google Drive
- **API directa** (`Google.Apis.Drive.v3` + `Google.Apis.Auth`), **sin rclone**.
- Scope `drive.file`; consent screen de Google Cloud **publicado**.
- ClientId/Secret embebidos en `DriveBackupService` (hoy placeholders `PENDIENTE_CLIENT_ID` / `PENDIENTE_CLIENT_SECRET`).
- Token de refresco guardado **por-PC con DPAPI** en `%LocalAppData%\TP_ControlVehicular\Drive` (`DpapiFileDataStore : IDataStore`), **no** en la tabla. Clave del token: `UsuarioCredencial = "usuario"`.
- **Un archivo por día**: nombre Drive `ControlVehicular_YYYYMMDD_FULL.bak`; re-subir el mismo día **sobrescribe**.
- Botón **Resguardar**: sube **siempre el último** `.bak` y **solo si no se subió todavía** (compara `UltimaSubidaArchivo` + `FechaUltimaSubida` del mismo día contra la config).

## Presentación
- `CtlBackup` — listado (`Nombre/Archivo`, `FechaModificacion`, `TamanoLegible`) + botones **Hacer Backup** / **Probar Backup** / **Resguardar** / **Actualizar**.
- `CtlBackupConfiguracion` — form (`NombreBaseDatos`, `RutaBackupServer`, `RutaBackupUNC`, `DriveFolderId`) + **Conectar con Google Drive** / **Guardar**.
- Menú **Administración** con gating por `Configuracion.Backup.Gestionar`. VMs muestran `FrmConfirmacion`; el code-behind solo invoca métodos del VM.

## Fuera de alcance (fase posterior)
- **Cifrado** del `.bak`.
- **Agendado/automatización** del respaldo (Task Scheduler + `sqlcmd`).
- **Permisos SQL del login** (`db_backupoperator` / `dbcreator`) o SPs firmados por certificado: se resuelven en el despliegue real (hoy Windows Auth contra `WALTERG20\SQLEXPRESS2019`).
- Backup diferencial/logarítmico y retención GFS.
- Instalador / alta automática de base y datos por defecto (aspiracional).
