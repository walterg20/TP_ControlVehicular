# esquema-base-datos Specification

## Purpose
Define the canonical layout, ordering and idempotency guarantees of the `bd/` SQL script set that materializes the `ControlVehicular` database, plus the representative seed of Argentine vehicle makes and models.

## Requirements

### Requirement: Canonical Script Set

The `bd/` directory SHALL contain exactly four canonical idempotent SQL scripts applied in this order — `01_estructura.sql` (structure/DDL), `02_procedimientos.sql` (stored procedures), `03_triggers.sql` (triggers) and `04_datos.sql` (data population) — plus a non-productive development/test utility `05_utilidades_test.sql` and a `README.md`. Every canonical script SHALL be re-runnable without error and SHALL NOT create duplicate objects or rows.

#### Scenario: Fresh database is built from the canonical set
- **WHEN** the four canonical scripts are applied in order to an empty database
- **THEN** the schema, stored procedures, triggers and seed data are created successfully

#### Scenario: Re-applying the canonical set is idempotent
- **WHEN** the four canonical scripts are applied a second time to an already-populated database
- **THEN** every script completes without error and no object or row is duplicated

### Requirement: Consolidated Structure

`01_estructura.sql` SHALL define the full physical schema — the 18 tables `Cliente`, `Marca`, `Modelo`, `Vehiculo`, `PropietarioVehiculo`, `Roles`, `Usuarios`, `Permisos`, `RolPermisos`, `Taller`, `Servicio`, `RegistroServicio`, `DetalleServicio`, `Factura`, `Pago`, `MetodoPago`, `ConfiguracionEmpresa` and `ConfiguracionBackup` — together with their primary keys, foreign keys, constraints and indexes.

#### Scenario: All application tables exist after structure is applied
- **WHEN** `01_estructura.sql` has been applied
- **THEN** the 18 canonical tables exist in the database

#### Scenario: Obsolete table is not created
- **WHEN** the structure script has been applied
- **THEN** the obsolete plural table `Facturas` does not exist

### Requirement: Consolidated Stored Procedures

`02_procedimientos.sql` SHALL contain every stored procedure consumed by the application, and for each procedure it SHALL contain only its latest effective definition (no superseded earlier versions).

#### Scenario: Latest definition wins
- **WHEN** a stored procedure was redefined by a later fix script
- **THEN** the consolidated script contains the latest effective body and not the superseded one

#### Scenario: Reporting procedures are present
- **WHEN** the procedure script has been applied
- **THEN** the reporting procedures used by the application exist

### Requirement: Consolidated Triggers

`03_triggers.sql` SHALL define the three domain triggers `TR_DetalleServicio_ValidarPrecioYCantidad`, `TR_RegistroServicio_SincronizarKm` and `TR_DetalleServicio_BloquearModificacionFinalizada`, in an idempotent form.

#### Scenario: Domain triggers exist after applying
- **WHEN** `03_triggers.sql` has been applied
- **THEN** all three domain triggers exist and are enabled

### Requirement: Seed Data and Argentine Market Catalog

`04_datos.sql` SHALL seed, idempotently, the base data required to operate the system: roles (Administrator, Receptionist, Mechanic), permissions and role-permission links, the workshop, the company configuration, the backup configuration and the initial Administrator user. It SHALL also seed an expanded catalog of vehicle brands and models representative of the Argentine market. The seed SHALL preserve `Toyota = Id 1` and `Renault = Id 2` for existing identifiers.

#### Scenario: Base data is available after seeding
- **WHEN** `04_datos.sql` has been applied to an empty database
- **THEN** roles, permissions, role-permission links, configuration rows and the initial Administrator user exist

#### Scenario: Brand catalog reflects the Argentine market
- **WHEN** the data script has been applied
- **THEN** the `Marca`/`Modelo` catalog contains brands and representative models that circulate in Argentina

#### Scenario: Existing brand identifiers are preserved
- **WHEN** the data script is applied
- **THEN** `Toyota` is `Id 1` and `Renault` is `Id 2`

#### Scenario: Seeding does not duplicate rows on re-run
- **WHEN** `04_datos.sql` is applied a second time
- **THEN** no seed row is duplicated

### Requirement: Development/Test Utility Script

`05_utilidades_test.sql` SHALL be a non-productive utility that removes end-to-end test artifacts (vehicles with `TDD%` plates and `EndToEnd` clients, with their orders, invoices and payments) in a foreign-key-safe and idempotent way, leaving real and demo data untouched.

#### Scenario: Test artifacts are purged
- **WHEN** `05_utilidades_test.sql` is applied to a database that contains end-to-end test artifacts
- **THEN** the test vehicles, clients and their orders, invoices and payments are removed and real data is preserved

#### Scenario: Utility is not part of the productive bootstrap
- **WHEN** a fresh production database is built
- **THEN** applying `01`–`04` is sufficient and `05` is not required

### Requirement: Historical Script Preservation

The original SQL scripts SHALL be preserved under `docs/historico/bd/` rather than deleted, so the consolidation is auditable.

#### Scenario: Originals remain available
- **WHEN** the reorganization is complete
- **THEN** the original scripts are available under `docs/historico/bd/`
