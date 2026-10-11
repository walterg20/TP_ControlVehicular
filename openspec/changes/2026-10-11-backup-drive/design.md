# Technical Design: Respaldo de Base de Datos con Resguardo en Google Drive

## Design Details

### 1. Modelo de datos

**Entidad** `Entidad/ConfiguracionBackup.cs` (singleton, `Id = 1`):

```csharp
public class ConfiguracionBackup
{
    public int Id { get; set; }
    public string NombreBaseDatos { get; set; } = "ControlVehicular";
    public string RutaBackupServer { get; set; } = string.Empty;   // ruta local del servidor (el servicio SQL escribe el .bak)
    public string RutaBackupUNC { get; set; } = string.Empty;      // ruta compartida que lee la app (listar/subir)
    public string? DriveFolderId { get; set; }                     // carpeta destino en Drive (null = se crea/busca "ControlVehicularBackups")
    public string? UltimaSubidaArchivo { get; set; }               // nombre del último .bak subido
    public DateTime? FechaUltimaSubida { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
```

Convenciones del proyecto: `string` → `string.Empty`; sin navegación.

**Por qué dos rutas.** `BACKUP DATABASE` lo escribe el **servicio SQL**, por lo que la ruta del `.bak` es absoluta **del servidor** (`RutaBackupServer`, ej. `C:\Backups\ControlVehicular`). La app, que puede correr en otra PC, no puede leer `C:\...` del servidor; necesita la ruta **compartida UNC** (`RutaBackupUNC`, ej. `\\SERVIDOR\Backups`) para listar y subir el archivo.

**Configuración EF** `Datos/Configuracion/ConfiguracionBackupConfiguration.cs` (`IEntityTypeConfiguration<ConfiguracionBackup>`), siguiendo `ConfiguracionEmpresaConfiguration`:
- `ToTable("ConfiguracionBackup")`, `HasKey(c => c.Id)`.
- `NombreBaseDatos` (128) y `RutaBackupServer`/`RutaBackupUNC` (260) required; `DriveFolderId`/`UltimaSubidaArchivo` (260) opcionales.

**`DbSet`** en `Datos/Data/CVDbContext.cs`: `public DbSet<ConfiguracionBackup> ConfiguracionBackup { get; set; } = null!;`

**Esquema** (`bd/21_ConfiguracionBackup.sql`, idempotente — el proyecto **no mantiene migraciones EF**):
- `IF OBJECT_ID('ConfiguracionBackup','U') IS NULL CREATE TABLE ...`.
- `IF NOT EXISTS (SELECT 1 FROM ConfiguracionBackup) INSERT` la fila semilla `Id = 1` con `NombreBaseDatos = 'ControlVehicular'` y rutas vacías.

**Permisos** (`bd/22_PermisosBackup.sql`, idempotente, patrón `bd/12`/`bd/20`):
- `INSERT INTO Permisos (Nombre) VALUES ('Configuracion.Backup.Gestionar')` con `WHERE NOT EXISTS`.
- Asignar a Administrador (`IdRol = 1`) con `WHERE NOT EXISTS` en `RolPermisos`. No se asigna a Recepcionista (2) ni Mecánico (3).

### 2. Repositorio y servicio de configuración

**Repositorio** `Datos/Repositories/ConfiguracionBackupRepository.cs` sobre `Repository<T>` +
`Negocio/Interfaces/IConfiguracionBackupRepository.cs` con `Task<ConfiguracionBackup?> ObtenerAsync()` (siempre la fila `Id = 1`).

**Servicio** `Negocio/Interfaces/IBackupConfiguracionService.cs`:
- `Task<BackupConfiguracionDto> ObtenerAsync()`
- `Task<BackupConfiguracionDto> GuardarAsync(BackupConfiguracionDto dto)`

Implementación `Negocio/Services/BackupConfiguracionService.cs`: usa el repositorio + `IMapper`; al guardar setea `FechaActualizacion = DateTime.Now` (y crea la fila `Id = 1` si no existe, patrón `EmpresaConfiguracionService`).

**DTO** `Negocio/DTOs/BackupConfiguracionDto.cs`: `NombreBaseDatos`, `RutaBackupServer`, `RutaBackupUNC`, `DriveFolderId`, `UltimaSubidaArchivo`, `FechaUltimaSubida`, `FechaActualizacion`. `MappingProfile` agrega el mapeo unidireccional `ConfiguracionBackup` → `BackupConfiguracionDto`.

### 3. Servicio de base de datos (`IDatabaseBackupService`)

`Negocio/Interfaces/IDatabaseBackupService.cs`; implementación `Negocio/Services/DatabaseBackupService.cs`. Inyecta `IConfiguration` y usa `Microsoft.Data.SqlClient.SqlConnection` con la cadena `ConnectionStrings:DefaultConnection` (las sentencias `BACKUP`/`RESTORE` **no** pueden ejecutarse dentro de una transacción, por eso se usan comandos directos y no `ExecuteSqlRaw`).

- `Task<BackupResultDto> HacerBackupAsync()`:
  1. Lee la configuración; valida que `RutaBackupServer` no esté vacía.
  2. Nombre de archivo `ControlVehicular_YYYYMMDD_HHMMSS_FULL.bak` (según `NombreBaseDatos`).
  3. `BACKUP DATABASE [<db>] TO DISK = N'<ruta>' WITH INIT, COMPRESSION, CHECKSUM, STATS = 10;`
  4. `RESTORE VERIFYONLY FROM DISK = N'<ruta>' WITH CHECKSUM;`
  5. Devuelve `BackupResultDto { Archivo, Ruta, TamanoBytes, FechaHora, Mensaje }`.
- `Task<BackupTestResultDto> ProbarBackupAsync(string archivoBak)` — **nunca toca la base productiva**:
  1. `RESTORE FILELISTONLY FROM DISK = ...` para descubrir nombres lógicos/datos/log dinámicos.
  2. `IF DB_ID('ControlVehicular_Test') IS NOT NULL` → `ALTER DATABASE [ControlVehicular_Test] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [ControlVehicular_Test];`
  3. `RESTORE DATABASE [ControlVehicular_Test] FROM DISK = ... WITH MOVE '<logicoDatos>' TO '<carpeta>\ControlVehicular_Test.mdf', MOVE '<logicoLog>' TO '<carpeta>\ControlVehicular_Test_log.ldf', REPLACE, RECOVERY;`
  4. Verifica `DATABASEPROPERTYEX(...,'Status') = 'ONLINE'` y `SELECT COUNT(*)` de tablas clave.
  5. Devuelve `BackupTestResultDto { Exitoso, NombreBasePrueba, Detalle }`.
- `Task<IReadOnlyList<BackupArchivoDto>> ListarAsync()`: escanea `RutaBackupUNC` (o `RutaBackupServer` si la UNC está vacía) por `*.bak` y devuelve nombre, ruta completa, tamaño y fecha de modificación, ordenado descendente por fecha. Es más simple y confiable que consultar `msdb.dbo.backupset`.

### 4. Servicio de nube (`IDriveBackupService`, Google Drive API)

`Negocio/Interfaces/IDriveBackupService.cs`; implementación `Negocio/Services/DriveBackupService.cs`.

- **Autenticación OAuth** con `GoogleWebAuthorizationBroker.AuthorizeAsync` y scope **`DriveService.Scope.DriveFile`** (`drive.file`: la app solo ve/crea los archivos que ella misma subió → mínimo privilegio; no exige verificación de Google). El `Consent screen` del proyecto de Google Cloud debe estar **publicado**.
- **Credenciales OAuth desde archivo** `credenciales.json` (tipo *Desktop app*, descargado de Google Cloud) leído desde `AppContext.BaseDirectory` con `GoogleClientSecrets.FromStream`; el `.csproj` lo copia a la salida y está **gitignored** (cada PC aporta el suyo). Si falta, el servicio lanza un `InvalidOperationException` explicando dónde colocarlo. El `token` de refresco **no** viaja al repositorio.
- **Token de refresco por-PC** guardado con **DPAPI** (`System.Security.Cryptography.ProtectedData`) en `%LocalAppData%\TP_ControlVehicular\Drive\token.dat` (implementando un `IDataStore` propio sobre `FileDataStore`). **No** se persiste en la tabla ni en texto plano.
- **Carpeta destino en Drive**: si `DriveFolderId` está vacío, `ObtenerOCrearCarpetaAsync` busca (`mimeType = folder and name = 'ControlVehicularBackups' and trashed = false`) y, si no existe, **crea** la carpeta y persiste su id con `GuardarAsync` (el scope `drive.file` no ve carpetas ajenas, por eso la app la crea ella misma). Si hay un id configurado, se usa tal cual.
- `bool TieneCredencialesGuardadas()` — indica si ya hay token para habilitar "Resguardar".
- `Task<bool> AutenticarAsync(Window owner)` — lanza el flujo interactivo (una sola vez por PC).
- `Task<DriveUploadResultDto> SubirUltimoAsync()`:
  1. Toma el `.bak` más reciente de `RutaBackupUNC`.
  2. Si `UltimaSubidaArchivo == nombre` **y** `FechaUltimaSubida` es del mismo día → **no sube** (ya resguardado).
  3. Resuelve la carpeta destino con `ObtenerOCrearCarpetaAsync` (usa `DriveFolderId`; si está vacío, busca/crea `ControlVehicularBackups` y persiste su id).
  4. Nombre destino en Drive: `ControlVehicular_YYYYMMDD_FULL.bak` (un archivo por día). Si ya existe en la carpeta destino → lo **sobrescribe** (`Files.Update`); si no → `Files.Create`.
  5. Al terminar, actualiza `UltimaSubidaArchivo` y `FechaUltimaSubida` vía `IBackupConfiguracionService.RegistrarSubidaAsync`.
  6. Devuelve `DriveUploadResultDto { Subido, Archivo, IdDrive, Mensaje }`.

### 5. UI — Dos pantallas

Ambas siguen el patrón de 4 filas/tarjetas de los `Ctl*` existentes (ver `CtlConfiguracion`).

- `Presentacion/Pantalla/Configuracion/CtlBackup.xaml` + `.xaml.cs` — **Respaldos**: listado (`DataGrid`) de archivos `.bak` + 3 botones con la paleta del proyecto:
  - **💾 Hacer backup** (`#27AE60`) → `HacerBackupCommand`.
  - **🧪 Probar backup** (`#2980B9`) → `ProbarBackupCommand` sobre el `.bak` seleccionado.
  - **☁️ Resguardar en Drive** (`#8E44AD`) → `ResguardarCommand`.
  - **🔄 Actualizar** (`#2980B9`) → `ActualizarCommand`.
- `Presentacion/Pantalla/Configuracion/CtlBackupConfiguracion.xaml` + `.xaml.cs` — **Configuración de Respaldo**: campos `NombreBaseDatos`, `RutaBackupServer`, `RutaBackupUNC`, `DriveFolderId`, botón **Conectar con Google Drive** y botón **Guardar** (`#27AE60`, `CornerRadius=6`).
- ViewModels `Presentacion/ViewModels/BackupViewModel.cs` y `BackupConfiguracionViewModel.cs`, heredando `BaseViewModel` (`RelayCommand` async, `ObservableCollection<BackupArchivoDto>`, propiedades con `OnPropertyChanged()`).
- Los code-behind resuelven las VMs del DI (variable `vmBackup` / `vmBackupConfiguracion` según convención `vm<Entidad>`), registran `Loaded` **una sola vez** y usan `FrmConfirmacion.Mostrar` / `MostrarAviso` (nunca `MessageBox`).

### 6. Menú y RBAC

- `Presentacion/MainWindow.xaml`: nuevo ítem `💾 Respaldos` (y `⚙️ Configuración de Respaldo`) dentro de la sección **ADMINISTRACIÓN**.
- `Presentacion/MainWindow.xaml.cs`: handlers `MenuItem_Click_Backup` / `MenuItem_Click_BackupConfiguracion` que resuelven los `Ctl*` del DI y los muestran (`AgregarPagina`).
- `AplicarRestriccionesPorRol()`: incluir la visibilidad de ambos ítems en el gate de `secAdministracion` y mostrar/ocultar según `Configuracion.Backup.Gestionar`.

### 7. DI y paquetes

- `App.xaml.cs`: registrar `IConfiguracionBackupRepository`, `IBackupConfiguracionService`, `IDatabaseBackupService`, `IDriveBackupService`, `BackupViewModel`, `BackupConfiguracionViewModel`, `CtlBackup`, `CtlBackupConfiguracion` (`AddScoped`/`AddTransient`).
- `TP_ControlVehicular.csproj`: agregar `Google.Apis.Drive.v3`, `Google.Apis.Auth` y `System.Security.Cryptography.ProtectedData`; e incluir `credenciales.json` (si existe) con `CopyToOutputDirectory = PreserveNewest`.
- `.gitignore`: ignorar `credenciales.json`, `credentials.json` y `token.json`.

### 8. Orden de aplicación
1. Scripts `bd/21` y `bd/22` sobre `ControlVehicular`.
2. Paquetes NuGet + entidad + configuración EF + `DbSet`.
3. Repositorio + servicio de configuración + DTO + mapeo.
4. `IDatabaseBackupService` (backup / probar / listar).
5. `IDriveBackupService` (OAuth DPAPI / subir).
6. Pantallas + VMs + DI + menú + gating.
7. `dotnet build "TP_ControlVehicular.slnx"` y prueba manual de los 3 botones.

## Trade-offs / decisiones
- **Dos rutas (server vs UNC)**: es la consecuencia inevitable de que el `.bak` lo escribe el servicio SQL; asumir una sola ruta rompe el modelo central de 2 PCs.
- **Google Drive API en vez de rclone**: se evita distribuir un binario externo y su configuración; se paga con código OAuth y una dependencia NuGet, aceptable para el alcance.
- **Tabla singleton configurable en vez de constantes**: la ruta de respaldo y la carpeta Drive cambian por taller; deben configurarse sin recompilar.
- **Un archivo por día en Drive, sobrescribiendo el mismo día**: evita acumular cientos de archivos por reintentos y cumple el resguardo "fuera del equipo".
- **Restaurar en `_Test` para "probar"**: es la única verificación real de que el `.bak` es utilizable; se aísla en `ControlVehicular_Test` para no tocar jamás la base productiva.
- **Permisos SQL del login fuera de alcance**: la app necesita `db_backupoperator` (backup) y `dbcreator` (crear/dropear/restaurar `_Test`); se resuelven en despliegue (o ya se cumplen si el usuario instaló el SQL Express como `sysadmin`).
- **Scripts idempotentes en vez de migraciones EF**: coincide con cómo se gestiona hoy el esquema real del proyecto.
