# Design: Universal Customer/Vehicle Search and Unified Registration in Work Order Form

## Context

`CtlOrdenServicioForm` is the single entry point for creating and editing work orders (`RegistroServicio`). Its header currently exposes two independent selectors:

- `cmbClientes`: editable `ComboBox` bound to `ClientesDisponibles`, filtered in memory by `FiltrarClientes(string?)` against `Nombre`, `Apellido`, `Dni` from a **3-character** threshold.
- `cmbVehiculos`: non-editable `ComboBox` bound to `VehiculosDisponibles`, a passive list filtered by the currently selected `ClienteId` through `FiltrarVehiculosPorCliente()`.

New records are created through two independent modals: `FrmCliente` (`ClienteViewModel` → `RegistrarClienteHandler`) and `FrmVehiculo` (`VehiculoViewModel` → `RegistrarVehiculoHandler`), each of which re-opens the screen after a successful save.

The data model is:
- `Cliente` (Id, Nombre, Apellido, Dni, FechaNacimiento, Direccion, Email, Telefono, Activo).
- `Vehiculo` (Id, ModeloId, Anio, Patente, KmActual) → `Modelo` → `Marca`.
- `PropietarioVehiculo` (ClienteId, VehiculoId, FechaAdquisicion, FechaVenta, EsActual) is the ownership link; the **current** owner is the row with `EsActual = true`.

`OrdenServicioViewModel.LoadCombosAsync()` already loads the full client catalog (`_todosLosClientes`) and the full vehicle catalog with model, brand and current owner (`_todosLosVehiculos`) into memory.

See [proposal.md](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/openspec/changes/busqueda-universal-alta-unificada/proposal.md) for the full motivation.

## Goals / Non-Goals

**Goals:**
- One search control that matches DNI, surname, name and plate simultaneously, from a **3-character** threshold, reusing the existing in-memory catalogs (no new DB queries, no schema change).
- Unambiguous row formatting: `[Patente] - Apellido, Nombre (DNI) - Vehiculo`.
- Auto-fill of client **and** vehicle on selection; grouping for clients owning several vehicles.
- A no-results action row that opens a **single** modal creating client + vehicle + ownership link, with plate pre-fill and focus moving to `Kilometraje` after a successful save.
- Keep the MVVM boundary: the ViewModel owns state and the search algorithm; the code-behind owns WPF-specific concerns (dropdown, focus, modal lifecycle).

**Non-Goals:**
- No database schema, stored procedure or migration changes.
- No DNI/CUIT external API lookup (the project has none; the field stays manual).
- No changes to `FrmCliente` / `FrmVehiculo`, which stay available for their own screens.
- No rework of the services combo, the checklist or the work order saving flow.

## Decisions

### 1. A dedicated search-result DTO instead of reusing `ClienteDto`
**Decision:** Introduce `Negocio/DTOs/BusquedaClienteVehiculoDto` as the row type of the universal search.

**Rationale:** The row is not a client and not a vehicle: it is the *pair* (client + vehicle) plus presentation metadata. Reusing `ClienteDto` would force the dropdown to lose the plate, and reusing `VehiculoDto` would lose the owner name/DNI split needed for the `[Patente] - Apellido, Nombre (DNI)` format.

**Shape:**
```csharp
public class BusquedaClienteVehiculoDto
{
    public int ClienteId { get; set; }
    public int VehiculoId { get; set; }        // 0 when the row is a grouped client row
    public string Patente { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string VehiculoDescripcion { get; set; } = string.Empty; // "Marca Modelo"
    public bool EsAgrupado { get; set; }       // client owning several vehicles
    public int CantidadVehiculos { get; set; }
    public bool EsAccionRegistrar { get; set; } // "No se encontraron resultados + Registrar"
    public string DisplayText { get; set; } = string.Empty;
}
```
`DisplayText` is computed by the ViewModel so the XAML only binds one property, and the DTO stays a plain transport object.

**Alternative rejected:** a `ComboBox` whose items are `VehiculoDto` with a multi-column `ItemTemplate`. Rejected because the "no results → register" row and the grouped "Ver N vehiculos" row are not vehicles and would pollute the type.

### 2. Search algorithm runs in the ViewModel over the in-memory catalogs
**Decision:** `OrdenServicioViewModel.FiltrarBusquedaUniversal(string? texto)` builds the result rows from `_todosLosClientes` and `_todosLosVehiculos` (already loaded by `LoadCombosAsync`), and `OrdenServicioViewModel.SeleccionarResultadoUniversal(BusquedaClienteVehiculoDto)` applies the selection.

**Rationale:**
- The catalogs are already fully materialized in memory by the existing feature `orden-servicio-busqueda-combos`; no extra I/O is introduced and the search is instantaneous.
- The ViewModel stays unit-testable without a database (the existing `OrdenServicioCombosBusquedaTests` follow this pattern).
- Avoiding a new round trip per keystroke keeps the UI responsive; the debounce (250 ms) plus an in-memory filter is enough for the current data volume.

**Matching rules (case-insensitive substring):**
- A vehicle row matches when the term hits `Patente` **or** the owner's `Dni`, `Apellido` or `Nombre`.
- A client with **exactly one** vehicle produces a normal row `[Patente] - Apellido, Nombre (DNI) - Vehiculo`.
- A client with **2 or more** vehicles produces a **grouped** row `Apellido, Nombre (DNI: nnn) - [Ver N vehiculos]`, replacing its individual vehicle rows, so the operator is not flooded with near-duplicate lines.
- If the term produces no row at all, a single row with `EsAccionRegistrar = true` and text `🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo` is appended.
- Results are capped (25) to keep the dropdown usable.

**Alternative rejected:** a server-side query (`SqlQueryRaw` / stored procedure) over `Vehiculo JOIN PropietarioVehiculo JOIN Cliente`. Deferred: it is the right long-term answer for large fleets, but it needs a new SP in `bd/03_procedimientos.sql`, a projection type and pagination semantics that this change does not need yet. The decision is recorded here as the migration path.

### 3. Threshold of 3 characters, consistent with the existing combos
**Decision:** The predictive filter activates at **3 or more characters**, exactly like `FiltrarClientes` / `FiltrarServicios` today.

**Rationale:** consistency across the form; below the threshold the plate prefix ("AA") would return a huge, noisy list; it also keeps the operator's mental model unchanged from the shipped behavior.

### 4. Selection is applied by an explicit method, not by the setter
**Decision:** The combo binds `SelectedItem` to `ResultadoBusquedaSeleccionado`; the code-behind calls `SeleccionarResultadoUniversal(...)` when the selection changes.

**Rationale:** the setter of a bound property is also written by the code-behind when the form pre-fills an existing order (modification mode), which must not trigger the "open the vehicle dropdown" side effect. Keeping the side effect in an explicit method makes the intent obvious and avoids re-entrancy between the binding and the handler.

### 5. Grouped rows select the client only and expand the vehicle dropdown
**Decision:** Selecting a grouped row sets `ClienteId` only; `FiltrarVehiculosPorCliente()` then repopulates `VehiculosDisponibles` with that client's vehicles, and the code-behind opens `cmbVehiculos` and focuses it.

**Rationale:** it satisfies the "one click" requirement without guessing which vehicle the operator meant. The `VehiculoId` stays at 0 until the operator picks, which the existing `ValidateProperty(nameof(VehiculoId))` already reports as an error on save.

### 6. The unified modal owns a dedicated ViewModel
**Decision:** New `ClienteVehiculoViewModel` (inheriting `BaseViewModel`, implementing `INotifyDataErrorInfo`) injects `IClienteRepository`, `IVehiculoRepository`, `IModeloRepository`, `IMarcaRepository` and `IRepository<PropietarioVehiculo>` plus `IMapper`. `RegistrarAsync()` performs, in order: insert client → insert vehicle → insert ownership link, and exposes `ClienteCreado` / `VehiculoCreado` for the caller.

**Rationale:**
- A dedicated ViewModel keeps a single object graph for the two-column form (brand → model cascade) without polluting `OrdenServicioViewModel` or reusing two independent ViewModels that would fight over the same screen.
- The repositories already follow the "one `DbContext` per operation" factory pattern (`Repository<T>.CrearContexto()`), so the three inserts never share a context and no "A second operation was started" error can occur.
- `FrmClienteVehiculo` exposes `ClienteCreado`/`VehiculoCreado` so the caller (`CtlOrdenServicioForm`) can select the new records without re-querying.

**Alternative rejected:** reuse `FrmCliente` + `FrmVehiculo` chained (open the second after the first). Rejected: two windows, no shared context, plate must be retyped — exactly the friction this change removes.

### 7. Year and engine are optional; engine is not persisted
**Decision:** The modal exposes `Patente`, `Marca`, `Modelo`, `Anio` and `Motor`. `Anio` is optional (defaults to the current year). `Motor` is captured in the form only and **not persisted**: the `Vehiculo` table has no motor column and this change adds no schema.

**Rationale:** the user asked for year/engine to be optional "so reception is not delayed"; not blocking reception on data the database does not hold is preferable to a schema change. `Motor` remains available for the operator to type and is discarded on save (documented in the modal tooltip).

### 8. Focus management is explicit
**Decision:** After a successful unified registration (or after selecting a single-vehicle row), the code-behind moves focus to `txtKm` (Kilometraje). After selecting a grouped row, focus goes to `cmbVehiculos` and the dropdown opens.

**Rationale:** the acceptance criterion explicitly requires the keyboard to travel to `Kilometraje` after the modal closes; the grouped case needs the vehicle to be chosen before the kilometrage makes sense.

## Risks / Trade-offs

- **[Risk]** Editing an existing order pre-fills `ClienteId`/`VehiculoId`; the new universal combo must show a readable label even though it holds search rows, not clients.
  - *Mitigation:* in modification mode the code-behind writes the client's display text into the combo's `Text` (an editable combo can show text that is not one of its items) and clears the row collection, so no stale row can be re-selected.

- **[Risk]** Modifying the `ItemsSource` of an editable `ComboBox` while typing can move the caret or close the dropdown.
  - *Mitigation:* same as the existing implementation — restore `SelectionStart`/`SelectionLength` after filtering and guard the re-entrant `TextChanged` with the `_omitirFiltroUniversal` flag.

- **[Risk]** The in-memory catalog grows unbounded in a very large fleet.
  - *Mitigation:* the result list is capped at 25 rows; the long-term migration path (server-side query) is recorded in Decision 2.

- **[Risk]** A partial failure between the three inserts could leave an orphan client.
  - *Mitigation:* the inserts run in order (client → vehicle → link) and the ownership link is the last step; if it fails the caller reports the error and the operator retries. A shared transaction across repositories would require a new unit-of-work API, which is out of scope.

- **[Risk]** The "no results" row could be selected accidentally with the Enter key.
  - *Mitigation:* it is the only row in the dropdown in that state, it is explicitly labelled as an action, and it opens a modal that can be cancelled without side effects.

## Migration Plan

Single feature branch `feature/busqueda-universal-alta-unificada`, merged into `develop` once the build and the test suite are green. The change is backward compatible at the domain level: no table, column or stored procedure is touched, and `FrmCliente` / `FrmVehiculo` remain fully functional.
