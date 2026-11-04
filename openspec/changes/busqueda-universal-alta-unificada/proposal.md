# Proposal: Universal Customer/Vehicle Search and Unified Registration in Work Order Form

## Why

Today, registering a work order in `CtlOrdenServicioForm` forces the operator to decide *how* to search: the client combo matches only `Nombre`, `Apellido` or `Dni`, while the vehicle combo is a passive list that only reacts to a previously selected client. A mechanic or receptionist who only knows the license plate cannot find the owner, and a client who owns several vehicles requires several round trips through two separate dropdowns.

On top of that, registering a brand-new client and their vehicle requires **two separate modal windows** (`FrmCliente` and `FrmVehiculo`), which loses the context of the order being created and forces the operator to remember and retype the plate.

This change introduces a single universal search bar that matches DNI, surname, name and license plate simultaneously, and a unified two-column registration modal that creates client, vehicle and ownership link in one transaction-like flow.

## What Changes

- **Universal predictive search**:
  - The client selector in `CtlOrdenServicioForm` becomes a single editable combo that matches the term (case-insensitive) against `Dni`, `Apellido`, `Nombre` **and** `Patente` simultaneously.
  - The predictive filter activates from **3 characters** onwards (consistent with the existing client/service search threshold); below the threshold the dropdown stays closed and no filtering is applied.
  - Result rows are formatted unambiguously as `[Patente] - Apellido, Nombre (DNI) - Vehiculo`.

- **Selection semantics**:
  - Selecting a row that points to a single vehicle auto-fills **both** the client and the vehicle fields of the work order.
  - Selecting a row that points to a client owning **more than one vehicle** fills the client only and displays `[Ver N vehiculos]`; the vehicle dropdown is then opened so the operator picks one of the client's vehicles in a single click.

- **Unified registration flow (no results)**:
  - When the term yields no match, the dropdown shows a single actionable row: `🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo`.
  - Choosing it (or pressing the existing `➕` button) opens **one** modal with two visual columns — client data on the left (DNI, surname, name, phone) and vehicle data on the right (plate, brand, model, optional year/engine) — saved with a single `Guardar y Vincular` button.
  - The plate typed in the search bar is pre-filled in the modal when it looks like a plate.
  - On success the modal closes by itself, the work order form shows the new client and vehicle already selected, and keyboard focus moves to the `Kilometraje` field.

## Capabilities

### New Capabilities
None.

### Modified Capabilities
- `orden-servicio`: The client/vehicle selection area of `CtlOrdenServicioForm` is replaced by a universal predictive search that also matches license plates, auto-fills client and vehicle on selection, groups clients owning several vehicles, and offers a unified client+vehicle registration modal when no match is found.

## Impact

- **Presentation**:
  - `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml` / `.xaml.cs`: universal search combo, selection handling, focus management, wiring to the unified modal.
  - `Presentacion/Pantalla/OrdenServicio/FrmClienteVehiculo.xaml` / `.xaml.cs` (new): unified two-column registration modal.
  - `Presentacion/ViewModels/OrdenServicioViewModel.cs`: `ResultadosBusquedaUniversal` collection, `FiltrarBusquedaUniversal`, `SeleccionarResultadoUniversal` and the events that request the modal and the focus change.
  - `Presentacion/ViewModels/ClienteVehiculoViewModel.cs` (new): validation and transactional registration of client + vehicle + ownership.
- **Negocio**:
  - `Negocio/DTOs/BusquedaClienteVehiculoDto.cs` (new): unified search-result row.
- **Composition**: `App.xaml.cs` registers `ClienteVehiculoViewModel`.
- **Dependencies**: none added. Reuses `Debouncer`, `FrmConfirmacion`, existing repositories and the `Repository<T>` per-operation `DbContext` factory pattern.
- **Tests**: new unit tests in `TP_ControlVehicular.Tests` for the universal filter, the grouping rule, the no-results action row and the unified registration view model.
- **Database**: no schema or stored procedure changes. The search runs over the client/vehicle catalogs already loaded in memory by `LoadCombosAsync`.
