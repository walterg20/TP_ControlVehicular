# Service Management (ABM Servicio) Capability Specification

## Purpose
Defines functional and technical specifications for managing catalog services (`Servicio` entity: `Nombre`, `Precio`, `Activo`) in the `TP_ControlVehicular` application.

## Requirements

### Requirement 1: Menu Reorganization
- **SHALL** add the `"📦 Servicios"` ABM button in `MainWindow.xaml` under the **ADMINISTRACIÓN** section.
- **SHALL** remove the placeholder button `"📦 Repuestos / Servicios"` from the **OPERACIONES** section.
- **SHALL** restrict visibility of `"📦 Servicios"` based on user role (visible to `Administrador`).

### Requirement 2: Catalog CRUD Operations
- **SHALL** list all registered services in `CtlServicio.xaml` with columns: ID, Nombre, Precio, Estado (Activo/Inactivo).
- **SHALL** support searching/filtering services by name.
- **SHALL** support creating new catalog services via modal window `FrmServicio.xaml` (`RegistrarServicioHandler`).
- **SHALL** support modifying existing catalog services via modal window `FrmServicio.xaml` (`ModificarServicioHandler`).
- **SHALL** support soft-deleting/toggling active status (`EliminarServicioHandler` / `Activo = false`).

---

## Scenarios (BDD Specifications)

### Scenario 1: Accessing Services ABM
- **Given** an authenticated Administrator user
- **When** the user clicks on `"📦 Servicios"` in the ADMINISTRACIÓN section
- **Then** `MainWindow` SHALL load `CtlServicio` into `grdContenido`
- **And** `CtlServicio` SHALL automatically fetch and display all catalog services.

### Scenario 2: Registering a New Service
- **Given** the user is on `CtlServicio`
- **When** the user clicks `"Nuevo Servicio"`
- **Then** the application SHALL display `FrmServicio` modal
- **When** the user enters name `"Cambio de aceite y filtro"` and price `45000` and clicks `"Guardar"`
- **Then** `RegistrarServicioHandler` saves the entity to the database via `IServicioRepository`
- **And** `CtlServicio` reloads the list.

### Scenario 3: Modifying an Existing Service
- **Given** a service `"Cambio de correa"` exists
- **When** the user selects the service and clicks `"Editar"`
- **Then** `FrmServicio` opens populated with the current name and price
- **When** the user updates the price to `85000` and saves
- **Then** `ModificarServicioHandler` updates the database entity.
