# Role-Based Access Control Specification

## Purpose
Defines functional specifications for restricting application features and menu items based on the authenticated user's role (`Administrador`, `Recepcionista`, `Mecanico`, etc.).

## Requirements

### Requirement 1: Role Evaluation upon Login
- **SHALL** evaluate `UsuarioDto.RolNombre` immediately after authentication succeeds.
- **SHALL** apply menu visibility and capability restrictions before presenting `MainWindow` controls.

### Requirement 2: Role Access Mapping
- **Administrador**:
  - **SHALL** have full access to all sections: Dashboard, Operations (Work Orders, Services), Administration (Clients, Vehicles, Models, Brands, Workshops, Users, Roles), and Reports.
- **Recepcionista**:
  - **SHALL** have access to: Dashboard, Operations (Work Orders), Administration (Clients, Vehicles), and Reports (Work Orders Report).
  - **SHALL NOT** have access to system administration (Users, Roles, Workshop settings).
- **Mecanico**:
  - **SHALL** have access to: Dashboard, Operations (Work Orders assigned), and Reports (Work Orders Report).
  - **SHALL NOT** have access to Client editing, Vehicle deletion, User management, or Role configuration.

### Requirement 3: UI Visibility Control
- **SHALL** toggle `Visibility.Collapsed` on sidebar buttons that are prohibited for the logged-in role.
- **SHALL** prevent unauthorized navigation even if invoked programmatically.

---

## Scenarios (BDD Specifications)

### Scenario 1: Admin User Login
- **Given** an authenticated user with role `"Administrador"`
- **When** the main window menu loads
- **Then** all menu options (Dashboard, Operations, Administration, Reports, Users, Roles) SHALL be visible.

### Scenario 2: Receptionist User Login
- **Given** an authenticated user with role `"Recepcionista"`
- **When** the main window menu loads
- **Then** `btnUsuario`, `btnRol`, and `btnTaller` SHALL be hidden (`Visibility.Collapsed`)
- **And** Dashboard, Customers, Vehicles, and Work Orders Report SHALL remain visible and accessible.

### Scenario 3: Mechanic User Login
- **Given** an authenticated user with role `"Mecanico"`
- **When** the main window menu loads
- **Then** system administration buttons SHALL be hidden
- **And** Work Orders Report and Dashboard SHALL remain accessible.
