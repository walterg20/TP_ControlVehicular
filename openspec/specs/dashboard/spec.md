# Dashboard Capability Specification

## Purpose
Defines the functional and visual specifications for the executive Dashboard feature in `TP_ControlVehicular`. The dashboard provides real-time overview metrics and a data grid listing vehicles currently managed in the workshop.

## Requirements

### Requirement: Sidebar Navigation Menu Placement
The system SHALL display a `"📊 Dashboard"` navigation entry in `MainWindow`'s sidebar menu, positioned at the top of the navigation menu, above the OPERACIONES section. Selecting the entry SHALL open `CtlDashboard` inside `grdContenido`.

#### Scenario: Navigating to Dashboard
- **GIVEN** an authenticated user is on `MainWindow`
- **WHEN** the user clicks on `"📊 Dashboard"` located above the OPERACIONES section
- **THEN** the application SHALL clear `grdContenido`
- **AND** inject `CtlDashboard` into `grdContenido`
- **AND** automatically trigger `DashboardViewModel.CargarDashboardAsync()`.

### Requirement: Summary Metric Cards (Top Section)
The system SHALL display three summary cards styled with rounded corners (`CornerRadius="12"` / `rounded-xl`) and drop shadows (`shadow-md`):
1. **Vehículos Activos**: the number of distinct vehicles that have at least one work order in {`Abierta`, `En Proceso`}. It SHALL match the number of rows shown in the workshop vehicles grid.
2. **En Proceso**: the number of work orders in {`Abierta`, `En Proceso`}.
3. **Entregadas Hoy**: the number of work orders in {`Completada`, `Pagada`} whose order date is the current date (the model has no delivery timestamp, so the order date is used as an approximation).

Every metric SHALL be computed from real work-order data. The system SHALL NOT fabricate, randomly generate, or derive any metric by applying fixed factors to another metric, and SHALL NOT fall back to demonstration data.

The cards SHALL present clean typography, clear icons (e.g. 🚗, 📋, 🚚), and distinct accent colors (green, orange, purple).

#### Scenario: Loading Metric Cards & Vehicle List
- **GIVEN** `CtlDashboard` is loaded
- **WHEN** `ObtenerDashboardHandler` queries the database
- **THEN** it SHALL return the metric counts (`VehiculosActivos`, `EnProceso`, `EntregadasHoy`)
- **AND** the list of workshop vehicles
- **AND** the UI SHALL populate the three cards and the data grid smoothly.

#### Scenario: Empty Vehicle List Fallback
- **GIVEN** no vehicles are currently registered in the database
- **WHEN** `CtlDashboard` loads
- **THEN** the summary cards SHALL display `0`
- **AND** the data grid SHALL display an empty list without throwing null reference exceptions.

#### Scenario: Metrics reflect real work orders
- **GIVEN** work orders exist in the database in known states
- **WHEN** `ObtenerDashboardHandler` computes the metric cards
- **THEN** each card SHALL equal the count derived from those work orders
- **AND** no card SHALL be derived from another card by a fixed factor.

### Requirement: Workshop Vehicles Data Grid (Bottom Section)
The system SHALL display a data grid titled `"Vehículos en el Taller"` listing the distinct vehicles that have at least one work order in {`Abierta`, `En Proceso`}, with the columns Patente (license plate), Marca (brand), Modelo (model), Año (year), Cliente (client name / DNI) and Kilometraje (current odometer). The grid SHALL provide a refresh button (`"🔄 Actualizar"`) that reloads the metrics asynchronously.

#### Scenario: Grid lists active workshop vehicles
- **WHEN** `CtlDashboard` loads for a user who sees the whole workshop
- **THEN** the grid SHALL list each distinct vehicle with an order in {`Abierta`, `En Proceso`} exactly once.

#### Scenario: Refresh button reloads metrics
- **WHEN** the user clicks the `"🔄 Actualizar"` button
- **THEN** the dashboard SHALL reload the metric cards and the vehicle grid asynchronously.

### Requirement: Role-Based Dashboard Scoping
The system SHALL scope the dashboard data according to the authenticated user's role:
- **Administrador** and **Recepcionista** SHALL see the whole-workshop data.
- **Mecánico** SHALL see only the work orders in which they participate (orders that have at least one service detail assigned to that mechanic).

All metric cards and the vehicle grid SHALL use the same role-scoped data set, so the cards and the grid are always consistent with each other.

#### Scenario: Administrador sees the whole workshop
- **GIVEN** the authenticated user has the Administrador role
- **WHEN** the dashboard loads
- **THEN** the metrics and the vehicle grid SHALL include every work order of the workshop.

#### Scenario: Recepcionista sees the whole workshop
- **GIVEN** the authenticated user has the Recepcionista role
- **WHEN** the dashboard loads
- **THEN** the metrics and the vehicle grid SHALL include every work order of the workshop.

#### Scenario: Mecánico sees only their own orders
- **GIVEN** the authenticated user has the Mecánico role
- **WHEN** the dashboard loads
- **THEN** the metrics and the vehicle grid SHALL include only work orders that have at least one service detail assigned to that mechanic
- **AND** SHALL exclude work orders in which the mechanic does not participate.

### Requirement: Monthly Revenue Metric (Admin)
The system SHALL display an additional **Ingresos del Mes** card, visible only to the **Administrador**, showing the billed income summed from the first day of the current month through the current date. The revenue SHALL be read from the managerial income source (`sp_ReporteIngresosAdmin` via `IReporteGerencialRepository.ObtenerIngresosPorFechaAsync`). For non-Administrador roles the card SHALL NOT be shown.

#### Scenario: Administrador sees monthly revenue
- **GIVEN** the authenticated user has the Administrador role
- **WHEN** the dashboard loads
- **THEN** the **Ingresos del Mes** card SHALL be visible
- **AND** it SHALL show the sum of the income from the first day of the current month through today.

#### Scenario: Non-Administrador does not see monthly revenue
- **GIVEN** the authenticated user has the Recepcionista or Mecánico role
- **WHEN** the dashboard loads
- **THEN** the **Ingresos del Mes** card SHALL NOT be visible.
