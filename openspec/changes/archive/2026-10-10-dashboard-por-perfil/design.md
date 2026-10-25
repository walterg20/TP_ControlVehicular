# Design: Role-Based Dashboard with Real Metrics and Admin Monthly Revenue

## Context
The dashboard is rendered by `CtlDashboard` (a `UserControl` hosted by `MainWindow`) with its logic in `DashboardViewModel`, which calls `ObtenerDashboardHandler`. Today:
- `ObtenerDashboardHandler` injects `IVehiculoRepository` + `IMapper`, resolves `mecanicoId` for the Mecánico role, calls `VehiculoRepository.GetAllWithDetailsAsync(mecanicoId)` and maps vehicles to `VehiculoDto`.
- `DashboardMetricsDto { VehiculosActivosCount, EnProcesoCount, EntregadasHoyCount, List<VehiculoDto> VehiculosEnTaller }`.
- `EnProcesoCount` / `EntregadasHoyCount` are fabricated as `Vehiculos.Count * 0.5` / `* 0.25`.
- `VehiculoRepository.GetAllWithDetailsAsync` applies no state filter, so it returns every vehicle.

Existing patterns to reuse:
- Work orders are read through `IRegistroServicioRepository.GetReporteCompletoAsync(int? mecanicoId)` (orders with vehicle + client + model + brand, recepcionista, and details with service + mechanic).
- Admin income is read through `IReporteGerencialRepository.ObtenerIngresosPorFechaAsync(desde, hasta)` (`sp_ReporteIngresosAdmin` → `ReporteIngresoDto`).
- Roles: `RolesSistema { Administrador = 1, Recepcionista = 2, Mecanico = 3 }`; the current user is in `UserSession.CurrentUser` (`UsuarioDto` with `IdUsuario`, `IdRol`).
- States: `RegistroServicio.Estado` ∈ {`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`} (use the `RegistroServicio.EstadoCancelada` constant).
- No new SQL is expected: the Admin revenue reuses `sp_ReporteIngresosAdmin`.

## Goals / Non-Goals
- Goals: real metrics, role scoping, Admin monthly revenue, refresh button.
- Non-Goals: on-screen charts, new permissions (the revenue card reuses the Admin role, aligned with `Reporte.Gerencial.Ver`), changing the PDF reports.

## Decisions

### D1 — Metrics unit is the work order (RegistroServicio)
- `VehiculosActivosCount` = `orders.Where(active).Select(o => o.VehiculoId).Distinct().Count()`, where active = {`Abierta`, `En Proceso`}. It matches the number of rows shown in the grid.
- `EnProcesoCount` = `orders.Count(active)`.
- `EntregadasHoyCount` = `orders.Count(o => (Completada|Pagada) && o.Fecha.Date == today)`.
- Rationale: consistent with the report conventions (a work order is the unit); removes the fabricated factors.

### D2 — Role scoping
- Resolve the current user once in the handler.
- Mecánico → pass `mecanicoId` to `GetReporteCompletoAsync`, so only orders with a detail of that mechanic are returned.
- Administrador / Recepcionista → `GetReporteCompletoAsync(null)` (whole workshop).

### D3 — Admin monthly revenue
- If the role is Administrador, call `ObtenerIngresosPorFechaAsync(firstDayOfMonth, today)` and sum `IngresosTotales` into `IngresosMesTotal`; set `MostrarIngresos = true`.
- Otherwise `IngresosMesTotal = 0` and `MostrarIngresos = false` (the card is not shown).

### D4 — DTO and ViewModel
- Extend `DashboardMetricsDto` with `decimal IngresosMesTotal` and `bool MostrarIngresos`.
- `DashboardViewModel` gains `IngresosMesTotal` (decimal) and `MostrarIngresos` (bool); the existing `CargarDashboardCommand` is reused by the refresh button.

### D5 — View
- Add the fourth card (**Ingresos del Mes**) bound to `IngresosMesTotal`, with its visibility bound to `MostrarIngresos`.
- Add the missing `"🔄 Actualizar"` button bound to `CargarDashboardCommand`.
- Keep the existing design tokens (rounded corners, drop shadows) and the button palette.

## Risks / Trade-offs
- **Approximate "Entregadas Hoy"**: the model has no delivery/finalization timestamp, so the order date approximates it. Documented as a limitation.
- **Admin revenue access**: `sp_ReporteIngresosAdmin` is already the source of truth for the managerial report; reusing it avoids a divergent income definition and avoids new SQL.
- **Encoding**: XAML/VM edits must preserve UTF-8 (accents, emoji). Anchor edits on ASCII-only substrings and use HTML entities for new accents/emoji.

## Migration Plan
- No schema or data migration; read-only queries over existing tables and stored procedures.
- No database script required unless verification shows a missing index; the next free SQL number is `19`.

## Open Questions
- (none)
