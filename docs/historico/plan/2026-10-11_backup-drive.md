# Plan — Respaldo de Base de Datos con Resguardo en Google Drive

- **Spec (OpenSpec):** `openspec/changes/2026-10-11-backup-drive` (capability `backup-base-datos`).
- **Rama:** `feature/backup-drive` (desde `develop`).
- **Fecha:** 2026-10-11.
- **Alcance:** solo la **configuración de la app** para backup → prueba (restore) → subida a Google Drive. Despliegue objetivo: 2 PCs de taller contra SQL Server central (sin LocalDB).

## Decisiones de diseño
- **Nube = Google Drive API directa** (`Google.Apis.Drive.v3`), **sin rclone**; scope `drive.file`.
- **Cifrado del `.bak`: NO** en esta fase.
- **Un archivo por día**; en Drive `ControlVehicular_YYYYMMDD_FULL.bak`; re-subir el mismo día **sobrescribe**.
- **Botón Resguardar**: sube siempre el último `.bak`, solo si no se subió todavía (compara `UltimaSubidaArchivo` + `FechaUltimaSubida` del mismo día).
- **Tabla por script en `bd/`** (estilo `19_ConfiguracionEmpresa.sql`), **no** por migración EF.
- **Dos rutas**: `RutaBackupServer` (server-side, escribe el `BACKUP`) y `RutaBackupUNC` (la app lee para listar/subir).
- **Refresh token por-PC con DPAPI** (`DpapiFileDataStore : IDataStore`), no en la tabla. ClientId/Secret embebidos (placeholders hasta tener los reales).
- `BACKUP`/`RESTORE` vía `Microsoft.Data.SqlClient.SqlConnection` directo (no `ExecuteSqlRaw`).

## Bloques de implementación y commits
| # | Bloque | Commit | Contenido |
|---|---|---|---|
| 1 | Esquema y permisos (BD) | `eb5a527` | `bd/21_ConfiguracionBackup.sql`, `bd/22_PermisosBackup.sql` (ejecutados en `ControlVehicular`). |
| 2 | Paquetes + capa de datos | `30a1800` | csproj (Google.Apis.Auth 1.77.0, Google.Apis.Drive.v3 1.77.0.4276, ProtectedData 10.0.12), `Entidad/ConfiguracionBackup.cs`, `ConfiguracionBackupConfiguration.cs`, `DbSet<>`, repositorio. |
| 3 | Negocio (configuración) | `46bbd36` | `BackupConfiguracionDto`, `IBackupConfiguracionService`, `BackupConfiguracionService`, `MappingProfile`. |
| 4 | Servicio de BD | `794d1ec` | DTOs `BackupResultDto`/`BackupTestResultDto`/`BackupArchivoDto`, `IDatabaseBackupService`, `DatabaseBackupService`. |
| 5 | Servicio de nube | `b7b5c42` | `DriveUploadResultDto`, `IDriveBackupService`, `DpapiFileDataStore`, `DriveBackupService`. |
| 6 | UI + VMs | `214b969` | `BackupViewModel`, `BackupConfiguracionViewModel`, `CtlBackup.xaml(.cs)`, `CtlBackupConfiguracion.xaml(.cs)`. |
| 7 | DI + menú + RBAC | `b1af2f7` | Registros en `App.xaml.cs`, botones `btnBackup`/`btnBackupConfiguracion` en `MainWindow.xaml`, handlers + gating por `Configuracion.Backup.Gestionar` + reset en `ResaltarBotonActivo`. |

## Verificación
- `dotnet build "TP_ControlVehicular.slnx"` → **0 errores** (warnings preexistentes).
- Prueba manual (Admin): ver pantalla, hacer backup, probar backup (`ControlVehicular_Test` online), conectar Drive. Recepcionista/Mecánico **no** ven la opción. *(Pendiente de ejecutar en el entorno con SQL Server.)*

## Pendientes / fase de despliegue
- Colocar los **ClientId/Secret reales** de Google Cloud (hoy placeholders) y **publicar** el consent screen.
- Definir **permisos SQL del login** (Windows Auth hoy; evaluar `db_backupoperator`/`dbcreator` o SPs firmados) y la **carpeta compartida UNC** del servidor.
- Fuera de alcance: cifrado del `.bak`, agendado (`Task Scheduler` + `sqlcmd`), backup diferencial/log, retención GFS, instalador.

## Cierre de spec
- `openspec validate 2026-10-11-backup-drive --strict` → valid.
- Actualizado `MEMORY.md`, `docs/referencia/backup.md`, `docs/referencia/bd-y-dominio.md` y tabla de dominio en `AGENTS.md`.
