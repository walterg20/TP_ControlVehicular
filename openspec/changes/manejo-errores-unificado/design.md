# Design: Manejo unificado de errores silenciosos

## Decisions
- Notifier centralizado con evento `ErrorOccurred(string mensaje, Exception? ex)`. Los VMs/code-behind llaman `_errorNotifier.Notify(ex, contexto)`; no se cambia el flujo (returns/seguimiento).
- `MainWindow.MostrarError(mensaje, modal=false)`: toast 3s por defecto; modal `FrmConfirmacion.MostrarAviso` si `modal=true`.
- Mensaje amigable al usuario; detalle técnico en `Debug.WriteLine` + log en `%AppData%/TP_ControlVehicular/logs/error.log`.
- El notifier se inyecta en VMs y code-behind donde sea posible; los code-behind acceden vía `App.ServiceProvider`.
- Global handlers en `App.OnStartup` registran excepciones no manejadas.
- Toast por defecto para errores no críticos; modal para errores de guardado/operación crítica (parametrizable).

## Risks
- Ruido: notificar cada catch podría generar spam; se deduplica por contexto+mensaje en ventana de 2s.
- Thread-safety: el evento puede dispararse desde hilos no-UI; `MainWindow` hace `Dispatcher.Invoke`.
- No romper tests: el notifier es opcional (default null) en constructores de VMs.