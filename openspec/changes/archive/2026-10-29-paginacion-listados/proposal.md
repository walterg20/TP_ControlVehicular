# Proposal: Database Pagination for All Listing Screens

## Why

Every listing screen in the system loads its entire result set into memory and filters it on the client. `ClienteViewModel`, `MarcaViewModel`, `ModeloViewModel`, `VehiculoViewModel`, `RolViewModel`, `UsuarioViewModel`, `ServicioViewModel`, `TallerViewModel`, `OrdenServicioViewModel`, `MisTrabajosViewModel`, `ReporteOrdenesViewModel` and `ReportesViewModel` all follow the same shape: `GetTodosAsync()` / `GetReporteCompletoAsync()` pulls the whole table (or every service order) into an `ObservableCollection`, and a `ListadoXFiltered` property applies `IndexOf(..., OrdinalIgnoreCase)` in memory.

This pattern has three concrete costs:

1. **Unbounded memory and latency.** The 50+ client / 40+ vehicle / multi-hundred order seed data already exceeds what a grid should hold; the same query cost grows linearly with the table.
2. **The search is not actually a database search.** Filtering in memory cannot use indexes, so every keystroke re-scans the whole collection, and any filter that spans joined columns (client name, brand/model, mechanic) can only be expressed against an already-flattened DTO.
3. **No reusable pagination.** The only screen that paginates today is the Managerial Report (`CtlReporteGerencial`), and it carries its own hand-rolled pagination state inline in `ReporteGerencialViewModel` plus a bespoke toolbar hard-coded in `CtlReporteGerencial.xaml`. There is nothing reusable, so every new screen would have to copy it.

The project already shipped database-level pagination for the Managerial Income grid (`sp_ReporteIngresosAdminPaginado`). This change generalises that proven approach into a reusable control plus a reusable ViewModel base, and applies it to every main listing.

## What Changes

- **Reusable pagination infrastructure**
  - `IPaginacion` contract and `BasePaginableViewModel` base class, centralising page state, clamping, page-size handling and the First/Previous/Next/Last commands.
  - `CtlPaginacion` reusable `UserControl` under `Presentacion/Pantalla/Compartido/`, visually identical to the current Managerial Report toolbar.
  - `Debouncer` helper (500 ms) so server-side search does not issue one database round-trip per keystroke.
- **Managerial Report migration** — `CtlReporteGerencial` replaces its inline toolbar with `CtlPaginacion`; `ReporteGerencialViewModel` derives from `BasePaginableViewModel` without changing any public member, so the existing pagination tests keep passing.
- **Database-level pagination + server-side search for every main listing**, including the small catalogues (Marca, Modelo, Servicio, Taller, Rol) — no listing is exempted.
  - CRUD listings use EF Core (`.Skip` / `.Take` + `.CountAsync`), which translates to `OFFSET ... FETCH NEXT ... ROWS ONLY`.
  - Transactional listings and reports that already expose joined/flattened projections use new stored procedures in `bd/03_procedimientos.sql`, consumed with the existing `SqlQueryRaw("EXEC sp_...")` pattern.
- **Search moves to the server** for all listings, with an explicit `Buscar` button and `Enter` key support, plus a 500 ms debounce while typing. Any filter change resets to page 1.
- **PDF exports are unchanged**: report PDFs keep exporting the complete filtered result set, not only the visible page.

## Capabilities

### New Capabilities

- `paginacion-listados`: reusable pagination control and ViewModel base, plus the server-side pagination and search contract that every listing must satisfy.

### Modified Capabilities

- `clientes`: client listing paginated in the database with server-side search.
- `marcas`: brand listing paginated in the database with server-side search.
- `modelos`: model listing paginated in the database with server-side search.
- `vehiculos`: vehicle listing paginated in the database with server-side search.
- `usuarios`: user listing paginated in the database with server-side search.
- `taller`: workshop listing paginated in the database with server-side search.
- `service-management`: service listing paginated in the database with server-side search.
- `role-access`: role listing paginated in the database with server-side search.
- `mis-trabajos`: mechanic order listing paginated in the database with server-side search.
- `reportes`: order report, reception reports and the managerial report toolbar migrate to the shared pagination control; report PDFs keep exporting the full filtered set.

## Impact

- **Presentation**
  - New: `Presentacion/ViewModels/IPaginacion.cs`, `Presentacion/ViewModels/BasePaginableViewModel.cs`, `Presentacion/Pantalla/Compartido/CtlPaginacion.xaml(.cs)`, `Presentacion/Helpers/Debouncer.cs`.
  - Modified: `Presentacion/ViewModels/ReporteGerencialViewModel.cs`, `Presentacion/Pantalla/Reporte/CtlReporteGerencial.xaml` and every listing ViewModel / `Ctl*.xaml` in scope.
- **Data & Business**
  - New: paginated + searchable methods on `IClienteRepository`, `IMarcaRepository`, `IModeloRepository`, `IVehiculoRepository`, `IUsuarioRepository`, `IRolRepository`, `IServicioRepository`, `ITallerRepository`, `IRegistroServicioRepository`, `IReporteRepository` and their implementations in `Datos/Repositories/`.
  - New generic helper `Repository<T>.GetPaginadoAsync(...)` in `Datos/Repositories/Repository.cs`.
- **Database**
  - `bd/03_procedimientos.sql`: new `sp_ReporteOrdenesPaginado`, `sp_HistorialVehiculoPaginado`, `sp_VehiculosPorClientePaginado`. No new numbered script files (the canonical script per object type is reused).
- **Tests**: new unit tests in `TP_ControlVehicular.Tests` covering the pagination base class (clamping, page-size reset, total-page calculation) and the pilot screen.
- **Docs**: `docs/plan/2026-10-29_paginacion-listados.md`, plus `MEMORY.md` and `AGENTS.md` conventions.

## Non-Goals

- Paginating aggregated or inherently bounded grids: `CtlReporteOperativo`, `CtlReporteServicios`, the Dashboard vehicle grid, and the order checklist grid inside `CtlOrdenServicioForm`.
- `Pantalla/Taller/CtlMisTrabajos.xaml` (`grdTrabajos`) — dead code, never registered in `MainWindow`.
- Changing the authorisation / RBAC model, the auto-mapping profile shape, or the PDF layout.
