p = r"E:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\MEMORY.md"
s = open(p, encoding="utf-8").read()
start = s.find("<<<<<<< HEAD")
end = s.find(">>>>>>> feature/manejo-errores-unificado") + len(">>>>>>> feature/manejo-errores-unificado")
replacement = """- Rama de trabajo: `develop` (specs `ayuda-contextual-interactiva` y `manejo-errores-unificado` mergeadas).
- **Ayuda contextual interactiva en Orden de Servicio (spec `ayuda-contextual-interactiva`)**:
  - Servicio `IAyudaService`/`AyudaService` carga ayudas desde `Datos/Configuracion/ayudas_ordenes_servicio.json` (por rol, pantalla y control) y persiste preferencias de expansion en `%AppData%/TP_ControlVehicular/Ayudas`.
  - DTOs `AyudaContextoDto`, `AyudaPorRolDto`, `AyudaSeccionDto`. Control reutilizable `CtlAyudaContextual` (panel colapsable) con DependencyProperty `Secciones`.
  - `CtlOrdenServicioForm`: panel lateral colapsable (boton "Ayuda"/"Ocultar ayuda"), carga de ayuda al cambiar foco entre controles, rol actual desde la sesion.
  - `OrdenServicioViewModel`: propiedad `AyudaActual`, metodo `CargarAyudaPorControl`, `EstablecerRolActual`.
  - Registro DI en `App.xaml.cs`. JSON copiado al output como Content. Textos en UTF-8 correcto.
- **Manejo unificado de errores silenciosos (spec `manejo-errores-unificado`)**:
  - Servicio `IErrorNotifier`/`ErrorNotifierService` con `Notify(ex, contexto, modal)`; dedup 2s; log a `%AppData%/TP_ControlVehicular/logs/error.log`; evento `ErrorOccurred`.
  - Registro global en `App` (`DispatcherUnhandledException`, `TaskScheduler.UnobservedTaskException`, `AppDomain.UnhandledException`).
  - `MainWindow.MostrarError(mensaje, modal)` unificado (toast por defecto, modal opcional) suscrito al evento.
  - Auditoria de ~59 catch silenciosos en code-behind + VMs: ahora notifican via el notifier. Catch con `Debug.WriteLine` tambien notifican."""
s = s[:start] + replacement + s[end:]
open(p, "w", encoding="utf-8").write(s)
print("done")
PYEOF