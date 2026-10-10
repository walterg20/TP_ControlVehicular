# Delta Specification: Implement Executive Dashboard

## Added Requirements

### Requirement: Navigation Menu Entry
- **SHALL** add `btnDashboard` (`"📊 Dashboard"`) in `MainWindow.xaml` positioned above the OPERACIONES section.
- **SHALL** handle click event to inject `CtlDashboard` into `grdContenido`.

### Requirement: Summary Metric Cards
- **SHALL** render 3 summary cards in `CtlDashboard.xaml`:
  - **Vehículos Activos**: Bound to `VehiculosActivosCount`.
  - **En Proceso**: Bound to `EnProcesoCount`.
  - **Entregadas Hoy**: Bound to `EntregadasHoyCount`.
- **SHALL** apply design tokens: `CornerRadius="12"` (`rounded-xl`), `DropShadowEffect` (`shadow-md`), `Padding="16"`, `Margin="8"`.

### Requirement: Workshop Vehicles Data Grid
- **SHALL** render a DataGrid bound to `VehiculosEnTaller` (`ObservableCollection<VehiculoDto>`).
- **SHALL** include columns for Patente, Marca, Modelo, Año, Cliente, and Km Actual.
