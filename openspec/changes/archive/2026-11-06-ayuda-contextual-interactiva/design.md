# Design: Ayuda contextual interactiva en Orden de Servicio

## Decisions
- El panel de ayuda es **colapsable** (botón "Ayuda"/"Ocultar ayuda" que muestra/oculta el panel de 350 px a la derecha).
- Ayuda solo en la pantalla Orden de Servicio (no se extiende a otros CRUD por ahora).
- Los nombres de control en el JSON deben coincidir con los `x:Name` reales del XAML (`ctlBusquedaUniversal`, `cmbVehiculos`, `cmbServicios`, `txtKm`, `cmbEstado`, `cmbTalleres`, `dgChecklist`).
- `IAyudaService` se inyecta como dependencia opcional en `OrdenServicioViewModel` (default null) para no romper los tests existentes.
- El rol actual se establece en `CtlOrdenServicioForm.ConfigurarViewModel` a partir de `MainWindow.UsuarioSesionActual.IdRol`.
- El JSON se copia al directorio de salida como `Content` con `PreserveNewest`.
- Persistencia de preferencias de expansión por rol/ayuda en `%AppData%/TP_ControlVehicular/Ayudas`.

## Risks
- El panel colapsable reduce el ancho disponible del formulario; usar `Grid.ColumnDefinitions` con `Width="*"` y `Width="Auto"` y un `ToggleButton`/`Button` para mostrar/ocultar.
- `CargarAyudaPorControl` es `async void`; capturar excepciones y no propagarlas (ya implementado).
- Textos del JSON con caracteres de reemplazo: reescribir en UTF-8 correcto.