# Tasks: orden-servicio-asignacion-mecanico

- [x] 1. Update `OrdenServicioViewModel.cs` <!-- id: 1 -->
  - [x] 1.1 Use `_usuarioRepository.GetActivosAsync()` in `LoadCombosAsync` <!-- id: 1.1 -->
  - [x] 1.2 Add check for `DetallesOrdenActual.Any(d => d.UsuarioId <= 0)` in `GuardarOrdenAsync` <!-- id: 1.2 -->
- [x] 2. Update `CtlOrdenServicioForm.xaml.cs` <!-- id: 2 -->
  - [x] 2.1 Default `idMecanicoAuto` to the first available mechanic when user is receptionist/admin <!-- id: 2.1 -->
  - [x] 2.2 Add user-friendly warning dialog in `BtnGuardar_Click` if any detail lacks mechanic <!-- id: 2.2 -->
- [x] 3. Automated Tests <!-- id: 3 -->
  - [x] 3.1 Add unit tests in `TP_ControlVehicular.Tests` verifying validation and combo loading <!-- id: 3.1 -->
  - [x] 3.2 Verify test suite passes <!-- id: 3.2 -->
