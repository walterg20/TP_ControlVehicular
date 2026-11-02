# Design Document: orden-servicio-asignacion-mecanico

## Architecture Overview
The change affects the Work Order form presentation layer (`CtlOrdenServicioForm`) and the corresponding ViewModel (`OrdenServicioViewModel`).

```mermaid
flowchart TD
    A[Receptionist adds Service] --> B{Active mechanics in MecanicosDisponibles?}
    B -- Yes --> C[Set UsuarioId = MecanicosDisponibles[0].IdUsuario]
    B -- No --> D[Set UsuarioId = 0]
    C --> E[User can change mechanic in ComboBox]
    D --> E
    E --> F[User clicks Guardar]
    F --> G{Any item has UsuarioId <= 0?}
    G -- Yes --> H[Show Alert: Debe asignar un mecánico a cada servicio]
    G -- No --> I[Proceed to GuardarOrdenAsync]
    I --> J{ViewModel validation passes?}
    J -- No --> K[Abort with error message]
    J -- Yes --> L[Save via Handler to Database]
```

## Detailed Changes

### 1. `OrdenServicioViewModel.cs`
- `LoadCombosAsync`: Change `_usuarioRepository.GetAllAsync()` to `_usuarioRepository.GetActivosAsync()`.
- `GuardarOrdenAsync`: Before mapping and dispatching to `_registrarHandler` / `_modificarHandler`, check:
  ```csharp
  if (DetallesOrdenActual.Any(d => d.UsuarioId <= 0))
  {
      RegistrationFailed?.Invoke(this, "Debe asignar un mecánico a cada servicio de la orden.");
      return false;
  }
  ```

### 2. `CtlOrdenServicioForm.xaml.cs`
- `BtnAgregarItem_Click`: When creating a new `DetalleServicioDto`:
  ```csharp
  int idMecanicoAuto = 0;
  if (Application.Current.MainWindow is MainWindow mWin && mWin.UsuarioSesionActual != null)
  {
      bool esMecanico = mWin.UsuarioSesionActual.RolNombre?.IndexOf("Mec", StringComparison.OrdinalIgnoreCase) >= 0 || mWin.UsuarioSesionActual.IdRol == 3;
      if (esMecanico)
          idMecanicoAuto = mWin.UsuarioSesionActual.IdUsuario;
  }
  if (idMecanicoAuto == 0 && vmOrdenServicioForm.MecanicosDisponibles.Count > 0)
  {
      idMecanicoAuto = vmOrdenServicioForm.MecanicosDisponibles[0].IdUsuario;
  }
  ```
- `BtnGuardar_Click`:
  ```csharp
  if (vmOrdenServicioForm.DetallesOrdenActual.Any(d => d.UsuarioId <= 0))
  {
      Compartido.FrmConfirmacion.MostrarAviso("Debe asignar un mecánico a cada servicio de la orden antes de guardar.", "Validación", Window.GetWindow(this));
      return;
  }
  ```
