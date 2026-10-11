# Technical Design: Taller ABM Fix

## Design Changes
1. `TallerViewModel`:
   - Refactor `GuardarTallerAsync()` to build `Taller` entity using form properties `Nombre`, `Direccion`, `Telefono`, `Activo`. Determine `Id` using `TallerSeleccionado?.IdTaller ?? 0`.
   - Add `LimpiarFormulario()` method to reset form values.
   - Refactor `ToggleActivoAsync()` to update `TallerSeleccionado.Activo` and persist via `ModificarTallerHandler`.
2. `FrmTaller`:
   - In constructor for new Taller, call `vmTaller.LimpiarFormulario()`.
   - In constructor for edit Taller, populate `vmTaller` properties and set `TallerSeleccionado`.
3. `CtlTaller`:
   - Clean up code-behind to match XAML button bindings (`BtnNuevo_Click`, `BtnEditar_Click`, `BtnBorrar_Click`, `BtnBuscar_Click`).
   - Use `FrmConfirmacion.Mostrar(...)` for status toggle.
