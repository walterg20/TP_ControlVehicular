# Delta Specification: Work Orders Report Submenu

## Added Requirements

### Requirement: Work Orders Report Submenu Entry
- **SHALL** add a `"REPORTES"` section in `MainWindow.xaml` sidebar menu.
- **SHALL** add a `"📋 Órdenes de Trabajo"` button under `REPORTES`.
- **SHALL** open `CtlReporteOrdenes` in `grdContenido` upon click.

### Requirement: Work Order Information Fields
- **SHALL** display a comprehensive DataGrid in `CtlReporteOrdenes.xaml` containing:
  - **N° Orden**: Unique order ID
  - **Fecha Ingreso**: Arrival timestamp
  - **Vehículo**: License plate and Brand - Model (e.g. `[AB123CD] Toyota - Corolla`)
  - **Cliente**: Client full name and DNI
  - **Km Ingresado**: Odometer reading at reception
  - **Servicios Aplicados**: List of services performed (e.g. `"Cambio de aceite, Cambio de correa, Filtros"`)
  - **Mecánico Asignado**: Full name of assigned mechanic
  - **Recepcionista**: Full name of receiving staff member
  - **Estado**: Status (`Pendiente`, `En Proceso`, `Completada`, `Entregada`)

### Requirement: Filtering & Search
- **SHALL** provide search inputs to filter report rows by Client name, License Plate, or Mechanic name.
- **SHALL** provide a status filter dropdown.
