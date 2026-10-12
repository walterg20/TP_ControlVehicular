# Tasks: Respaldo de Base de Datos con Resguardo en Google Drive

- [x] 1. Esquema y permisos (BD)
  - [x] 1.1 Crear `bd/21_ConfiguracionBackup.sql` (tabla `ConfiguracionBackup` + fila semilla, idempotente).
  - [x] 1.2 Crear `bd/22_PermisosBackup.sql` (permiso `Configuracion.Backup.Gestionar` → Admin `IdRol = 1`, idempotente).
  - [x] 1.3 Ejecutar ambos scripts sobre `ControlVehicular`.

- [x] 2. Paquetes, capa de datos
  - [x] 2.1 Agregar a `TP_ControlVehicular.csproj`: `Google.Apis.Drive.v3`, `Google.Apis.Auth`, `System.Security.Cryptography.ProtectedData`.
  - [x] 2.2 Crear `Entidad/ConfiguracionBackup.cs`.
  - [x] 2.3 Crear `Datos/Configuracion/ConfiguracionBackupConfiguration.cs` (patrón `ConfiguracionEmpresaConfiguration`).
  - [x] 2.4 Agregar `DbSet<ConfiguracionBackup>` en `Datos/Data/CVDbContext.cs`.
  - [x] 2.5 Crear `Negocio/Interfaces/IConfiguracionBackupRepository.cs` (`ObtenerAsync`).
  - [x] 2.6 Crear `Datos/Repositories/ConfiguracionBackupRepository.cs`.

- [x] 3. Negocio (configuración)
  - [x] 3.1 Crear `Negocio/DTOs/BackupConfiguracionDto.cs`.
  - [x] 3.2 Agregar mapeo `ConfiguracionBackup` → `BackupConfiguracionDto` en `MappingProfile.cs`.
  - [x] 3.3 Crear `Negocio/Interfaces/IBackupConfiguracionService.cs`.
  - [x] 3.4 Crear `Negocio/Services/BackupConfiguracionService.cs` (`ObtenerAsync`, `GuardarAsync`).

- [x] 4. Servicio de base de datos (`IDatabaseBackupService`)
  - [x] 4.1 Crear DTOs `BackupResultDto`, `BackupTestResultDto`, `BackupArchivoDto`.
  - [x] 4.2 Crear `Negocio/Interfaces/IDatabaseBackupService.cs`.
  - [x] 4.3 Implementar `HacerBackupAsync` (`BACKUP ... WITH INIT, COMPRESSION, CHECKSUM` + `RESTORE VERIFYONLY`).
  - [x] 4.4 Implementar `ProbarBackupAsync` (restore a `ControlVehicular_Test` con `FILELISTONLY` + `MOVE` + `REPLACE, RECOVERY`).
  - [x] 4.5 Implementar `ListarAsync` (escaneo de `*.bak` en la carpeta efectiva).

- [x] 5. Servicio de nube (`IDriveBackupService`)
  - [x] 5.1 Crear `Negocio/DTOs/DriveUploadResultDto.cs` y `Negocio/Interfaces/IDriveBackupService.cs`.
  - [x] 5.2 Implementar OAuth `drive.file` con token de refresco cifrado con DPAPI (por-PC).
  - [x] 5.3 Implementar `SubirUltimoAsync` (un archivo por día; sobrescribe si ya existe; solo si no se subió).
  - [x] 5.4 Actualizar `UltimaSubidaArchivo`/`FechaUltimaSubida` en la configuración al subir.
  - [x] 5.5 Leer credenciales OAuth de `credenciales.json` (copiado a la salida por el `.csproj`, gitignored) y auto-crear/buscar la carpeta `ControlVehicularBackups` cuando no hay `DriveFolderId`.
  - [x] 5.6 Actualizar el mini tutorial `docs/referencia/backup-configuracion.md` (archivo de credenciales y carpeta automática).

- [x] 6. Presentación (pantallas, VMs, menú, DI)
  - [x] 6.1 Crear `Presentacion/Pantalla/Configuracion/CtlBackup.xaml` + `.xaml.cs`.
  - [x] 6.2 Crear `Presentacion/Pantalla/Configuracion/CtlBackupConfiguracion.xaml` + `.xaml.cs`.
  - [x] 6.3 Crear `Presentacion/ViewModels/BackupViewModel.cs`.
  - [x] 6.4 Crear `Presentacion/ViewModels/BackupConfiguracionViewModel.cs`.
  - [x] 6.5 Registrar repositorio, servicios, VMs y pantallas en `App.xaml.cs`.
  - [x] 6.6 Agregar ítems **Respaldos**/**Configuración de Respaldo** y navegación en `MainWindow.xaml`/`.xaml.cs`.
  - [x] 6.7 Gate por permiso `Configuracion.Backup.Gestionar` en `AplicarRestriccionesPorRol()`.

- [ ] 7. Verificación y cierre
  - [x] 7.1 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [x] 7.2 Probar: Admin ve la pantalla, hace backup, prueba el backup (`_Test` online) y conecta Drive; Recepcionista/Mecánico NO ven la opción.
  - [x] 7.3 Actualizar `docs/referencia/bd-y-dominio.md` (tabla y permiso nuevos).
  - [x] 7.4 Actualizar `MEMORY.md` y crear `docs/plan/2026-10-11_backup-drive.md`.
