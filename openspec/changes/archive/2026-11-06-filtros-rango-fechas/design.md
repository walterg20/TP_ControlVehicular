# Design: Filtros por Rango de Fechas en Listados y Reportes

## Architectural Context
The presentation layer uses WPF with MVVM pattern (`BaseViewModel`, `RelayCommand`, `ICollectionView` / `ObservableCollection<T>`).
Reports are generated using QuestPDF with standardized components (`ComposeEncabezadoTaller`, `ComposePieDePagina`, and `EmpresaBranding` logo resolver).

## Technical Strategy

### 1. `OrdenServicioViewModel` & `CtlOrdenServicio.xaml`
- Properties:
  - `FechaDesde`: `DateTime?` (default: `new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)`)
  - `FechaHasta`: `DateTime?` (default: `DateTime.Today`)
  - `EstadoFiltro`: `string` (default: `"Todos"`)
  - `MensajeValidacionFechas`: `string`
- Filtering logic in `ListadoOrdenesFiltered`:
  ```csharp
  var query = Ordenes.AsEnumerable();
  if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde.Value.Date > FechaHasta.Value.Date)
  {
      MensajeValidacionFechas = "La fecha 'Desde' no puede ser posterior a 'Hasta'.";
      return Enumerable.Empty<RegistroServicioDto>();
  }
  MensajeValidacionFechas = string.Empty;
  if (FechaDesde.HasValue) query = query.Where(o => o.Fecha.Date >= FechaDesde.Value.Date);
  if (FechaHasta.HasValue) query = query.Where(o => o.Fecha.Date <= FechaHasta.Value.Date);
  if (!string.IsNullOrWhiteSpace(EstadoFiltro) && EstadoFiltro != "Todos")
      query = query.Where(o => string.Equals(o.Estado, EstadoFiltro, StringComparison.OrdinalIgnoreCase));
  if (!string.IsNullOrWhiteSpace(TextoBusqueda))
      query = query.Where(o => /* text matches */);
  ```
- PDF Generation in `ImprimirListadoFiltradoAsync()`:
  - Generates A4 Landscape document with `ReporteExtensions.ComposeEncabezadoTaller("Listado de Órdenes de Trabajo")`
  - Subtitle with active filters (period, status, search)
  - Table containing the filtered collection with columns: N°, Fecha, Cliente, Patente, Vehículo, Recepción, Km, Estado.
  - Footer with `ReporteExtensions.ComposePieDePagina()`.

### 2. `MisTrabajosViewModel` & `CtlMisTrabajos.xaml`
- Add `FechaDesde` and `FechaHasta` properties.
- In `OrdenesFilter(object item)`:
  - Validate `orden.Fecha.Date >= FechaDesde.Value.Date` and `orden.Fecha.Date <= FechaHasta.Value.Date`.

### 3. `ReporteOrdenesViewModel` & `CtlReporteOrdenes.xaml`
- Set default period to current month (`new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)` to `DateTime.Today`).
- Add `LimpiarFiltrosCommand` to reset filters.
- Add `MensajeValidacionFechas` for visual alert when `FechaDesde > FechaHasta`.
