# Tasks: Recuperación de Base de Datos con Doble Factor

## 1. Negocio — Interfaz y servicio

- [x] 1.1 Agregar `RestaurarAsync(...)` a `IDatabaseBackupService`.
- [x] 1.2 Inyectar `IUsuarioRepository` en `DatabaseBackupService`.
- [x] 1.3 Implementar `RestaurarAsync` con validación del segundo factor y restore productivo.
- [x] 1.4 Reusar `ObtenerNombresLogicosAsync`, `ObtenerCarpetaDatosServidorAsync`, `Escapar`.

## 2. Presentación — ViewModel

- [x] 2.1 Agregar propiedad `PuedeRestaurar`.
- [x] 2.2 Agregar `RestaurarBackupCommand`.
- [x] 2.3 Implementar `RestaurarBackupAsync()` (confirmación + modal + servicio + resultado).
- [x] 2.4 Actualizar `NotificarComandos` para el nuevo comando.

## 3. UI — Modal de segundo factor

- [x] 3.1 Crear `FrmAutorizacionBackup.xaml` y code-behind en `Presentacion/Pantalla/Configuracion/`.
- [x] 3.2 Exponer propiedades `DniAutorizador` y `ContrasenaAutorizador`.
- [x] 3.3 Validar campos no vacíos al Aceptar.

## 4. UI — Pantalla de Respaldos

- [x] 4.1 Agregar botón "🔁 Restaurar Backup" en `CtlBackup.xaml`.
- [x] 4.2 Actualizar texto del pie de la pantalla.

## 5. Verificación

- [x] 5.1 Compilar: `dotnet build "TP_ControlVehicular.slnx"` sin errores.
- [ ] 5.2 Prueba manual: seleccionar backup → Restaurar → segundo factor → verificar restauración y mensaje de reinicio.