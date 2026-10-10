# Proposal: Role-Based Menu Access Restrictions & Work Orders Report Submenu

## Why
1. Security & Role Segregation: Currently, all users see the complete administration menu (Users, Roles, Workshops). The system must restrict menu options dynamically based on the assigned role (`Administrador`, `Recepcionista`, `Mecanico`).
2. Operations Reporting: Operational staff and managers need a dedicated report view (`Reportes -> Órdenes de Trabajo`) tracking vehicle arrivals, odometer readings, assigned mechanics, receiving receptionists, and applied services (e.g. oil change, belt replacement, filters).

## What
1. **Role-Based Access Control (RBAC)**: Implement dynamic menu filtering in `MainWindow` based on `UsuarioDto.RolNombre`.
2. **Work Orders Domain Entities & DTOs**: Define `OrdenTrabajo`, `Servicio`, and `DetalleOrdenTrabajo` entities/DTOs capturing vehicle reception, odometer reading, receptionist ID, mechanic ID, and applied services list.
3. **Work Orders Report Submenu (`CtlReporteOrdenes.xaml`)**: Create a new report view in a `"REPORTES"` section of `MainWindow` displaying comprehensive work order details:
   - Vehicle (Brand - Model [License Plate])
   - Client Name & DNI
   - Entry Odometer Reading (Km Ingresado)
   - Applied Services (e.g. "Cambio de aceite", "Cambio de correa", "Filtros")
   - Assigned Mechanic
   - Receiving Receptionist

## Out of Scope
- Financial invoicing or payment gateway integrations.
- Exporting reports to external PDF/Excel formats (can be added in future iterations).
