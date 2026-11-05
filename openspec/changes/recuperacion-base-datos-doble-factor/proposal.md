# Proposal: Recuperación de Base de Datos con Doble Factor

## Why

El sistema ya permite generar, probar y resguardar respaldos de la base de datos,
pero no existe una operación para **recuperar** (restaurar) la base de datos productiva
a partir de un respaldo `.bak`. Esta operación es crítica y destructiva: sobrescribe
la base `ControlVehicular` con el contenido del respaldo elegido. Por eso debe estar
protegida por un segundo factor de autorización: un **otro usuario** (distinto al
administrador que ejecuta la acción) con credenciales válidas y permisos de gestión
de respaldos debe autorizarla.

## What Changes

- Agregar el método `RestaurarAsync(archivoBak, dniAutorizador, contrasenaAutorizador)`
  a `IDatabaseBackupService` y su implementación en `DatabaseBackupService`.
- La restauración se ejecuta contra la base productiva (`ControlVehicular`), conectándose
  a `master`, poniendo la BD en `SINGLE_USER`, ejecutando `RESTORE DATABASE ... WITH REPLACE, RECOVERY`
  y volviendo a `MULTI_USER`.
- Agregar la propiedad `PuedeRestaurar` y el comando `RestaurarBackupCommand` en `BackupViewModel`,
  con el flujo: confirmación → modal de segundo factor → llamada al servicio → resultado.
- Crear el formulario modal `FrmAutorizacionBackup` (DNI + contraseña) en
  `Presentacion/Pantalla/Configuracion/`.
- Agregar el botón "🔁 Restaurar Backup" en `CtlBackup.xaml`.
- El autorizador debe ser un usuario distinto al de la sesión, activo, con contraseña
  correcta y con el permiso `Configuracion.Backup.Gestionar` en su rol.

## Capabilities

### New Capabilities
- `restauracion-base-datos`: restauración de la base de datos productiva desde un respaldo
  seleccionado, con doble factor de autorización.

### Modified Capabilities
- `backup-base-datos`: se agrega la acción "Restaurar Backup" a la pantalla de Respaldos y
  el requirement de doble factor para la restauración.

## Impact

- **Código**: `Negocio/Services/DatabaseBackupService.cs`, `Negocio/Interfaces/IDatabaseBackupService.cs`,
  `Presentacion/ViewModels/BackupViewModel.cs`, nuevo `FrmAutorizacionBackup.xaml/.cs`,
  `Presentacion/Pantalla/Configuracion/CtlBackup.xaml`.
- **DI**: sin cambios (el servicio ya está registrado; el modal se instancia directamente).
- **Base de datos**: operación destructiva sobre `ControlVehicular`; no requiere migraciones.
- **Seguridad**: segundo factor obligatorio; el autorizador no puede ser el mismo usuario de la sesión.