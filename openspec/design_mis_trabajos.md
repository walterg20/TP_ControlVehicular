# Technical Design: Mis Trabajos Panel

## 1. Architecture & Data Flow
The "Mis Trabajos" panel will be implemented using the existing MVVM structure in the `Presentacion` layer.

### 1.1 Data Fetching
- We will reuse the `ListarRegistroServiciosHandler` to fetch all active orders.
- In the `MisTrabajosViewModel`, we will flatten the `DetalleServicioDto` lists from all orders, filtering by `UsuarioId == CurrentUsuarioId` and `Estado != "Cancelada"`.
- We will project these into a new helper class `MiTrabajoItemViewModel` (or just an observable class) that wraps the `DetalleServicioDto` with order-level properties like `VehiculoPatente`, `Fecha`, and the parent `RegistroServicioDto`.

### 1.2 State Updates
- When the mechanic toggles the "Revisado / OK" checkbox or changes the "Observaciones" text box and loses focus, a command `GuardarCambioCommand` will be triggered.
- This command will:
  1. Retrieve the parent `RegistroServicioDto`.
  2. Invoke `EvaluarEstadoGeneral()` to update the order's status if all items are completed.
  3. Map the updated order to the Entity `RegistroServicio`.
  4. Call `ModificarRegistroServicioHandler` to persist changes.
  5. Refresh the list.

### 1.3 Routing / Navigation
- A "Ver Orden" (View Order) button in the data grid will allow the mechanic to navigate to the full order form.
- The constructor of `CtlOrdenServicioForm` will be modified to accept an `origen` parameter: `public CtlOrdenServicioForm(RegistroServicioDto orden, string origen = "General")`.
- `VolverAlListado()` in `CtlOrdenServicioForm` will check this `origen` flag and instantiate `CtlMisTrabajos` if it came from there, otherwise it defaults to `CtlOrdenServicio`.

## 2. Components to Create/Modify

### 2.1 ViewModels
* **[NEW]** `Presentacion/ViewModels/MisTrabajosViewModel.cs`: Will handle data fetching, filtering (Todos/Pendientes/Finalizados), and saving changes.
* **[NEW]** `MiTrabajoItem`: Helper class inside the viewmodel namespace to hold flattened item data.

### 2.2 Views
* **[NEW]** `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml` & `.xaml.cs`: The UserControl UI. Will contain:
  - Header & Search bar.
  - RadioButtons or ComboBox for the Quick Filter (Pendientes/Finalizados/Todos).
  - DataGrid showing: Order N°, Fecha, Patente, Tarea, Observaciones (TextBox), Revisado (CheckBox), and a "Ver Orden" Button.

### 2.3 Navigation Updates
* **[MODIFY]** `Presentacion/MainWindow.xaml.cs`: Ensure `MenuItem_Click_MisTrabajos` instantiates `CtlMisTrabajos`.
* **[MODIFY]** `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml.cs`: Update constructor and `VolverAlListado()` to support returning to "Mis Trabajos".
* **[MODIFY]** `App.xaml.cs`: Register `MisTrabajosViewModel` in the DI container (`services.AddTransient<MisTrabajosViewModel>()`).

## 3. UX / UI Details
- The DataGrid should follow the same styling as `CtlOrdenServicio.xaml` (white card, rounded corners, shadow, blue header).
- An empty state should be displayed when no tasks match the filter.
- Actions (check, observe) must be intuitive and save automatically or via a distinct "Guardar Cambios" button at the bottom of the grid to batch save (we will go with a distinct "Guardar Cambios" button to avoid complex LostFocus bindings on the grid and ensure EF transaction safety).
