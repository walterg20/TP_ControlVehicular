# Tasks: Respaldo de Base de Datos con Resguardo en Google Drive

- [ ] 1. Esquema y permisos (BD)
  - [ ] 1.1 Crear `bd/21_ConfiguracionBackup.sql` (tabla `ConfiguracionBackup` + fila semilla, idempotente).
  - [ ] 1.2 Crear `bd/22_PermisosBackup.sql` (permiso `Configuracion.Backup.Gestionar` → Admin `IdRol = 1`, idempotente).
  - [ ] 1.3 Ejecutar ambos scripts sobre `ControlVehicular`.

- [ ] 2. Paquetes, capa de datos
  - [ ] 2.1 Agregar a `TP_ControlVehicular.csproj`: `Google.Apis.Drive.v3`, `Google.Apis.Auth`, `System.Security.Cryptography.ProtectedData`.
  - [ ] 2.2 Crear `Entidad/ConfiguracionBackup.cs`.
  - [ ] 2.3 Crear `Datos/Configuracion/ConfiguracionBackupConfiguration.cs` (patrón `ConfiguracionEmpresaConfiguration`).
  - [ ] 2.4 Agregar `DbSet<ConfiguracionBackup>` en `Datos/Data/CVDbContext.cs`.
  - [ ] 2.5 Crear `Negocio/Interfaces/IConfiguracionBackupRepository.cs` (`ObtenerAsync`).
  - [ ] 2.6 Crear `Datos/Repositories/ConfiguracionBackupRepository.cs`.

- [ ] 3. Negocio (configuración)
  - [ ] 3.1 Crear `Negocio/DTOs/BackupConfiguracionDto.cs`.
  - [ ] 3.2 Agregar mapeo `ConfiguracionBackup` → `BackupConfiguracionDto` en `MappingProfile.cs`.
  - [ ] 3.3 Crear `Negocio/Interfaces/IBackupConfiguracionService.cs`.
  - [ ] 3.4 Crear `Negocio/Services/BackupConfiguracionService.cs` (`ObtenerAsync`, `GuardarAsync`).

- [ ] 4. Servicio de base de datos (`IDatabaseBackupService`)
  - [ ] 4.1 Crear DTOs `BackupResultDto`, `BackupTestResultDto`, `BackupArchivoDto`.
  - [ ] 4.2 Crear `Negocio/Interfaces/IDatabaseBackupService.cs`.
  - [ ] 4.3 Implementar `HacerBackupAsync` (`BACKUP ... WITH INIT, COMPRESSION, CHECKSUM` + `RESTORE VERIFYONLY`).
  - [ ] 4.4 Implementar `ProbarBackupAsync` (restore a `ControlVehicular_Test` con `FILELISTONLY` + `MOVE` + `REPLACE, RECOVERY`).
  - [ ] 4.5 Implementar `ListarAsync` (escaneo de `*.bak` en la carpeta efectiva).

- [ ] 5. Servicio de nube (`IDriveBackupService`)
  - [ ] 5.1 Crear `Negocio/DTOs/DriveUploadResultDto.cs` y `Negocio/Interfaces/IDriveBackupService.cs`.
  - [ ] 5.2 Implementar OAuth `drive.file` con token de refresco cifrado con DPAPI (por-PC).
  - [ ] 5.3 Implementar `SubirUltimoAsync` (un archivo por día; sobrescribe si ya existe; solo si no se subió).
  - [ ] 5.4 Actualizar `UltimaSubidaArchivo`/`FechaUltimaSubida` en la configuración al subir.

- [ ] 6. Presentación (pantallas, VMs, menú, DI)
  - [ ] 6.1 Crear `Presentacion/Pantalla/Configuracion/CtlBackup.xaml` + `.xaml.cs`.
  - [ ] 6.2 Crear `Presentacion/Pantalla/Configuracion/CtlBackupConfiguracion.xaml` + `.xaml.cs`.
  - [ ] 6.3 Crear `Presentacion/ViewModels/BackupViewModel.cs`.
  - [ ] 6.4 Crear `Presentacion/ViewModels/BackupConfiguracionViewModel.cs`.
  - [ ] 6.5 Registrar repositorio, servicios, VMs y pantallas en `App.xaml.cs`.
  - [ ] 6.6 Agregar ítems **Respaldos**/**Configuración de Respaldo** y navegación en `MainWindow.xaml`/`.xaml.cs`.
  - [ ] 6.7 Gate por permiso `Configuracion.Backup.Gestionar` en `AplicarRestriccionesPorRol()`.

- [ ] 7. Verificación y cierre
  - [ ] 7.1 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [ ] 7.2 Probar: Admin ve la pantalla, hace backup, prueba el backup (`_Test` online) y conecta Drive; Recepcionista/Mecánico NO ven la opción.
  - [ ] 7.3 Actualizar `docs/referencia/bd-y-dominio.md` (tabla y permiso nuevos).
  - [ ] 7.4 Actualizar `MEMORY.md` y crear `docs/plan/2026-10-11_backup-drive.md`.
