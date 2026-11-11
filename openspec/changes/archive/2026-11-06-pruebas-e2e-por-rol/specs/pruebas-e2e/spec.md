# End-to-End Tests by Role Specification — Delta

## ADDED Requirements

### Requirement: Isolated Test Database Provisioning
The test suite SHALL run against a dedicated SQL Server database and SHALL NEVER read from or write to the production database (`ControlVehicular`). The connection string SHALL be resolved from the environment variable `TPCV_TEST_CONNECTION` when present; otherwise it SHALL be derived from `ConnectionStrings:DefaultConnection` in `appsettings.json` with the catalog replaced by `ControlVehicular_Test`. Provisioning SHALL be idempotent and SHALL run at most once per test run: the target database is created if it does not exist, and the canonical scripts `bd/01_estructura.sql`, `bd/02_procedimientos.sql`, `bd/03_triggers.sql` and `bd/04_datos.sql` are applied in that order, each `USE [ControlVehicular]` statement rewritten to the test catalog, and each script split on `GO` batch separators. Provisioning SHALL use the scripts (which carry the stored procedures, triggers and catalog seed data the reports depend on) rather than EF Core migrations alone. The provisioning caveat SHALL be documented: the test catalog is also the restore target of `IDatabaseBackupService.ProbarBackupAsync`, so real restore calls MUST NOT be exercised against it.

#### Scenario: Test database is created and provisioned
- **WHEN** the test run starts and the test database does not exist
- **THEN** the database is created and all four canonical scripts are applied successfully

#### Scenario: Provisioning is idempotent
- **WHEN** the test run starts and the test database already exists with the schema and catalog data
- **THEN** provisioning completes without error and without duplicating catalog rows

#### Scenario: Production database is never touched
- **WHEN** any end-to-end test executes
- **THEN** all reads and writes target the test catalog only, and `ControlVehicular` is left unchanged

### Requirement: Automatic Skip Without SQL Server
The test suite SHALL detect whether the configured SQL Server instance is reachable. When it is not reachable, every test that requires the database SHALL be reported as skipped with an explanatory reason, and the run SHALL succeed (no failure). Detection and skipping SHALL be implemented with project-owned code (a `FactAttribute` subclass) and SHALL NOT add new NuGet dependencies.

#### Scenario: No server available
- **WHEN** the SQL Server instance cannot be reached
- **THEN** every database-dependent test is skipped with a reason and the test run succeeds

#### Scenario: Server available
- **WHEN** the SQL Server instance is reachable
- **THEN** the database-dependent tests execute instead of being skipped

### Requirement: Run-Scoped Test Isolation
Each test run SHALL seed its own data under a unique marker of the form `TDD-{yyyyMMddHHmmss}` and SHALL remove only the rows it created (identified by that marker) during teardown, in foreign-key-safe order. Seeding and cleanup SHALL NOT delete rows that belong to other runs or to the development data.

#### Scenario: Run seed is tagged
- **WHEN** a test run seeds a client, vehicle, work order, task, user or classification
- **THEN** each seeded row is identifiable by the run marker

#### Scenario: Teardown removes only this run's data
- **WHEN** the test run finishes
- **THEN** the rows carrying this run's marker are deleted and unrelated rows remain

### Requirement: Recepcionista Work-Order Lifecycle
A Recepcionista SHALL be able to drive a work order from registration through payment against the real database. The suite SHALL cover registering a work order (which assigns a correlative execution order, sets the order to `"En Proceso"`, sets the first task to `"En Curso"` and the remaining tasks to `"Pendiente"`), paying it once it is `"Completada"`, and producing both the reception and the payment PDF receipts.

#### Scenario: Registration assigns sequence and initial states
- **WHEN** a Recepcionista registers a work order with several services
- **THEN** the order is `"En Proceso"`, each task has a correlative `OrdenEjecucion`, the first task is `"En Curso"` and the following tasks are `"Pendiente"`

#### Scenario: Full happy path from registration to receipt
- **WHEN** a Recepcionista registers a work order with a service and a mechanic, the mechanic finalizes every task, and the Recepcionista registers a payment for the full balance
- **THEN** the order is `"Completada"` before payment, becomes `"Pagada"` after payment, a single invoice exists with a total equal to the sum of `Precio * Cantidad`, and both the reception and payment PDF receipts are generated as non-empty byte arrays

#### Scenario: Invoice generation is idempotent
- **WHEN** the invoice is generated twice for the same work order
- **THEN** the second call returns the existing invoice and no second invoice row is created

#### Scenario: Invoice total is zero for an order with no tasks
- **WHEN** the invoice is generated for a work order that has no tasks
- **THEN** the invoice total is zero

#### Scenario: Reception receipt for an unknown order
- **WHEN** the reception receipt is requested for a non-existent work-order id
- **THEN** the handler returns no document (null) instead of throwing

#### Scenario: Payment receipt before payment
- **WHEN** the payment receipt is requested for a work order that has no registered payment
- **THEN** the handler returns no document (null) instead of throwing

### Requirement: Billing Rejections
The billing service SHALL reject invalid payment attempts with the exact domain messages, and the suite SHALL cover each rejection. A payment SHALL only be accepted for an order that is `"Completada"`, for a valid payment method, and never for an already-paid invoice or a card payment whose amount is not exactly the outstanding balance.

#### Scenario: Payment for an unknown invoice
- **WHEN** a payment is registered for a non-existent invoice id
- **THEN** the operation fails with `"Factura no encontrada."`

#### Scenario: Payment for an order that is not completed
- **WHEN** a payment is registered for an order that is not `"Completada"`
- **THEN** the operation fails with `"La orden de servicio debe estar completada para procesar el pago."`

#### Scenario: Payment for an already-paid invoice
- **WHEN** a payment is registered for an invoice whose balance is already zero
- **THEN** the operation fails with `"La factura ya está pagada."`

#### Scenario: Payment with an invalid method
- **WHEN** a payment is registered with a payment-method id that does not exist
- **THEN** the operation fails with `"Método de pago no válido."`

#### Scenario: Card payment with a wrong amount
- **WHEN** a card payment is registered for an amount different from the outstanding balance
- **THEN** the operation fails with `"El pago con tarjeta debe ser por el monto exacto del saldo."`

### Requirement: Mecánico Task Progression
A Mecánico SHALL only see and act on their own non-finalized tasks, and SHALL advance tasks through the real database. Advancing a task marks it `"Finalizada"` and enables the next pending task of the same order (moving it to `"En Curso"`); when no pending tasks remain, the order becomes `"Completada"`. The Mecánico SHALL be able to produce the daily work-sheet report for a date.

#### Scenario: Own pending tasks are listed
- **WHEN** a Mecánico requests their task list
- **THEN** only tasks assigned to that mechanic that are not finalized are returned

#### Scenario: Advancing a task finalizes it and enables the next
- **WHEN** a Mecánico advances a task that has a later pending task in the same order
- **THEN** the advanced task becomes `"Finalizada"` and the next pending task becomes `"En Curso"`

#### Scenario: Finalizing the last task completes the order
- **WHEN** a Mecánico advances the last pending task of an order
- **THEN** the task state becomes `"Finalizada"` and the order state becomes `"Completada"`

#### Scenario: Advancing an already-finalized task is a no-op
- **WHEN** a Mecánico advances a task that is already `"Finalizada"`
- **THEN** the operation completes without changing any state

#### Scenario: Finalized tasks leave the pending list
- **WHEN** a task has been finalized and the Mecánico requests their task list again
- **THEN** the finalized task is no longer returned

#### Scenario: Daily work-sheet report
- **WHEN** a Mecánico requests the daily work-sheet report for a date that has assigned tasks
- **THEN** the report returns the corresponding rows and a non-empty PDF is produced

#### Scenario: Advancing another mechanic's task
- **WHEN** a Mecánico attempts to advance a task assigned to a different mechanic
- **THEN** the operation is rejected with an authorization error and the task state is unchanged

#### Scenario: Advancing a non-existent task
- **WHEN** a Mecánico attempts to advance a task id that does not exist
- **THEN** the operation is rejected with `"Tarea no encontrada."`

### Requirement: Administrador Configuration
An Administrador SHALL manage workshop and company configuration against the real database. The suite SHALL cover registering, modifying and listing workshops, and saving and re-reading the company configuration (including a logo), verifying that changes are persisted.

#### Scenario: Register, modify and list workshops
- **WHEN** an Administrador registers a workshop, modifies it and lists the active workshops
- **THEN** the created workshop appears with the modified values in the listing

#### Scenario: Company configuration round-trip
- **WHEN** an Administrador saves the company configuration with a logo and then reads it back
- **THEN** the persisted values, including the logo bytes, match what was saved

### Requirement: Administrador Backup Orchestration
The suite SHALL verify the backup screen's orchestration with controlled doubles (`IDatabaseBackupService`, `IDriveBackupService`, `IBackupConfiguracionService`) because real `BACKUP`/`RESTORE` and real Google Drive uploads are destructive and environment-dependent. The backup configuration SHALL be persisted against the real database.

#### Scenario: Backup configuration round-trip
- **WHEN** an Administrador saves the backup configuration and reads it back
- **THEN** the persisted database name and paths match what was saved

#### Scenario: Manual backup delegates to the database service
- **WHEN** the backup command runs and the database service reports success
- **THEN** the resulting message reflects success and no error is shown

#### Scenario: Manual backup failure is surfaced
- **WHEN** the backup command runs and the database service throws or reports failure
- **THEN** the failure is surfaced to the caller and no success is shown

#### Scenario: Resguardar without credentials
- **WHEN** the Drive service reports that no credentials are stored
- **THEN** the resguardar command is not allowed to proceed silently and the caller is informed that authentication is required

### Requirement: Report Data by Role
The suite SHALL verify that the report data sources return real rows for seeded data, and that role-based filtering is applied. The Administrador reports (income, top clients, most-repaired models, services by range, mechanic productivity) are read through `IReporteGerencialRepository`; the work-orders report is read through `IRegistroServicioRepository` and SHALL filter by mechanic when the current user is a Mecánico; the mechanic report is read through `IReporteRepository`.

#### Scenario: Managerial reports return seeded rows
- **WHEN** an Administrador requests the managerial reports for a date range that contains a seeded paid order
- **THEN** the income, top-clients, models and services report data sources return consistent, non-error results

#### Scenario: Work-orders report filters for a mechanic
- **WHEN** the current user is a Mecánico and the work-orders report is requested
- **THEN** only orders that involve that mechanic are returned

#### Scenario: Report for a range with no data
- **WHEN** a report is requested for a date range with no seeded data
- **THEN** the report returns an empty collection and does not throw

### Requirement: Role-Based Access Control Verification
The suite SHALL verify at runtime which capabilities each role actually has, using the permission data seeded in the database and the application's role gates. An Administrador SHALL have the administrative capabilities and SHALL NOT have `MisTrabajos.Ver`; a Recepcionista SHALL have work-order management and order/operational report visibility but SHALL NOT have administrative capabilities; a Mecánico SHALL have `MisTrabajos.Ver` and the mechanic report but SHALL NOT have administrative or billing-configuration capabilities.

#### Scenario: Administrator capabilities
- **WHEN** the effective permissions for the Administrador role are evaluated
- **THEN** administrative capabilities are granted and `MisTrabajos.Ver` is denied

#### Scenario: Recepcionista capabilities
- **WHEN** the effective permissions for the Recepcionista role are evaluated
- **THEN** client, vehicle, brand, model and work-order management plus order/operational report visibility are granted, and administrative capabilities are denied

#### Scenario: Mecánico capabilities
- **WHEN** the effective permissions for the Mecánico role are evaluated
- **THEN** `MisTrabajos.Ver` and the mechanic report are granted, and administrative and billing-configuration capabilities are denied

#### Scenario: Unknown user has no capabilities
- **WHEN** permissions are evaluated with no authenticated user
- **THEN** no capabilities are granted

### Requirement: Unit-Level Handler Behavior
The suite SHALL include unit tests for handlers and services that depend only on repository interfaces, using doubled repositories and the real AutoMapper profile, covering both success and failure. These tests SHALL run without a database.

#### Scenario: Handler success maps entity to DTO
- **WHEN** a handler is invoked with a valid input and its repository double succeeds
- **THEN** a DTO with the mapped values is returned

#### Scenario: Authentication success returns the session user
- **WHEN** authentication is attempted with an active user's correct document and password
- **THEN** the result is successful and the session user is returned

#### Scenario: Authentication failure for an unknown document
- **WHEN** authentication is attempted with a document that does not exist
- **THEN** the result reports the user was not found and no session user is returned

#### Scenario: Authentication failure for an inactive user
- **WHEN** authentication is attempted for a user that is not active
- **THEN** the result reports the user is inactive and no session user is returned

#### Scenario: Authentication failure for a wrong password
- **WHEN** authentication is attempted with the correct document but a wrong password
- **THEN** the result reports an incorrect password and no session user is returned

#### Scenario: Company configuration defaults when absent
- **WHEN** the company configuration is read through the service and the repository returns no record
- **THEN** a default configuration is returned instead of an error
