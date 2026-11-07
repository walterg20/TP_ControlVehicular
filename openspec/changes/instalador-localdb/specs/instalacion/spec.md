# instalacion Specification (delta)

## ADDED Requirements

### Requirement: First-Run Setup
The application SHALL detect an empty database (no users) on startup and show a setup modal to create the initial administrator and company data before allowing use.

#### Scenario: Empty database triggers setup
- **WHEN** the application starts and no users exist
- **THEN** the setup modal is shown and the main window is blocked until completion

### Requirement: SuperAdmin Role
The system SHALL include a `SuperAdmin` role with exclusive permission `Configuracion.Backup.Gestionar` for validating database restorations.

#### Scenario: Only SuperAdmin can restore
- **WHEN** a user without `Configuracion.Backup.Gestionar` attempts restoration
- **THEN** restoration is denied

### Requirement: Base Seed Only
The installer SHALL seed only base data (roles, permissions, matrix, workshop, configs, admin, 15 services, 45 brands, 203 models). Demo population (`06_poblacion.sql`) SHALL NOT be included.

#### Scenario: Fresh install has base data only
- **WHEN** installation completes
- **THEN** the database contains base seed data and no demo orders/clients