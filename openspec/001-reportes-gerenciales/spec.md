# Specification: Management and Historical Reports

## User Story
**As an** Administrator
**I want to** view revenue and resolution time reports across all workshops
**So that** I can track performance and make business decisions.

## Acceptance Criteria
1. **Dashboard Access**: The system must provide a Dashboard view (`CtlDashboard`) accessible from the main navigation.
2. **Revenue Report**: The dashboard must display a Revenue Report populated by the `sp_ReporteIngresos` stored procedure.
3. **Resolution Time Report**: The dashboard must display a Resolution Time Report populated by the `sp_ReporteTiemposResolucion` stored procedure.
4. **Global Filters**: The dashboard must include filters for 'Start Date', 'End Date', and 'Workshop'.
5. **Role-Based Filtering**: 
   - If the logged-in user is an **Administrator**, all filters are enabled.
   - If the logged-in user is **not an Administrator**, the 'Workshop' filter must be locked/hidden, and inherently filtered to their assigned workshop.

## BDD Scenarios

**Scenario: Administrator views reports across all workshops**
- **Given** an Administrator is logged into the system
- **When** they navigate to the Dashboard
- **Then** they should see the Revenue and Resolution Time reports
- **And** the 'Workshop' filter should be visible and selectable
- **And** the data should reflect the selected workshop (or all if left blank/all).

**Scenario: Non-Administrator views reports for their workshop**
- **Given** a non-administrator user (e.g., Workshop Manager) is logged into the system
- **When** they navigate to the Dashboard
- **Then** the 'Workshop' filter should be hidden or disabled
- **And** the reports should only display data pertaining to their assigned workshop.
