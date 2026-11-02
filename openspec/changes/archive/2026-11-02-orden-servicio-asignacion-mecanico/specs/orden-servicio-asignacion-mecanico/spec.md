# Specification: Work Order Mechanic Assignment & Validation

## Capability: orden-servicio-asignacion-mecanico

The system SHALL enforce that every service item added to a work order has a valid assigned mechanic before persisting to the database, and SHALL default newly added service items to an available mechanic when created by a non-mechanic user.

### Scenario: Non-mechanic adds a service item
- **Given** a user logged in as Receptionist or Administrator
- **And** there are active mechanics loaded in `MecanicosDisponibles`
- **When** the user clicks "Agregar Ítem" to add a service to the order
- **Then** the newly added detail item SHALL have its `UsuarioId` set to the ID of the first available mechanic by default.

### Scenario: Attempting to save order with unassigned mechanic
- **Given** a work order in creation or editing mode
- **When** at least one service in `DetallesOrdenActual` has `UsuarioId <= 0`
- **And** the user attempts to save the order
- **Then** the system SHALL prevent the save operation
- **And** the system SHALL display a validation warning indicating that every service must have an assigned mechanic.

### Scenario: Loading active mechanics
- **Given** the work order form is opened
- **When** `LoadCombosAsync` is executed
- **Then** the repository query for mechanics SHALL retrieve active users with their associated role
- **And** only users with mechanic role (by name or ID) SHALL be added to `MecanicosDisponibles`.
