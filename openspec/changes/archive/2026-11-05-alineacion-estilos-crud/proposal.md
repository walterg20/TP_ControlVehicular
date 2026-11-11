# Proposal: Alignment of CRUD Screen Styling with CtlModelo

## Context
The application recently aligned `CtlModelo`, `CtlMarca`, and `CtlVehiculo` to a unified UI and interaction pattern for soft-deleted entities:
1. Status filter dropdown ("Todos", "Activos", "Inactivos") with RBAC permission visibility.
2. Inactive row highlighting in red (`#C0392B` foreground, `#FDECEA` background).
3. Removal of the redundant "Activo" checkbox column in DataGrids.
4. Consistent action button bar at the bottom right: Edit (`#F39C12`), Delete (`#C0392B`), Reactivate (`#27AE60`).

The remaining CRUD management screens (`CtlCliente`, `CtlUsuario`, `CtlRol`, `CtlServicio`, and `CtlTaller`) still use an older layout (such as a "Mostrar inactivos" checkbox, visible "Activo" column, blue reactivate button placed on the left, or no status filter).

## Proposal
Align `CtlCliente`, `CtlUsuario`, `CtlRol`, `CtlServicio`, and `CtlTaller` with `CtlModelo.xaml`:
- Replace "Mostrar inactivos" checkbox with `Estado:` dropdown (`Todos`, `Activos`, `Inactivos`), visible only when user has permission.
- Add `DataGrid.RowStyle` with DataTrigger for `Activo == False` (red text, light red background).
- Remove the `Activo` column from all 5 DataGrids.
- Standardize bottom bar actions: Edit (Orange), Delete (Red), Reactivate (Green).
- Update repositories and ViewModels to support the three-state status filter.
