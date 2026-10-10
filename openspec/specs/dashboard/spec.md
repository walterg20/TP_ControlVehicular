# Dashboard Capability Specification

## Purpose
Defines the functional and visual specifications for the executive Dashboard feature in `TP_ControlVehicular`. The dashboard provides real-time overview metrics and a data grid listing vehicles currently managed in the workshop.

## Requirements

### Requirement 1: Sidebar Navigation Menu Placement
- **SHALL** display a new navigation entry `"📊 Dashboard"` in `MainWindow`'s sidebar menu.
- **SHALL** position the `"📊 Dashboard"` button at the top of the sidebar navigation menu, **above the OPERACIONES section**.
- **SHALL** open `CtlDashboard` inside `grdContenido` when clicked.

### Requirement 2: Summary Metric Cards (Top Section)
- **SHALL** display three distinct summary cards styled with rounded corners (`CornerRadius="12"` / `rounded-xl`) and drop shadows (`shadow-md`):
  1. **Vehículos Activos**: Total count of active vehicles registered in the workshop.
  2. **En Proceso**: Total count of active work orders currently in progress.
  3. **Entregadas Hoy**: Total count of completed work orders delivered on the current date.
- **SHALL** present clean typography, clear icons (e.g. 🚗, 📋, 🚚), and distinct accent colors (Green, Orange, Purple).

### Requirement 3: Workshop Vehicles Data Grid (Bottom Section)
- **SHALL** display a data grid titled `"Vehículos en el Taller"`.
- **SHALL** list all active workshop vehicles with columns:
  - Patente (License Plate)
  - Marca (Brand)
  - Modelo (Model)
  - Año (Year)
  - Cliente (Client Name / DNI)
  - Kilometraje (Current Odometer)
- **SHALL** provide a refresh button (`"🔄 Actualizar"`) to reload metrics asynchronously.

---

## Scenarios (BDD Specifications)

### Scenario 1: Navigating to Dashboard
- **Given** an authenticated user is on `MainWindow`
- **When** the user clicks on `"📊 Dashboard"` located above the OPERACIONES section
- **Then** the application SHALL clear `grdContenido`
- **And** inject `CtlDashboard` into `grdContenido`
- **And** automatically trigger `DashboardViewModel.CargarDashboardAsync()`.

### Scenario 2: Loading Metric Cards & Vehicle List
- **Given** `CtlDashboard` is loaded
- **When** `ObtenerDashboardHandler` queries the database
- **Then** it SHALL return the metric counts (`VehiculosActivos`, `EnProceso`, `EntregadasHoy`)
- **And** the list of workshop vehicles
- **And** the UI SHALL populate the three cards and the data grid smoothly.

### Scenario 3: Empty Vehicle List Fallback
- **Given** no vehicles are currently registered in the database
- **When** `CtlDashboard` loads
- **Then** the summary cards SHALL display `0`
- **And** the data grid SHALL display an empty list without throwing null reference exceptions.
