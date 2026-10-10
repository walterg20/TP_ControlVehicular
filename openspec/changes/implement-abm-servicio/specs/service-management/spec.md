# Delta Specification: ABM Servicio & Menu Reorganization

## Added Requirements

### Requirement: Menu Restructuring
- **SHALL** remove `btnRepuestosServicios` from OPERACIONES in `MainWindow.xaml`.
- **SHALL** add `btnServicio` (`"📦 Servicios"`) under ADMINISTRACIÓN in `MainWindow.xaml`.
- **SHALL** update `AplicarRestriccionesPorRol` to show `btnServicio` to `Administrador` and hide it from non-admin roles.

### Requirement: Service Catalog CRUD
- **SHALL** implement `IServicioRepository` providing `GetActivosAsync()` and `GetByIdAsync()`.
- **SHALL** implement handlers: `ListarServiciosHandler`, `RegistrarServicioHandler`, `ModificarServicioHandler`, `EliminarServicioHandler`.
- **SHALL** build `CtlServicio.xaml` with DataGrid listing ID, Nombre, Precio, and Estado.
- **SHALL** build `FrmServicio.xaml` modal dialog with input validation (Nombre non-empty, Precio > 0).
