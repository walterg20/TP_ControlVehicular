# Technical Design: Mis Trabajos — Desktop Master–Detail

## Data Flow

```
MisTrabajosViewModel.LoadAsync()
   -> ListarRegistroServiciosHandler.HandleAsync()   // all orders + Detalles
   -> keep orders where Estado != "Cancelada" AND
      Detalles.Any(d => d.UsuarioId == currentUser)
   -> build MiOrdenItemViewModel (master) per order
        MiTareaItemViewModel (detail) per own Detalle, sharing the same
        DetalleServicioDto reference held by the order
   -> Ordenes (ObservableCollection<MiOrdenItemViewModel>)
   -> OrdenesView (ICollectionView, filtered by Mostrar + text)
   -> auto-select first order -> rebuild TareasDeOrden
```

The detail items wrap the **same** `DetalleServicioDto` instances contained in the order's `Detalles`, so `Guardar Cambios` can iterate all orders' tasks and persist consistent data.

## ViewModel

### `MiTareaItemViewModel` (detail row)
- Holds `RegistroServicioDto Orden`, `DetalleServicioDto Detalle`, and a back-reference `MiOrdenItemViewModel? OrdenPadre`.
- Exposes pass-through read-only props: `OrdenId`, `VehiculoPatente`, `VehiculoDetalle`, `VehiculoCompleto`, `TareaNombre`.
- `Realizado` and `Observaciones` write through to `Detalle`; on change set `IsModified = true` and notify. `Realizado` additionally calls `OrdenPadre?.RefrescarProgreso()` so the master's `Progreso` updates live.

### `MiOrdenItemViewModel` (master row)
- Holds `RegistroServicioDto Orden` and `ObservableCollection<MiTareaItemViewModel> MisTareas`.
- Read-only props for columns: `OrdenId`, `Fecha`, `VehiculoPatente`, `VehiculoDetalle`, `VehiculoCompleto`, `ClienteDetalle`, `KmIngreso`.
- Progress: `TotalTareas`, `TareasRealizadas`, `TareasPendientes`, `TienePendientes`, `Progreso` (`"{done}/{total}"`).
- `RefrescarProgreso()` raises `PropertyChanged` for the progress members.

### `MisTrabajosViewModel`
- `Ordenes` + `OrdenesView` (`CollectionViewSource.GetDefaultView`), filter `OrdenesFilter`.
- `TareasDeOrden` (`ObservableCollection<MiTareaItemViewModel>`) bound to the detail grid.
- `TextoBusqueda` and `FiltroEstado` (`Pendientes` default) refresh the view and re-evaluate `IsListEmpty`.
- `OrdenSeleccionada` setter rebuilds `TareasDeOrden` and notifies `PuedeImprimirHistorial`, `HayOrdenSeleccionada`, `NoHayOrdenSeleccionada`.
- `IsListEmpty` = `OrdenesView.IsEmpty`; `HayOrdenSeleccionada` / `NoHayOrdenSeleccionada` drive the detail placeholder.
- `OrdenesFilter`: 
  - `Pendientes` keeps orders with `TienePendientes`;
  - `Finalizados` keeps orders without pending tasks;
  - `Todos` keeps all;
  - free-text matches order id, plate or client (case-insensitive).
- `LoadAsync`: group own tasks by order, sort orders by `Id` descending, select the first visible order (preserve prior selection by id when available).
- `GuardarCambiosAsync`: collect `Ordenes.SelectMany(o => o.MisTareas).Where(t => t.IsModified)`, persist every affected order with the **existing** entity mapping and status derivation, clear `IsModified`, then `LoadAsync()`.
- `PuedeImprimirHistorial` = `PuedeVerReportesMecanico && OrdenSeleccionada != null`; the PDF uses `OrdenSeleccionada.Orden.VehiculoId`.

## UI (`CtlMisTrabajos.xaml`)

Rows: header (`Auto`) · filter bar (`Auto`) · master card (`3*`, min 140) · `GridSplitter` (`Auto`) · detail card (`4*`, min 160) · footer (`Auto`).

- Both cards follow the project convention: white `Border` `CornerRadius="12"`, `DropShadowEffect` opacity 0.08, dark DataGrid headers (`#2C3E50`).
- Master DataGrid `dgOrdenes`: `SelectedItem` two-way to `OrdenSeleccionada`, `MouseDoubleClick` opens the order.
- Detail card header shows `#{OrdenId} — VehiculoCompleto` and hosts the `Ver Orden Completa` button; body hosts `dgTareasDeOrden` with columns `Tarea`, `Observaciones` (editable TextBox), `Revisado / OK` (CheckBox). Placeholder visible when `NoHayOrdenSeleccionada`.
- Footer keeps `Tareas del Día`, `Imprimir Historial Clínico` (visibility `PuedeImprimirHistorial`) and `Guardar Cambios`.

## Code-behind (`CtlMisTrabajos.xaml.cs`)
- `BtnVerOrden_Click` uses `vm.OrdenSeleccionada?.Orden`.
- `DgOrdenes_MouseDoubleClick` opens the full order only when the source belongs to a `DataGridRow`.
