# Delta Specification: Mis Trabajos — Desktop Master–Detail

## ADDED Requirements

### Requirement: Master Grid of Orders
The system SHALL display the mechanic's orders in a master grid where each order appears exactly once, showing only the logged-in user's tasks, sorted by order number descending and excluding cancelled orders.

#### Scenario: One row per order with own tasks
- **WHEN** the logged-in mechanic opens the Mis Trabajos screen
- **THEN** the master grid shows exactly one row per order that has at least one task assigned to that mechanic
- **AND** orders whose `Estado` is `Cancelada` are excluded
- **AND** the rows are sorted by order number descending (newest first)

#### Scenario: Progress reflects the mechanic's own tasks
- **WHEN** an order has three of the mechanic's five tasks marked as done
- **THEN** the `Progreso` column shows `3/5`

#### Scenario: Master columns
- **WHEN** the master grid is rendered
- **THEN** it shows the columns `Nº Orden`, `Fecha`, `Vehículo`, `Cliente`, `Km` and `Progreso`

### Requirement: Detail Grid of Tasks
The system SHALL show, below the master grid, only the logged-in mechanic's tasks for the selected order, allowing inline editing, and SHALL show an empty-state message when no order is selected.

#### Scenario: Selecting an order loads its tasks
- **WHEN** the mechanic selects an order in the master grid
- **THEN** the detail grid shows only that mechanic's tasks for the selected order

#### Scenario: Switching selection rebuilds the detail
- **WHEN** the mechanic selects a different order
- **THEN** the detail list is rebuilt with the tasks of the newly selected order

#### Scenario: Inline editing
- **WHEN** the mechanic edits `Observaciones` or toggles `Revisado / OK`
- **THEN** the task is tracked as modified
- **AND** toggling `Revisado / OK` immediately updates the master `Progreso`

#### Scenario: No selection placeholder
- **WHEN** no order is selected
- **THEN** the detail card shows an empty-state message

### Requirement: Filtering & Search
The system SHALL provide a `Mostrar` selector (`Pendientes`, `Finalizados`, `Todos`) and a free-text search, both applied to the master grid of orders.

#### Scenario: Default filter
- **WHEN** the screen loads
- **THEN** `Mostrar` defaults to `Pendientes`
- **AND** only orders where the mechanic still has at least one uncompleted task are listed

#### Scenario: Finalizados and Todos
- **WHEN** the mechanic selects `Finalizados`
- **THEN** only orders with no pending own tasks are listed
- **WHEN** the mechanic selects `Todos`
- **THEN** all orders are listed

#### Scenario: Free-text search
- **WHEN** the mechanic types a search term
- **THEN** orders are filtered by order number, vehicle plate or client, case-insensitively
- **AND** when nothing matches, the master grid shows an empty-state message

### Requirement: Open Full Order
The system SHALL let the mechanic open the full order form for the selected order, both from a `Ver Orden Completa` action and by double-clicking a master row, keeping the `"MisTrabajos"` origin.

#### Scenario: Ver Orden Completa button
- **WHEN** the mechanic clicks `Ver Orden Completa` with an order selected
- **THEN** `CtlOrdenServicioForm` opens for that order with origin `"MisTrabajos"`

#### Scenario: Double-click a master row
- **WHEN** the mechanic double-clicks a master row
- **THEN** the same full order form opens for the double-clicked order
- **AND** double-clicking outside a row does nothing

### Requirement: Save Changes
The system SHALL persist every modified task across every order when `Guardar Cambios` is invoked, then clear the modified flags and reload preserving the selected order.

#### Scenario: Persist modified tasks
- **WHEN** the mechanic invokes `Guardar Cambios` after modifying tasks in one or more orders
- **THEN** every modified task across every order is persisted
- **AND** the order status is derived from its details (Completada / En Proceso / Abierta)
- **AND** the list reloads preserving the selected order when it still exists

#### Scenario: Nothing to save
- **WHEN** `Guardar Cambios` is invoked with no modified tasks
- **THEN** no write occurs

### Requirement: Clinical History PDF
The system SHALL enable the `Imprimir Historial Clínico` action only when an order is selected and the logged-in user holds `Reporte.Mecanico.Ver`, and SHALL generate the history for the selected order's vehicle.

#### Scenario: History enabled with permission and selection
- **WHEN** the mechanic holds `Reporte.Mecanico.Ver` and an order is selected
- **THEN** the `Imprimir Historial Clínico` action is enabled and generates the history for the selected order's vehicle

#### Scenario: History disabled without selection
- **WHEN** no order is selected
- **THEN** the `Imprimir Historial Clínico` action is not available
