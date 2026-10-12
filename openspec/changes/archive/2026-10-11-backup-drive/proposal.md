# Proposal: Respaldo de Base de Datos con Resguardo en Google Drive

## Why
Hoy la base `ControlVehicular` **no tiene ningún mecanismo de respaldo** desde la aplicación: no existe pantalla, ni tarea agendada, ni copia externa. El despliegue objetivo son **2 PCs de un taller conectadas a un SQL Server central** (sin LocalDB), donde una falla del servidor o un error humano (borrado o actualización masiva) dejaría el sistema sin forma de recuperación. SQL Server 2019 **Express** no incluye Agente, por lo que no hay respaldos automáticos por defecto.

Esta entrega incorpora una pantalla de administración de respaldos que permite, desde la app: **generar un backup completo**, **verificar ese backup restaurándolo** en una base de prueba, y **resguardarlo en la nube (Google Drive)** para tener una copia fuera del equipo.

## What Changes
- Nueva entidad singleton `ConfiguracionBackup` (una sola fila, `Id = 1`) con la configuración del respaldo: nombre de la base, ruta del servidor (donde el servicio SQL escribe el `.bak`), ruta UNC (que lee la app), carpeta destino de Google Drive y último archivo subido.
- Nuevos scripts idempotentes: `bd/21_ConfiguracionBackup.sql` (tabla + fila semilla) y `bd/22_PermisosBackup.sql` (permiso `Configuracion.Backup.Gestionar` → Administrador).
- Repositorio `IConfiguracionBackupRepository` + servicio `IBackupConfiguracionService` para leer/guardar la configuración.
- **Servicio de base de datos** `IDatabaseBackupService`: `BACKUP DATABASE ... WITH INIT, COMPRESSION, CHECKSUM` + `RESTORE VERIFYONLY`; listado de `.bak` escaneando la carpeta; y **prueba** que restaura el `.bak` en `ControlVehicular_Test` (`RESTORE FILELISTONLY` + `MOVE` + `WITH REPLACE, RECOVERY`) y verifica que la base de prueba quede online con filas en tablas clave.
- **Servicio de nube** `IDriveBackupService` sobre **Google Drive API** (`Google.Apis.Drive.v3`): autenticación OAuth con scope `drive.file`, token de refresco guardado por-PC y cifrado con DPAPI; sube el último `.bak` con nombre `ControlVehicular_YYYYMMDD_FULL.bak` (re-subir el mismo día sobrescribe).
- **Dos pantallas** (UserControl + ViewModel): **Respaldos** (listado + 3 botones: Hacer backup / Probar backup / Resguardar en Drive) y **Configuración de Respaldo** (rutas, base, carpeta Drive, conexión con Google).
- Ítem de menú y gating por permiso `Configuracion.Backup.Gestionar` (solo Administrador).

## Impact
- **Capacidad nueva**: `backup-base-datos`.
- **Código afectado**: `Entidad/`, `Datos/Data/CVDbContext.cs`, `Datos/Configuracion/`, `Datos/Repositories/`, `Negocio/Interfaces/`, `Negocio/DTOs/`, `Negocio/Services/`, `Negocio/Mappers/MappingProfile.cs`, `Presentacion/Pantalla/Configuracion/`, `Presentacion/ViewModels/`, `Presentacion/MainWindow.xaml`/`.xaml.cs`, `App.xaml.cs`, `TP_ControlVehicular.csproj` (paquetes NuGet).
- **Base de datos**: alta de tabla `ConfiguracionBackup`, fila semilla y permiso `Configuracion.Backup.Gestionar`. Las operaciones `BACKUP`/`RESTORE` las ejecuta el **servicio SQL** (la ruta del `.bak` es server-side).
- **Dependencias nuevas**: `Google.Apis.Drive.v3`, `Google.Apis.Auth`, `System.Security.Cryptography.ProtectedData`.

## Non-goals (fuera de alcance de esta fase)
- **Cifrado** del archivo `.bak`.
- **Agendado/automatización** del respaldo (Task Scheduler + `sqlcmd`): se evaluará por separado.
- **Permisos SQL del login** (`db_backupoperator` / `dbcreator`) o SPs firmados por certificado: se resuelven en la fase de despliegue, no en la configuración de la app.
- **Backup diferencial/logarítmico** y retención GFS: fase posterior.
- **Instalador / alta automática de base y datos por defecto**: aspiracional, no se implementa aquí.
