# marca Specification (delta)

## ADDED Requirements

### Requirement: Soft Delete for Marca
The system SHALL implement soft delete for `Marca`: deleting a brand SHALL set `Activo = false` instead of physically removing the record. Reactivating SHALL set `Activo = true`. The record SHALL never be physically deleted from the UI.

#### Scenario: Delete sets Activo false
- **WHEN** an Administrator deletes a selected brand
- **THEN** the brand's `Activo` becomes `false` and it disappears from the active list

#### Scenario: Reactivate sets Activo true
- **WHEN** an Administrator reactivates an inactive brand
- **THEN** the brand's `Activo` becomes `true`

### Requirement: Status Filter for Marca Listing
The system SHALL provide a status filter (Todos / Activos / Inactivos) in the brand listing. Administrators SHALL see the filter; Recepcionista and Mecánico roles SHALL NOT see it and SHALL only see active brands.

#### Scenario: Admin sees filter
- **WHEN** an Administrator opens the Marcas screen
- **THEN** the status filter ComboBox is visible

#### Scenario: Non-admin does not see filter
- **WHEN** a Recepcionista or Mecánico opens the Marcas screen
- **THEN** the status filter ComboBox is not visible and only active brands are listed

#### Scenario: Filter by status
- **WHEN** an Administrator selects "Inactivos" in the filter
- **THEN** only inactive brands are listed

### Requirement: Visual Distinction for Inactive Marcas
Inactive brands SHALL be displayed in red text with a light pink background in the grid.

#### Scenario: Inactive row styling
- **WHEN** a brand with `Activo = false` is displayed in the grid
- **THEN** its row has red foreground and light pink background