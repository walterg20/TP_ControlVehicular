# Implementation Tasks: Mis Trabajos Panel

## Task 1: ViewModel Implementation
- **File:** `Presentacion/ViewModels/MisTrabajosViewModel.cs`
- **Actions:** 
  1. Create `MiTrabajoItemViewModel` helper class.
  2. Implement `MisTrabajosViewModel` extending `BaseViewModel`.
  3. Inject `ListarRegistroServiciosHandler`, `ModificarRegistroServicioHandler`.
  4. Implement `LoadAsync()` to fetch all active orders, flatten details where `UsuarioId == Session.IdUsuario`, and populate `ObservableCollection<MiTrabajoItemViewModel>`.
  5. Implement `GuardarCambiosAsync()` to iterate modified items, evaluate order status, and persist them.
  6. Implement properties for Search text and Status Filter (Pendientes/Finalizados/Todos) with a filtered `CollectionView`.

## Task 2: UI View Implementation
- **File:** `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml` & `.xaml.cs`
- **Actions:**
  1. Create UI with Header, Search TextBox, Status Filter ComboBox.
  2. Add `DataGrid` binding to the filtered list.
  3. Define columns: Order ID, Date, Patente, Task Name.
  4. Define editable columns for `Observaciones` (TextBox) and `Realizado` (CheckBox).
  5. Add "Ver Orden" Button column and "Guardar Cambios" Button at the bottom.
  6. Include Empty State visuals (Lupa + Text).
  7. In code-behind, call `vm.LoadAsync()` on load, and implement the "Ver Orden" click handler to instantiate `CtlOrdenServicioForm(orden, "MisTrabajos")`.

## Task 3: Navigation and Registration Updates
- **Files:** `Presentacion/MainWindow.xaml.cs`, `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml.cs`, `App.xaml.cs`
- **Actions:**
  1. In `MainWindow`, update `MenuItem_Click_MisTrabajos` to navigate to `CtlMisTrabajos`.
  2. In `CtlOrdenServicioForm.xaml.cs`, add the `origen` parameter to the constructor.
  3. In `CtlOrdenServicioForm.xaml.cs` -> `VolverAlListado()`, check `origen` to instantiate `CtlMisTrabajos` if it equals `"MisTrabajos"`.
  4. In `App.xaml.cs`, register `services.AddTransient<MisTrabajosViewModel>()`.
