# Delta Specification: Reportes

## ADDED Requirements

### Requirement: Mechanic Productivity by Task
The system SHALL compute mechanic productivity for a date range at the **task** level, grouping each `DetalleServicio` by its assigned mechanic, and SHALL classify each task by the task's own state. Only users with the Mechanic role (`RolId = 3`) SHALL be listed, and tasks belonging to cancelled orders (`RegistroServicio.Estado = 'Cancelada'`) SHALL be excluded.

#### Scenario: Every mechanic with tasks appears once
- **WHEN** the Reporte Operativo is generated for a range in which two mechanics each have at least one task
- **THEN** the productivity table lists both mechanics, each exactly once

#### Scenario: Completion uses the task state
- **WHEN** a mechanic's task has state `Finalizada`
- **THEN** that task counts as completed, regardless of the parent order's state

#### Scenario: Breakdown columns
- **WHEN** a mechanic row is rendered
- **THEN** it shows `Pendientes`, `En Curso` and `Completadas` counts
- **AND** `Completadas + En Curso + Pendientes` equals `TareasAsignadas`

#### Scenario: Percent of progress
- **WHEN** the row is rendered
- **THEN** `% Avance` is `Completadas / TareasAsignadas` (0 when there are no tasks)

#### Scenario: Only mechanics with tasks in range
- **WHEN** a mechanic has no tasks in the selected date range
- **THEN** that mechanic is not listed

#### Scenario: Only users with the Mechanic role
- **WHEN** a user without the Mechanic role has tasks in the range
- **THEN** that user is not listed in the productivity table

#### Scenario: Cancelled orders are excluded
- **WHEN** a task belongs to an order whose state is `Cancelada`
- **THEN** that task is not counted for any mechanic

#### Scenario: Date range on the order date
- **WHEN** a date range is selected
- **THEN** only tasks whose order date (`RegistroServicio.Fecha`) falls within the range (inclusive) are counted

### Requirement: Productivity Totals
The system SHALL show column totals for the productivity table.

#### Scenario: Totals row
- **WHEN** one or more mechanics are listed
- **THEN** a totals row shows the sum of `TareasAsignadas`, `Pendientes`, `En Curso` and `Completadas` across mechanics

### Requirement: Reporte Operativo PDF Breakdown
The Reporte Operativo PDF SHALL include the productivity breakdown columns and the totals row.

#### Scenario: PDF mirrors the screen
- **WHEN** the report is exported to PDF
- **THEN** each mechanic row shows `Tareas Asignadas`, `Pendientes`, `En Curso`, `Completadas` and `% Avance`
- **AND** a totals row is present

### Requirement: Order Report Real Data Only
`ObtenerReporteOrdenesHandler` SHALL return only real order data and SHALL NOT fabricate demonstration rows when there are no rows or when an error occurs.

#### Scenario: No rows
- **WHEN** there are no orders
- **THEN** the order report is empty and no demonstration rows are returned

#### Scenario: Error
- **WHEN** an error occurs while loading the order report
- **THEN** the report is empty and no demonstration rows are returned
