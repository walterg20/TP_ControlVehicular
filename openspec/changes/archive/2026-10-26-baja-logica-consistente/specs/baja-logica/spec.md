# Baja Lógica — Delta Specification

## ADDED Requirements

### Requirement: Active Flag Defaults To True On Creation

For entities that support logical deletion (`Cliente`, `Taller`, `Servicio`), a newly created record SHALL have `Activo = true` by default, both in the domain entity and in the database schema.

#### Scenario: New client is active by default

- **WHEN** a `Cliente` is instantiated without setting `Activo`
- **THEN** `Activo` is `true`

#### Scenario: New workshop is active by default

- **WHEN** a `Taller` is instantiated without setting `Activo`
- **THEN** `Activo` is `true`

#### Scenario: Database column has a default

- **WHEN** a row is inserted into `Cliente`, `Taller` or `Servicio` without providing `Activo`
- **THEN** the column defaults to `1` (`DEFAULT (1)`)

### Requirement: List Active Records By Default

Listing operations for `Cliente`, `Taller` and `Servicio` SHALL return only active records by default.

#### Scenario: Services list excludes inactive ones

- **WHEN** `ListarServiciosHandler.HandleAsync()` is called without arguments
- **THEN** only services with `Activo = true` are returned

#### Scenario: Workshops list excludes inactive ones

- **WHEN** `ListarTallerHandler.HandleAsync()` is called without arguments
- **THEN** only workshops with `Activo = true` are returned

#### Scenario: Clients list excludes inactive ones

- **WHEN** `ClienteViewModel.LoadAsync()` runs with `MostrarInactivos = false`
- **THEN** only clients with `Activo = true` are loaded

### Requirement: Explicitly List Inactive Records

The system SHALL provide a way to list all records (including inactive ones) on explicit request.

#### Scenario: Repository exposes GetTodosAsync

- **WHEN** `IRepository<T>.GetTodosAsync()` is invoked
- **THEN** all records of `T` are returned regardless of `Activo`

#### Scenario: Handler accepts incluirInactivos

- **WHEN** `ListarServiciosHandler.HandleAsync(true)` (or `ListarTallerHandler.HandleAsync(true)`) is called
- **THEN** active and inactive records are returned

#### Scenario: ViewModel toggles MostrarInactivos

- **WHEN** `MostrarInactivos` is set to `true` in `ClienteViewModel`/`TallerViewModel`/`ServicioViewModel`
- **THEN** the next `LoadAsync()` loads active and inactive records

### Requirement: Reactivate A Logically Deleted Record

The system SHALL allow reactivating a record previously marked inactive, setting `Activo = true` and making it appear again in the default listing.

#### Scenario: Reactivate service

- **WHEN** `ReactivarServicioHandler.HandleAsync(id)` is called for an inactive service
- **THEN** the service's `Activo` becomes `true` and it appears in the default service listing

#### Scenario: Reactivate workshop

- **WHEN** `ReactivarTallerHandler.HandleAsync(id)` is called for an inactive workshop
- **THEN** the workshop's `Activo` becomes `true` and it appears in the default workshop listing

#### Scenario: Reactivate client

- **WHEN** `ClienteViewModel.ReactivarClienteAsync(id)` is called for an inactive client
- **THEN** the client's `Activo` becomes `true` and it appears in the default client listing

### Requirement: UI Affordances For Inactive Records

Each of `CtlCliente`, `CtlTaller` and `CtlServicio` SHALL provide a "Mostrar inactivos" control in the toolbar (row 1) and a "Reactivar" action in the footer (row 3).

#### Scenario: Toggle reloads the list

- **WHEN** the user toggles "Mostrar inactivos"
- **THEN** the grid reloads including (or excluding) inactive records

#### Scenario: Reactivate button available

- **WHEN** an inactive record is selected
- **THEN** the "Reactivar" action sets it active and it reappears in the default listing
