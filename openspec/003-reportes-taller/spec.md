# Specification: Workshop Job Management and Reports

## 1. Feature Description
Mechanics need the ability to register `DetalleServicio` (parts used/tasks done) for their assigned services via a new view `CtlMisTrabajos`. They also need a report "Performed Services" (using `sp_ServiciosPorMecanico`) filtered by date range. A critical business rule dictates that a Mechanic (Role = 3) must only be able to see their own reports.

## 2. Acceptance Criteria
- **AC1:** Mechanics can access the `CtlMisTrabajos` view to add `DetalleServicio` to their assigned services.
- **AC2:** A "Performed Services" report is available, utilizing the `sp_ServiciosPorMecanico` stored procedure.
- **AC3:** The report accepts `FechaDesde`, `FechaHasta`, and `MecanicoId` as filters.
- **AC4:** **Role-Based Filtering:**
  - If the logged-in user is a Mechanic (Role = 3), the UI must hide or disable the `MecanicoId` filter, and the backend/handler must forcefully inject the user's ID into the query.
  - If the logged-in user is an Administrator, the UI must allow selecting any mechanic for the `MecanicoId` filter.

## 3. BDD Scenarios

### Scenario 1: Mechanic views their own services report
**Given** I am logged in as a Mechanic (Role = 3)
**When** I navigate to the "Performed Services" report
**Then** I should not see the option to select a mechanic
**And** the report should automatically display only my services based on my User ID.

### Scenario 2: Administrator views a mechanic's services report
**Given** I am logged in as an Administrator
**When** I navigate to the "Performed Services" report
**Then** I should see a dropdown to select a specific mechanic
**And** I can generate a report for the selected mechanic.

### Scenario 3: Mechanic adds service details
**Given** I am logged in as a Mechanic
**And** I am on the `CtlMisTrabajos` view
**When** I select an assigned service and add a `DetalleServicio`
**Then** the details should be saved successfully and associated with my service.
