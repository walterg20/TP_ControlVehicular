# Spec Delta

## MODIFIED Requirements

### Requirement: Master Grid of Orders
The system SHALL display the mechanic's orders in a master grid where each order appears exactly once, showing only the logged-in user's tasks, sorted by order number descending and excluding cancelled orders. The master grid SHALL be populated through database-level pagination, retrieving only the records of the requested page together with the total count of matching records, and SHALL render the shared pagination toolbar (`CtlPaginacion`) below the grid.

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

#### Scenario: Master grid paginated in the database
- **WHEN** the mechanic navigates to another page of the master grid
- **THEN** the system retrieves only the orders of that page from the database
- **AND** the toolbar reports the current page, the total page count and the total record count.

### Requirement: Filtering & Search
The system SHALL provide a `Mostrar` selector (`Pendientes`, `Finalizados`, `Todos`) and a free-text search, both applied to the master grid of orders. Both filters SHALL be applied by the database as part of the paginated query, and the search SHALL be triggered by an explicit `Buscar` action, by the `Enter` key, or automatically 500 ms after the user stops typing. Any filter change SHALL reset the current page to 1.

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

#### Scenario: Filter change resets the page
- **WHEN** the mechanic changes the `Mostrar` selector or the search term
- **THEN** the master grid is reloaded from page 1 with the new filter.
