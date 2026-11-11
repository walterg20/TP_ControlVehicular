# Spec Delta

## MODIFIED Requirements

### Requirement: Summary Metric Cards (Top Section)
The system SHALL display three summary cards styled with rounded corners (`CornerRadius="12"` / `rounded-xl`) and drop shadows (`shadow-md`):
1. **Vehículos Activos**: the number of distinct vehicles that have at least one work order in {`Abierta`, `En Proceso`}.
2. **En Proceso**: the number of work orders in {`Abierta`, `En Proceso`}.
3. **Entregadas Hoy**: the number of work orders in {`Completada`, `Pagada`} whose order date is the current date (the model has no delivery timestamp, so the order date is used as an approximation).

Every metric SHALL be computed from real work-order data. The system SHALL NOT fabricate, randomly generate, or derive any metric by applying fixed factors to another metric, and SHALL NOT fall back to demonstration data.

The cards SHALL present clean typography, clear icons (e.g. 🚗, 📋, 🚚), and distinct accent colors (green, orange, purple).

The **Vehículos Activos** card counts the same distinct vehicles that the workshop-vehicles grid used to list. Since the dashboard's bottom section now shows role-based charts instead of that grid, the card SHALL remain as the place where that figure is surfaced.

#### Scenario: Loading Metric Cards
- **GIVEN** `CtlDashboard` is loaded
- **WHEN** `ObtenerDashboardHandler` queries the database
- **THEN** it SHALL return the metric counts (`VehiculosActivos`, `EnProceso`, `EntregasHoy`)
- **AND** the UI SHALL populate the three cards and the role-based charts smoothly.

#### Scenario: Empty Data Fallback
- **GIVEN** no work orders are currently registered in the database
- **WHEN** `CtlDashboard` loads
- **THEN** the summary cards SHALL display `0`
- **AND** the charts SHALL render an empty state without throwing null reference exceptions.

#### Scenario: Metrics reflect real work orders
- **GIVEN** work orders exist in the database in known states
- **WHEN** `ObtenerDashboardHandler` computes the metric cards
- **THEN** each card SHALL equal the count derived from those work orders
- **AND** no card SHALL be derived from another card by a fixed factor.

### Requirement: Role-Based Visual Analytics (Bottom Section)
The system SHALL replace the workshop-vehicles data grid in the dashboard's bottom section with a set of visualizations that depend on the authenticated user's role. Exactly one set SHALL be visible per role.

- **Administrador**: the three managerial report visuals — income by month (vertical bars), most repaired models (horizontal bars) and services by quantity (pie chart with legend).
- **Recepcionista**: a bar chart with the work orders grouped by state (`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`).
- **Mecánico**: a bar chart with the mechanic's own tasks grouped by state (`Pendiente`, `En Curso`, `Finalizada`) plus a progress ring showing the percentage of completed tasks.

The section's title SHALL reflect the visible content. The visualizations SHALL be drawn with native WPF controls and SHALL NOT require any additional NuGet package.

#### Scenario: Administrador sees the managerial charts
- **GIVEN** the authenticated user has the Administrador role
- **WHEN** the dashboard loads
- **THEN** the bottom section SHALL show the income-by-month, most-repaired-models and services-by-quantity charts
- **AND** SHALL NOT show the reception or the mechanic charts.

#### Scenario: Recepcionista sees the work orders by state
- **GIVEN** the authenticated user has the Recepcionista role
- **WHEN** the dashboard loads
- **THEN** the bottom section SHALL show a chart with the work orders grouped by state
- **AND** SHALL NOT show the managerial charts nor the mechanic chart.

#### Scenario: Mecánico sees their own tasks and progress
- **GIVEN** the authenticated user has the Mecánico role
- **WHEN** the dashboard loads
- **THEN** the bottom section SHALL show a chart with the mechanic's own tasks grouped by state
- **AND** a progress ring with the percentage of completed tasks
- **AND** SHALL NOT show the managerial charts nor the reception chart.

#### Scenario: Refresh button reloads metrics and charts
- **WHEN** the user clicks the `"🔄 Actualizar"` button
- **THEN** the dashboard SHALL reload the metric cards and the role-based charts asynchronously.

#### Scenario: Charts show no data without failing
- **GIVEN** there are no records for the role's data set
- **WHEN** the dashboard loads
- **THEN** each chart SHALL render an empty-state message
- **AND** the application SHALL NOT throw.

### Requirement: Role-Based Dashboard Scoping
The system SHALL scope the dashboard data according to the authenticated user's role:
- **Administrador** and **Recepcionista** SHALL see the whole-workshop data.
- **Mecánico** SHALL see only the work orders in which they participate (orders that have at least one service detail assigned to that mechanic).

All metric cards and the role-based charts SHALL use the same role-scoped data set, so the cards and the charts are always consistent with each other. A role SHALL never be shown charts computed from data outside its scope.

#### Scenario: Administrador sees the whole workshop
- **GIVEN** the authenticated user has the Administrador role
- **WHEN** the dashboard loads
- **THEN** the metrics and the managerial charts SHALL include every work order of the workshop.

#### Scenario: Recepcionista sees the whole workshop
- **GIVEN** the authenticated user has the Recepcionista role
- **WHEN** the dashboard loads
- **THEN** the metrics and the work-orders-by-state chart SHALL include every work order of the workshop.

#### Scenario: Mecánico sees only their own orders
- **GIVEN** the authenticated user has the Mecánico role
- **WHEN** the dashboard loads
- **THEN** the metrics and the tasks chart SHALL include only tasks assigned to that mechanic
- **AND** SHALL exclude tasks assigned to other mechanics.

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

## REMOVED Requirements

### Requirement: Workshop Vehicles Data Grid (Bottom Section)
This requirement is removed: the dashboard's bottom section now shows role-based charts instead of a data grid. The distinct vehicles with an active work order remain available in the *Órdenes de Trabajo* screen, which supports search, state filtering and date range filtering.
