# Design: Reporte Operativo — Task-Level Mechanic Productivity

## Context
`ReporteOperativoViewModel` currently derives productivity from `ObtenerReporteOrdenesHandler`, which returns one row per **order** and takes only the first detail's mechanic. Productivity is therefore wrong at two levels: mechanics beyond the first detail are invisible, and `Estado == "Finalizada"` is compared against an order state that never takes that value, so completions are always `0`.

## Goals
- Compute productivity from `DetalleServicio` rows, grouped by `UsuarioId`.
- Keep the order report (`Reporte de Órdenes`) order-granular; give the operational report its own source.
- Add a breakdown (Pendientes / En Curso / Completadas / % Avance) and totals.
- Stop fabricating demonstration rows in the order report.

## Non-Goals
- No drill-down into tasks.
- No change to the "Vehículos Más Frecuentes (Top 10)" section.
- No new permission (existing `Reporte.Operativo.Ver` already gates the screen).
- No change to `RegistroServicio` / `DetalleServicio` schemas.

## Decisions

### 1. Aggregation unit and source
New stored procedure `sp_ReporteProductividadMecanicos(@FechaDesde DATE = NULL, @FechaHasta DATE = NULL)` that groups **tasks**, not orders:

```sql
SELECT u.Id AS MecanicoId,
       u.Nombre + ' ' + u.Apellido AS MecanicoNombre,
       COUNT(*) AS TareasAsignadas,
       SUM(CASE WHEN ds.Estado = 'Pendiente'  THEN 1 ELSE 0 END) AS Pendientes,
       SUM(CASE WHEN ds.Estado = 'En Curso'    THEN 1 ELSE 0 END) AS EnCurso,
       SUM(CASE WHEN ds.Estado = 'Finalizada'  THEN 1 ELSE 0 END) AS Completadas
FROM DetalleServicio ds
INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
INNER JOIN Usuarios u ON ds.UsuarioId = u.Id
WHERE (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
  AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
GROUP BY u.Id, u.Nombre, u.Apellido
ORDER BY Completadas DESC, TareasAsignadas DESC;
```

Rationale:
- **INNER JOIN `Usuarios`**: a task with no valid mechanic (`UsuarioId` not resolving to a user) is not "productividad de un mecánico" and is excluded.
- **Date on `RegistroServicio.Fecha`**: details have no own date; the order date is the report time axis (same as the order report's `FechaIngreso`).
- **Only mechanics with tasks**: the `GROUP BY` naturally omits mechanics with no rows; no `0`-row mechanics are listed.
- **`CREATE OR ALTER`**: idempotent, matching `bd/` conventions.
- SQL is placed in a new file `bd/13_SP_ReporteProductividadMecanicos.sql` (last existing script is `12_`).

### 2. DTO
New `Negocio/DTOs/Reportes/ProductividadMecanicoDto.cs`:

```csharp
public class ProductividadMecanicoDto
{
    public int MecanicoId { get; set; }
    public string MecanicoNombre { get; set; } = string.Empty;
    public int TareasAsignadas { get; set; }
    public int Pendientes { get; set; }
    public int EnCurso { get; set; }
    public int TareasCompletadas { get; set; }
    public double PorcentajeAvance => TareasAsignadas == 0
        ? 0
        : Math.Round(TareasCompletadas * 100.0 / TareasAsignadas, 0);
    public string PorcentajeAvanceTexto => $"{PorcentajeAvance:0}%";
}
```

The nested `ProductividadMecanicoDto` in `ReporteOperativoViewModel.cs` is removed; the DTO becomes a top-level class like the other report DTOs. Every SP column is aliased to match a DTO property name so Dapper maps by name without custom configuration.

`PorcentajeAvanceTexto` keeps formatting out of XAML.

### 3. Repository method
Add to `IReporteGerencialRepository`:

```csharp
Task<IEnumerable<ProductividadMecanicoDto>> ObtenerProductividadMecanicosAsync(DateTime? fechaDesde, DateTime? fechaHasta);
```

Implement in `ReporteGerencialRepository` mirroring `ObtenerModelosMasReparadosAsync`: `DynamicParameters` with `@FechaDesde` / `@FechaHasta` as `DbType.Date`, `QueryAsync<ProductividadMecanicoDto>("sp_ReporteProductividadMecanicos", ..., CommandType.StoredProcedure)`.

### 4. ViewModel
`ReporteOperativoViewModel.LoadAsync`:
- Drop the `ObtenerReporteOrdenesHandler` dependency (and its DI param) — it is no longer the productivity source.
- Load productivity via `_reporteGerencialRepo.ObtenerProductividadMecanicosAsync(FechaDesde, FechaHasta)`, ordered by `TareasCompletadas` desc then `TareasAsignadas` desc.
- Keep the vehicles section unchanged (`ObtenerModelosMasReparadosAsync`).
- Add totals properties for the summary card (`TotalTareas`, `TotalPendientes`, `TotalEnCurso`, `TotalCompletadas`, `PorcentajeAvanceGeneral`), raising `OnPropertyChanged` after reload.

The `Productividad` collection stays `ObservableCollection<ProductividadMecanicoDto>` (now the top-level DTO).

### 5. UI (XAML + PDF)
`CtlReporteOperativo.xaml`: add columns `Pendientes`, `En Curso`, `% Avance` next to `Tareas Asignadas` / `Tareas Completadas`, and a summary card with the general totals.

`ComposeContent` in the VM: widen the productivity table to the six data columns plus totals, and render a bold totals row. `ComposeEncabezadoTaller` / `ComposePieDePagina` (logo) are unchanged.

### 6. Remove demo fallback
`ObtenerReporteOrdenesHandler.HandleAsync`: delete the `catch { }` + `GetReportesDemostrativos()` fallback and the `GetReportesDemostrativos()` method. On empty data return an empty list; on error let it surface / return empty — never fabricate rows. This affects `Reporte de Órdenes` (and the informational `MecanicoNombre`, which stays as the first-detail mechanic since that report is order-granular by design).

## Risks / Trade-offs
- **Behavior change**: the order report becomes empty instead of showing demo rows when there is no data. Intended (demo data was hiding real emptiness).
- **Empty mechanic names**: full name (`Nombre Apellido`) is used, consistent with `sp_HistorialClinicoVehiculo`. The old code used `Nombre` only; the change is cosmetic and improves disambiguation.
- **Dashboard parity**: the screen and PDF share the same DTO, so they cannot diverge.

## Rollback
Revert the branch. The SP uses `CREATE OR ALTER`; reverting the C# code leaves an unused procedure (harmless).
