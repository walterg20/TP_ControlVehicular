# Design: Recuperación de Base de Datos con Doble Factor

## Context

El módulo de respaldos (`BackupViewModel`, `DatabaseBackupService`, `CtlBackup`) ya implementa
generación, prueba y resguardo en Drive. La restauración productiva es la contraparte destructiva
que faltaba. Se reutiliza la infraestructura existente (conexión SQL, nombres lógicos, carpeta de datos)
y se agrega el segundo factor vía `IUsuarioRepository`.

## Goals / Non-Goals

**Goals:**
- Restaurar la BD productiva desde un `.bak` seleccionado.
- Exigir segundo factor (otro usuario + clave) con permiso de gestión.
- Reusar código existente (`ObtenerNombresLogicosAsync`, `ObtenerCarpetaDatosServidorAsync`, `Escapar`).
- Conectar a `master` para evitar "database in use".

**Non-Goals:**
- No automatizar reinicio de la aplicación (solo sugerir).
- No agregar migraciones ni cambios de esquema.
- No introducir un permiso nuevo; se reusa `Configuracion.Backup.Gestionar`.

## Decisions

### 1. Verificación del segundo factor en el servicio (Negocio)
La validación de credenciales del autorizador se hace en `DatabaseBackupService.RestaurarAsync`,
no en el VM, para mantener la lógica de negocio centralizada y testeable. El servicio recibe
`IUsuarioRepository` por constructor.

### 2. Conexión a `master`
Se construye un connection string derivado con `SqlConnectionStringBuilder.InitialCatalog = "master"`
para ejecutar `RESTORE` sin conflicto con la BD productiva.

### 3. Modal de segundo factor
Se crea `FrmAutorizacionBackup` (XAML + code-behind) siguiendo la convención de formularios modales
(`Frm*.xaml`, Grid Margin=24, 3 filas, botones Aceptar/Cancelar). Expone `DniAutorizador` y
`ContrasenaAutorizador` como propiedades públicas.

### 4. Flujo del VM
`RestaurarBackupAsync`: validar permiso/selección → confirmar (FrmConfirmacion) → abrir modal →
llamar servicio → mostrar resultado. El modal se instancia directamente (no DI).

### 5. Seguridad
- Autorizador ≠ usuario de sesión (`UserSession.CurrentUser`).
- Autorizador activo, contraseña coincidente, rol con permiso `Configuracion.Backup.Gestionar`.
- Si falla la validación, se lanza `InvalidOperationException` con mensaje claro; el VM la captura y muestra.

## Risks / Trade-offs

- La restauración sobrescribe la BD productiva; la app debe reiniciarse para que EF re-conecte.
  Mitigación: mensaje de éxito sugiere reinicio.
- Contraseñas en texto plano (consistente con el sistema actual); no se cambia en este spec.
- `SET SINGLE_USER WITH ROLLBACK IMMEDIATE` cierra conexiones activas; puede interrumpir otras
  operaciones en curso. Mitigación: confirmación explícita antes de ejecutar.

## Migration Plan

No requiere migraciones de BD ni cambios de esquema. Solo código y XAML. Rollback: revertir el commit.