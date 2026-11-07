# Proposal: Manejo unificado de errores silenciosos

## Why
Muchos bloques `catch` del proyecto tragan excepciones silenciosamente (`catch { }`, `catch` sin cuerpo, `Debug.WriteLine` sin notificar). El usuario final no recibe feedback cuando falla una operación (carga, guardado, consulta), lo que dificulta el soporte y genera confusión. Se necesita un mecanismo centralizado que notifique de forma unificada (toast/modal) todos los errores silenciosos.

## What Changes
- Servicio `IErrorNotifier`/`ErrorNotifierService` con `Notify(Exception?, contexto)` que registra y dispara evento `ErrorOccurred`.
- Registro global en `App` (`DispatcherUnhandledException`, `TaskScheduler.UnobservedTaskException`, `AppDomain.UnhandledException`).
- `MainWindow.MostrarError(mensaje, modal)` unificado (toast por defecto, modal opcional) suscrito al evento.
- Auditoría y reemplazo de catch silenciosos en ViewModels (Fase A), code-behind (Fase B) y servicios (Fase C), manteniendo el flujo original.
- Registro DI del notifier.

## Capabilities
### New Capabilities
- `manejo-errores-unificado`: notificación centralizada de errores silenciosos vía toast/modal.

## Impact
- Archivos nuevos: `Negocio/Services/IErrorNotifier.cs`, `Negocio/Services/ErrorNotifierService.cs`.
- Archivos modificados: ~40 (VMs, code-behind, App, MainWindow, servicios).
- Tests: nuevos en `TP_ControlVehicular.Tests/Services` y ajustes.