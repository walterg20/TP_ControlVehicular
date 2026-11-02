# Change Proposal: orden-servicio-asignacion-mecanico

## Summary
Prevent Foreign Key constraint violations on `DetalleServicio.UsuarioId` (`FK_DetalleServicio_Usuarios_UsuarioId`) when receptionists or administrators register work orders with services, by auto-selecting an active mechanic by default and enforcing validation checks before saving.

## Problem Statement
When a user with the Receptionist or Administrator role registers a work order and adds a service line item, `idMecanicoAuto` was initialized to `0` because the user is not a mechanic. If the receptionist forgets or neglects to select a mechanic from the DataGrid's "Mecánico" column ComboBox:
- The item retains `UsuarioId = 0`.
- Validation checks (`ValidateAll`) pass without verifying `UsuarioId` on child details.
- Upon saving, EF Core attempts to insert a row into `DetalleServicio` with `UsuarioId = 0`, causing SQL Server to abort the transaction with:
  `The INSERT statement conflicted with the FOREIGN KEY constraint "FK_DetalleServicio_Usuarios_UsuarioId". The conflict occurred in database "ControlVehicular", table "dbo.Usuarios", column 'Id'.`

## Proposed Solution
1. **Auto-Assignment by Default**: When adding an item as a receptionist/admin, if active mechanics exist in `MecanicosDisponibles`, assign the first mechanic by default (`MecanicosDisponibles[0].IdUsuario`).
2. **Defensive Validation in View & ViewModel**:
   - In `CtlOrdenServicioForm.xaml.cs` (`BtnGuardar_Click`), verify that all details have `UsuarioId > 0` and display an informative dialog (`FrmConfirmacion.MostrarAviso`) if any detail lacks an assigned mechanic.
   - In `OrdenServicioViewModel.cs` (`GuardarOrdenAsync` / `ValidateAll`), ensure child details cannot be saved with `UsuarioId <= 0`.
3. **Robust Mechanics Query**: In `OrdenServicioViewModel.cs` (`LoadCombosAsync`), fetch active users with `_usuarioRepository.GetActivosAsync()` so that navigation property `u.Rol` is included and inactive users are excluded.
