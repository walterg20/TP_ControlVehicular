# Work Orders & Service Registration Specification

## Purpose
Defines functional and technical specifications for service registration (`RegistroServicio` & `DetalleServicio`) and the Work Orders Report based on the course-approved database schema.

## Requirements

### Requirement 1: Database Schema Compliance
- **SHALL** model entities according to the course-approved ER diagram:
  - `RegistroServicio`: `Id`, `VehiculoId`, `TallerId`, `UsuarioId` (Recepcionista - Recepciona), `Fecha`, `KmIngreso`, `Estado`.
  - `DetalleServicio`: `Id`, `RegistroServicioId`, `UsuarioId` (Mecánico - Confecciona), `ServicioId`, `Cantidad`, `Precio`, `Origen`, `Estado`.
  - `Servicio`: `Id`, `Nombre`, `Precio`, `Activo`.

### Requirement 2: Work Orders Report Submenu
- **SHALL** add a `"REPORTES"` section in `MainWindow.xaml` sidebar menu.
- **SHALL** add a `"📋 Órdenes de Trabajo"` button (`btnReporteOrdenes`).
- **SHALL** load `CtlReporteOrdenes` in `grdContenido`.

### Requirement 3: Data Mapping for Report
- **SHALL** display:
  - **Vehículo**: License plate and Brand - Model (e.g. `[AB123CD] Toyota - Corolla`).
  - **Cliente**: Client Full Name and DNI.
  - **Km Ingresado**: Odometer reading at vehicle entry (`RegistroServicio.KmIngreso`).
  - **Servicios Aplicados**: Aggregated list of service names performed (e.g., `"Cambio de aceite, Cambio de correa, Filtros"`).
  - **Mecánico**: Full name of assigned mechanic (`DetalleServicio.Usuario.Nombre`).
  - **Recepcionista**: Full name of receptionist who logged the entry (`RegistroServicio.Usuario.Nombre`).
  - **Fecha & Estado**: Entry timestamp and registration status.

---

## Scenarios (BDD Specifications)

### Scenario 1: Generating Work Orders Report
- **Given** service registrations exist with details assigned to mechanics
- **When** the user opens `CtlReporteOrdenes`
- **Then** the handler queries `RegistroServicio` including `Vehiculo.Cliente`, `Vehiculo.Modelo.Marca`, `Usuario` (Recepcionista), and `DetallesServicio` with `Servicio` and `Usuario` (Mecánico)
- **And** populates the report DataGrid cleanly.
