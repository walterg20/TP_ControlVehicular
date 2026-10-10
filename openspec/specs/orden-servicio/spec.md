# Work Orders (Ordenes de Servicio) Management

## Overview
Implement the UI components and ViewModels for managing Work Orders (`RegistroServicio`). This includes the main view (`CtlOrdenServicio`) and the creation/edition modal form (`FrmOrdenServicio`). The module will be accessible through the "Órdenes de Trabajo" menu item and will follow the established UI and validation standards of the project.

## Functional Requirements
1. **Menu Integration**: 
   - Link `CtlOrdenServicio` to the "Órdenes de Trabajo" menu item in `MainWindow`.
   - Respect role-based access control (RBAC) defined in `AplicarRestriccionesPorRol` for this menu.
2. **Main View (`CtlOrdenServicio`)**:
   - Follow the 4-row grid layout standard, with `<Grid Margin="10">`:
     - Row 0: Header/Title.
     - Row 1: Search bar and `➕ Nueva Orden` (Green `#27AE60`) button. Use `<TextBox.Resources>` and `<Button.Resources>` for `CornerRadius="6"`.
     - Row 2: Card with DataGrid showing work orders (`RegistroServicio`). DataGrid must have dark headers (`Background="#2C3E50"`, `Foreground="White"`, `FontWeight="Bold"`, `Padding="10,8"`).
     - Row 3: Footer card with `✏️ Editar` (Orange `#F39C12`) and `🗑️ Eliminar / Cancelar` (Red `#C0392B`). Use `<Button.Resources>` for `CornerRadius="6"`.
   - Card styling must include `CornerRadius="12"` and `DropShadowEffect (Opacity="0.08", Direction="270", BlurRadius="10" or "12")`.
   - Prevent concurrent initializations by registering the `Loaded` event only once in the constructor to avoid double `LoadAsync()` calls on DbContext.
3. **Modal Form (`FrmOrdenServicio`)**:
   - Master Grid with `Margin="24"` and side-by-side Layout (`Grid` with two columns: Width="130" and Width="*") wrapped inside a `ScrollViewer`.
   - Form Inputs: Inputs must use `Height="30"`, `UpdateSourceTrigger=LostFocus, ValidatesOnNotifyDataErrors=True`.
   - Modal window must set `Owner = Window.GetWindow(this)`.
   - Fields to manage: Vehicle selection (`VehiculoId`), Workshop selection (`TallerId`), Entry Mileage (`KmIngreso`), and Status (`Estado`).
   - Real-time Validation: Inputs must display validation errors using the red color (`#E74C3C`) right below each input. Implement `ConfigurarValidacionAlPerderFoco()` in code-behind.
   - Buttons: `Guardar / Aceptar` (Green `#27AE60`, `IsDefault="True"`), `Cancelar` (Red `#C0392B`). Both must use `<Button.Resources>` to define `CornerRadius="6"`.
4. **Validations & Error Handling**:
   - `KmIngreso` must be valid and numeric.
   - `VehiculoId` and `TallerId` must be selected (greater than 0).
   - Use `FrmConfirmacion.MostrarAviso(...)` for single-button modal error/alert messages (e.g., when the user hasn't selected a row to edit).
   - Use `FrmConfirmacion.Mostrar(...)` for Yes/No confirmations (e.g., when confirming deletion or status change). Do NOT use native `MessageBox.Show`.

## Acceptance Criteria
- [ ] `CtlOrdenServicio` is accessible from the "Órdenes de Trabajo" sidebar menu and displays the grid of work orders.
- [ ] `FrmOrdenServicio` opens as a modal window with the correct styling and captures data correctly.
- [ ] Real-time input validation triggers on `LostFocus` and shows error messages in red.
- [ ] Saving an order persists the `RegistroServicio` data correctly and refreshes the DataGrid.
- [ ] UI components strictly adhere to the project's styling and color conventions (4-row Grid, Colors, CornerRadius, FrmConfirmacion usage).
