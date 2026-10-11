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
1. **Vehículos Activos**: total count of active vehicles registered in the workshop.
2. **En Proceso**: total count of active work orders currently in progress.
3. **Entregadas Hoy**: total count of completed work orders delivered on the current date.

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

### Requirement: Workshop Vehicles Data Grid (Bottom Section)
The system SHALL display a data grid titled `"Vehículos en el Taller"` listing all active workshop vehicles with the columns Patente (license plate), Marca (brand), Modelo (model), Año (year), Cliente (client name / DNI) and Kilometraje (current odometer). The grid SHALL provide a refresh button (`"🔄 Actualizar"`) that reloads the metrics asynchronously.
