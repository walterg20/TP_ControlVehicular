# Proposal: Fix Taller ABM (Creation, Editing, and Deletion)

## Why
The Taller management module (`CtlTaller` and `FrmTaller`) had several implementation bugs preventing full CRUD operations:
1. `GuardarTallerAsync` in `TallerViewModel` required `TallerSeleccionado` to be non-null and read properties from `TallerSeleccionado` instead of ViewModel form properties (`Nombre`, `Direccion`, `Telefono`). This caused new Taller creation to fail and editing to ignore form inputs.
2. `CtlTaller.xaml.cs` had commented-out code and mismatched event handler method names (`BtnEditar_Click` and `BtnBorrar_Click` vs `BtnModificar_Click` and `BtnBaja_Click`).
3. `FrmTaller.xaml.cs` did not invoke `LimpiarFormulario()` when creating a new Taller.

## What
1. Update `TallerViewModel.cs`:
   - Fix `GuardarTallerAsync()` to use `Nombre`, `Direccion`, `Telefono`, and `Activo` properties.
   - Add `LimpiarFormulario()` to reset form state.
   - Ensure `ToggleActivoAsync()` correctly updates the Taller state via `ModificarTallerHandler`.
2. Update `FrmTaller.xaml.cs`:
   - Reset form via `LimpiarFormulario()` on new creation.
   - Populate ViewModel properties from `TallerDto` on edit.
   - Show validation warning if `GuardarTallerAsync()` fails.
3. Update `CtlTaller.xaml.cs`:
   - Connect `BtnNuevo_Click`, `BtnEditar_Click`, `BtnBorrar_Click`, and `BtnBuscar_Click` properly.
   - Use `FrmConfirmacion.Mostrar(...)` for deletion/baja confirmation.

## Impact
- Files modified: `TallerViewModel.cs`, `FrmTaller.xaml.cs`, `CtlTaller.xaml.cs`.
- No database schema or repository interface changes required.
