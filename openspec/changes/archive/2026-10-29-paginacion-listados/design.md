# Design

## Context

See `proposal.md` for background and motivation.

The application has two distinct data-access styles that this change must respect:

- **CRUD listings** (Cliente, Marca, Modelo, Vehículo, Rol, Usuario, Servicio, Taller) go through `Repository<T>` (EF Core, `CVDbContext`) and their `Listar*Handler` services, then land in an `ObservableCollection` inside the ViewModel.
- **Transactional listings and reports** (OrdenServicio, Mis Trabajos, Reporte Órdenes) go through `IRegistroServicioRepository.GetReporteCompletoAsync()` — an EF query with a deep `Include` graph — and are projected to flat DTOs **in C#**.
- **Report listings** (Reportes Recepción) already call stored procedures through `IReporteRepository` using `SqlQueryRaw("EXEC sp_...")`.
- **The one existing precedent**: `CtlReporteGerencial` paginates with `sp_ReporteIngresosAdminPaginado` + Dapper, and `ReporteGerencialViewModel` implements the page state by hand.

Two properties of the current code drive the design:

1. The in-memory search is expressed against **flattened DTO strings** (`ClienteDetalle` = `"Nombre Apellido [DNI]"`, `VehiculoDetalle` = `"Marca Modelo"`). Those concatenations are not translatable to SQL, so the server-side search has to be re-expressed per column.
2. `ReporteOrdenesViewModel` builds `ServiciosAplicados` with `string.Join(", ", r.Detalles.Where(...).Select(...))`. `string.Join` over a navigation collection **cannot** be translated by EF Core, so that screen cannot be paginated with EF Core LINQ.

## Goals / Non-Goals

**Goals:**
- One reusable pagination toolbar and one reusable ViewModel base, used by every listing.
- Database-level pagination and server-side search on every main listing, catalogues included.
- A `Buscar` button plus `Enter` support, with a 500 ms debounce while typing.
- Report PDFs keep exporting the complete filtered result set.
- No change to the public surface of `ReporteGerencialViewModel` (existing tests depend on it).

**Non-Goals:**
- Paginating bounded/aggregated grids (Dashboard, Operative Report, Services Report, order checklist).
- Rewriting the repository layer or introducing a new ORM/persistence abstraction.
- Changing the visual design of the pagination toolbar (it must look exactly like the current Managerial Report one).

## Decisions

### 1. Composition over inheritance for the ViewModel contract
- **Decision:** Define `IPaginacion : INotifyPropertyChanged` and have `BasePaginableViewModel : BaseViewModel, IPaginacion` implement it. `CtlPaginacion` exposes a single dependency property `Model` of type `IPaginacion`, and every screen binds `<c:CtlPaginacion Model="{Binding}" />`.
- **Rationale:** `IPaginacion` keeps the control decoupled from the ViewModel hierarchy (any object can drive the toolbar), while `BasePaginableViewModel` removes the duplication that exists today in `ReporteGerencialViewModel`. `INotifyPropertyChanged` on the interface is required so WPF subscribes to the change notifications along the `Model.Propiedad` binding path.
- **Alternative considered:** A standalone `PaginacionController` used by composition in every ViewModel. Rejected: it forces every screen to expose an extra property and to re-wire the bindings, and it cannot inherit `BaseViewModel`'s `SetProperty`/`Modulo` helpers.
- **Note:** `AGENTS.md` documents `VehiculoViewModel` as a flat class that does not inherit `BaseViewModel`, but the code declares `public class VehiculoViewModel : BaseViewModel`. The repository is the evidence: every ViewModel inherits `BaseViewModel`, so the inheritance route is safe. `AGENTS.md` is corrected in this change.

### 2. `CtlPaginacion` as a `UserControl` in `Compartido/`
- **Decision:** Place the control in `Presentacion/Pantalla/Compartido/`, next to `FrmConfirmacion`, with the exact markup of the current Managerial Report toolbar (`#F8F9F9` background, `#E5E7E9` border, `CornerRadius=8`, `Padding=12,8`, `#2C3E50` navigation buttons).
- **Rationale:** Extracting the existing markup guarantees the migration is visually neutral, and `Compartido/` is already the established home for shared presentation components.
- **Alternative considered:** A `DataTemplate` / `ControlTemplate` in `App.xaml` resources. Rejected: it would need to be re-declared per screen and makes the `Model` dependency property harder to consume.

### 3. EF Core `.Skip`/`.Take` + `.CountAsync` for CRUD listings
- **Decision:** Add a single generic method to `Repository<T>`:
  ```csharp
  public virtual async Task<(IEnumerable<T> Items, int TotalRegistros)> GetPaginadoAsync(
      int pagina, int tamanoPagina,
      Expression<Func<T, bool>>? filtro = null,
      Func<IQueryable<T>, IQueryable<T>>? configurador = null,
      Func<IQueryable<T>, IOrderedQueryable<T>>? orden = null)
  ```
  and expose a domain-named wrapper on each concrete repository (e.g. `IClienteRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)`).
- **Rationale:** EF Core 10 translates `Skip`/`Take` into `OFFSET ... FETCH NEXT ... ROWS ONLY`, satisfying the "pagination in the database" requirement without adding SQL objects for tables that already have a well-modelled EF repository. `CountAsync()` runs over the exact same filtered query, so the count and the page can never disagree.
- **Alternative considered:** A stored procedure per listing. Rejected for CRUD: it would duplicate the EF layer, require Dapper plumbing for eight tables, and add eight idempotent SQL objects to maintain for no functional gain.

### 4. New stored procedures only where SQL is unavoidable
- **Decision:** Add `sp_ReporteOrdenesPaginado`, `sp_HistorialVehiculoPaginado` and `sp_VehiculosPorClientePaginado` to `bd/03_procedimientos.sql`, consumed through the existing `SqlQueryRaw("EXEC sp_...")` pattern of `ReporteRepository`.
- **Rationale:** `ReporteOrdenesViewModel` needs `ServiciosAplicados` (a `string.Join` over a collection) and a search across six joined columns — not translatable by EF Core. The reception reports are already stored procedures, so they stay procedures.
- **Pattern:** a CTE `Base` + `CROSS JOIN` on `COUNT(*)`, exactly like `sp_ReporteIngresosAdminPaginado`: one round-trip, no race between the count and the page under concurrent writes.
- **Note:** `sp_ReporteOrdenesPaginado` uses `STRING_AGG` to reproduce `ServiciosAplicados`.

### 5. Re-expressing the in-memory search as SQL predicates
- **Decision:** Translate each flattened-DTO search into per-column predicates:
  | Current in-memory predicate | Server-side predicate |
  |---|---|
  | `ClienteDetalle` = `"Nombre Apellido [DNI]"` | three `LIKE`s over the actual proprietor columns |
  | `VehiculoDetalle` = `"Marca Modelo"` | two `LIKE`s over `Modelo.Marca.NombreMarca` and `Modelo.NombreModelo` |
  | `RecepcionistaNombre` = `"Nombre Apellido"` | two `LIKE`s over `Usuario.Nombre` / `Usuario.Apellido` |
  | `Id.ToString().Contains(txt)` | numeric special case: `OR Id == n` when the term parses as an integer |
  | plain `Contains` on a column | `EF.Functions.Like(column, $"%{txt}%")` over `COALESCE(column, '')` |
- **Rationale:** `EF.Functions.Like` avoids the `Contains` + `StringComparison` translation pitfalls and gives SQL Server index-friendly `LIKE '%…%'` semantics. The `COALESCE` guard preserves today's behaviour on nullable columns (today the C# code uses `?? string.Empty`).
- **Behavioural note (documented):** searching a term that only exists in the *concatenation* (e.g. `"Nombre [DNI]"` with the exact spacing) no longer matches; the term must match within one of the underlying columns. This is the accepted cost of moving the search to the database.
- **Note:** `Id.ToString().Contains(txt)` is **not** translatable by EF Core and is handled as a separate numeric branch.

### 6. `Buscar` button + `Enter` + 500 ms debounce
- **Decision:** Every listing screen exposes an explicit `Buscar` action (bound to a command) and handles `Enter` in the search box; a `Debouncer` helper (500 ms, `DispatcherTimer`) triggers the same reload while the user types.
- **Rationale:** A server-side search means one database round-trip per keystroke would be wasteful and would make the UI feel laggy. The explicit button gives the user a deterministic way to run the query, while the debounce keeps the "type and see it narrow down" behaviour that the current screens have.
- **Alternative considered:** Debounce only (no button). Rejected: it hides when the query runs and makes the interaction non-deterministic. Button only (no debounce) — rejected: it removes the live-filtering behaviour the screens have today.

### 7. Any filter change resets to page 1
- **Decision:** `BasePaginableViewModel.CambiarPaginaAsync` clamps the requested page into `[1, TotalPaginas]`. Whenever a filter (search text, status, date range, "show inactive") changes, the ViewModel sets `PaginaActual = 1` before reloading.
- **Rationale:** Page 5 of a new filter may not exist, and clamping to the last page instead of the first would surprise the user.

### 8. PDFs keep exporting the complete filtered set
- **Decision:** The report PDFs keep issuing an un-paginated query for the whole filtered result set; only the on-screen grid is paginated.
- **Rationale:** The current behaviour (Reporte Órdenes exports every filtered row) must be preserved: a printed report that silently contained only the visible page would be a data-loss regression for an accounting document.
- **Trade-off:** exporting costs one extra un-paginated query. Accepted.

### 9. `CtlMisTrabajos` master grid pagination
- **Decision:** The master grid of `MisTrabajos` (the mechanic's orders) is paginated in the database. The detail grid (tasks of the selected order) is not, because it is scoped to a single selected order.
- **Rationale:** The master grid grows with the workshop's backlog; the detail grid is bounded by the order's task list.
- **Note:** `MisTrabajosViewModel` currently filters with an `ICollectionView` over an in-memory collection. The collection is replaced by a database query; the per-mechanic task filter (`Detalles.Any(d => d.UsuarioId == usuarioId)`) and the "has at least one pending task" predicate are translatable and are pushed into the database.

### 10. `TallerNombre` becomes populated in the OrdenServicio grid
- **Decision:** The paginated `RegistroServicio` query adds `Include(r => r.Taller)`.
- **Rationale:** `GetReporteCompletoAsync` never included `Taller`, so `RegistroServicioDto.TallerNombre` was always empty — the column silently showed nothing. Including it is required anyway to make "search by workshop" a server-side predicate, and it fixes the latent defect as a side effect.
- **Impact:** visible behaviour change in `CtlOrdenServicio` (the column now shows the workshop name). Called out in the commit message.

## Risks / Trade-offs

- **[Risk]** EF Core `Include` of a collection combined with `Skip`/`Take` can filter the included collection incorrectly on some providers.
  - **Mitigation:** validated during the Phase 1 pilot and again on `MisTrabajos`, where `Detalles` is a collection include. If EF Core's subquery semantics misbehave, the affected listing falls back to the stored-procedure route.
- **[Risk]** The report PDFs now need a second, un-paginated query on export.
  - **Mitigation:** isolated in the export methods; the on-screen path is untouched.
- **[Risk]** `ReporteGerencialPaginacionTests` depend on the current public members of `ReporteGerencialViewModel`.
  - **Mitigation:** the base class exposes exactly the same member names and signatures (`PaginaActual`, `TotalPaginas`, `TotalRegistros`, `TamanoPagina`, `OpcionesTamanoPagina`, `TextoPaginacion`, `PuedeIrAnterior`, `PuedeIrSiguiente` and the four commands), so no test changes are required.
- **[Risk]** `TotalRegistros` could disagree with the row count under concurrent writes when the count and the page are fetched separately.
  - **Mitigation:** the stored procedures compute both in a single statement; EF Core listings accept the same behaviour as the existing Managerial implementation, which also issues two statements.
- **[Risk]** A listing that forgets to reset to page 1 on a filter change leaves the user on an empty page.
  - **Mitigation:** the reset is centralised in `BasePaginableViewModel` and covered by unit tests.
- **[Risk]** `AGENTS.md` states that `VehiculoViewModel` does not inherit `BaseViewModel`, which contradicts the code.
  - **Mitigation:** `AGENTS.md` is corrected in this change; the repository remains the source of truth.
