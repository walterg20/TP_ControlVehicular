# Design: Managerial Report — Explicit Query, Service Breakdown Pie Chart, and Cleanup

## Context
The managerial report is rendered by `CtlReporteGerencial` (a `UserControl` hosted by `MainWindow`) with its logic in `ReporteGerencialViewModel`. Data access goes through `IReporteGerencialRepository` (Dapper + `SqlConnection`), which today exposes:
- `ObtenerIngresosPorFechaAsync(FechaDesde, FechaHasta)` → `sp_ReporteIngresosAdmin` → `ReporteIngresoDto { Fecha, CantidadFacturas, IngresosTotales }`
- `ObtenerTopClientesAsync(FechaDesde, FechaHasta, TopN)` → `sp_ReporteTopClientes` → `ReporteTopClienteDto`
- `ObtenerModelosMasReparadosAsync(FechaDesde, FechaHasta, TopN)` → `ReporteModeloReparadoDto`

Existing patterns to reuse:
- PDFs are built with QuestPDF **2026.9.1**; every document header uses `ReporteExtensions.ComposeEncabezadoTaller(title)` (which draws the logo) and footer `ComposePieDePagina()`.
- Report stored procedures `CREATE OR ALTER`, alias every column to the exact DTO property name, and live in `bd/` as numbered idempotent scripts (next free number: `18`).
- `RegistroServicio.Estado` ∈ {`Abierta`,`En Proceso`,`Completada`,`Pagada`,`Cancelada`}; `DetalleServicio.Estado` ∈ {`Pendiente`,`En Curso`,`Finalizada`}. Aggregate quantities at the `DetalleServicio` level and exclude cancelled orders.

## Goals / Non-Goals
- Goals: explicit query, service-quantity breakdown (table + PDF pie chart), correct model name, remove dead view.
- Non-Goals: on-screen charting (WPF), new permissions, changing the income metric, Excel export.

## Decisions

### D1 — Explicit query (A2)
- Add a `Consultar` button in `CtlReporteGerencial.xaml` bound to the existing `CargarReporteCommand` (currently only triggered from date changes).
- Remove `_ = LoadAsync()` from the `FechaDesde` and `FechaHasta` setters so changing a date does not query.
- Guard in the load path: if `FechaDesde > FechaHasta`, set a validation message (`MensajeValidacion`) and return without querying.
- Rationale: matches the spec scenarios and removes redundant DB round-trips.

### D2 — Service quantity breakdown (A3)
- **Database**: new `bd/18_SP_ReporteServiciosPorRango.sql`, `CREATE OR ALTER PROCEDURE sp_ReporteServiciosPorRango (@FechaDesde DATETIME, @FechaHasta DATETIME)`:
  ```sql
  SELECT s.Id     AS ServicioId,
         s.Nombre AS Servicio,
         SUM(ds.Cantidad) AS Cantidad
  FROM DetalleServicio ds
  INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
  INNER JOIN Servicio s          ON ds.ServicioId = s.Id
  WHERE rs.Estado <> 'Cancelada'
    AND (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
    AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
  GROUP BY s.Id, s.Nombre
  HAVING SUM(ds.Cantidad) > 0
  ORDER BY Cantidad DESC, s.Nombre ASC;
  ```
  (Column names verified against the real schema during implementation; aliases match the DTO.)
- **DTO**: `Negocio/DTOs/Reportes/ReporteServicioCantidadDto.cs` with `ServicioId`, `Servicio`, `Cantidad`.
- **Repository**: add `Task<IEnumerable<ReporteServicioCantidadDto>> ObtenerServiciosPorRangoAsync(DateTime desde, DateTime hasta)` to `IReporteGerencialRepository` and implement it with `QueryAsync<ReporteServicioCantidadDto>` using `DbType.Date` parameters, mirroring the existing methods.
- **ViewModel**: new get-only `ObservableCollection<ReporteServicioCantidadDto> ServiciosPorRango { get; } = new();`, populated inside `LoadAsync` alongside the other three calls.
- **Screen**: a fourth `TabItem` "Servicios" with a `DataGrid` bound to `ServiciosPorRango` (Servicio / Cantidad), plus an "Imprimir PDF" button.

### D3 — PDF pie chart (A4)
- New `Negocio/Reportes/Documentos/ServiciosPieChartGenerator.cs` with a method `Compose(IContainer container, IReadOnlyList<ReporteServicioCantidadDto> data)` that renders:
  - a legend (service name, quantity, percentage), and
  - a pie chart drawn with QuestPDF's `Canvas` (`IContainer.Canvas((canvas, size) => ...)`) using SkiaSharp `SKPath` arcs (`ArcTo`) and a fixed color palette; slices are sized by `Cantidad / total`.
- The chart is only composed inside the PDF document (`ComposeServiciosContent`), never in the WPF view.
- The document uses `page.Header().Element(c => c.ComposeEncabezadoTaller("Reporte Gerencial — Servicios"))` and `page.Footer().Element(c => c.ComposePieDePagina())`, satisfying the logo rule.
- Output: PDF written to a temp path following the existing `Reporte*_{timestamp}.pdf` convention and opened with the same helper used by the other print commands.

### D4 — Model name fix (A1)
- `ReporteModeloReparadoDto.MarcaModelo` currently returns the literal `"{Marca} {Modelo}"` (missing `$`). Change to `=> $"{Marca} {Modelo}"`.
- No schema or query change.

### D5 — Remove dead view (A5)
- Delete `Presentacion/Pantalla/Reporte/ReporteGerencialView.xaml` and `.xaml.cs`.
- Remove the single DI registration `services.AddScoped<ReporteGerencialView>();` from `App.xaml.cs`. Keep `ReporteGerencialViewModel` and `CtlReporteGerencial`.

## Risks / Trade-offs
- **Chart dependency**: `Canvas` relies on SkiaSharp available transitively through QuestPDF. If `Canvas` is unavailable in this QuestPDF build, fall back to laying out slices with QuestPDF primitives (`Svg`, or a `Row`/`Column` legend plus a simple bar fallback); the spec allows any pie-chart rendering in the PDF.
- **Date boundary**: `CAST(rs.Fecha AS DATE)` between the range bounds includes the whole end day regardless of time component, matching the other report stored procedures.
- **Encoding**: XAML/VM edits must preserve UTF-8 (accents, emoji). Use the `.NET` write APIs, never `Set-Content`/`>`.
- **No data**: when the range has no services, the tab shows empty and the PDF omits the chart (no demo data).

## Migration Plan
- Apply `bd/18_SP_ReporteServiciosPorRango.sql` idempotently (`CREATE OR ALTER`), safe to re-run.
- No data migration; the new measurement reads existing tables.

## Open Questions
- (none)
