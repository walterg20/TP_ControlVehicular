# Configuración de Empresa Specification — Delta

## ADDED Requirements

### Requirement: Company Configuration Management
The system SHALL persist the company branding configuration (name, address, phone, email and logo) as a single logical record ("singleton"). Reading this configuration SHALL NOT create duplicates, and saving SHALL update the existing record instead of inserting a new one.

#### Scenario: Singleton record is created on first read
- **WHEN** the company configuration is requested and no record exists
- **THEN** the system returns the seeded default configuration (one record only)

#### Scenario: Saving updates the existing record
- **WHEN** an Administrator saves new company data
- **THEN** the existing record is updated (no second row is inserted)

### Requirement: Company Configuration Screen (Admin Only)
The system SHALL provide a **Configuración de Empresa** screen, reachable only by a user holding the `Configuracion.Empresa.Gestionar` permission. The screen SHALL display the current company data and logo preview, allow selecting a new logo image from disk, and allow editing name, address, phone and email. Name, address and phone SHALL be required; saving with any of them empty SHALL be rejected with a validation message and SHALL NOT persist.

#### Scenario: Administrator can open the screen
- **WHEN** a user with the `Configuracion.Empresa.Gestionar` permission navigates to the menu
- **THEN** the **Configuración de Empresa** entry is visible and opens the screen

#### Scenario: Non-administrator cannot see the entry
- **WHEN** a user without the `Configuracion.Empresa.Gestionar` permission is logged in
- **THEN** the **Configuración de Empresa** menu entry is not visible

#### Scenario: Selecting a logo image
- **WHEN** the Administrator selects an image file from disk
- **THEN** the screen shows a preview of the selected image

#### Scenario: Required fields enforced
- **WHEN** the Administrator attempts to save with an empty name, address or phone
- **THEN** a validation message is shown and the configuration is not saved

### Requirement: Company Configuration Persistence
Saving the configuration SHALL persist the company data and the logo binary to the database in a single operation, and SHALL update the local branding cache. The persisted logo SHALL be the source of truth; the local file SHALL be treated as a cache that can be regenerated.

#### Scenario: Data and logo saved together
- **WHEN** the Administrator saves the configuration with a selected logo
- **THEN** both the company data and the logo binary are stored in the database

#### Scenario: Cache refreshed after save
- **WHEN** the configuration is saved successfully
- **THEN** the local branding cache (logo file and metadata) is refreshed from the saved values

### Requirement: Branding Cache Resolution
The system SHALL materialize the company branding into a per-user local cache (`%LocalAppData%\TP_ControlVehicular\Empresa\`) consisting of the logo file and a metadata file. The logo SHALL be resolved through a single point (`EmpresaBranding`) that prefers the local cache and falls back to the packaged asset when the cache is absent. Report generation SHALL read the branding from this cache synchronously and SHALL NOT change its existing contract.

#### Scenario: Branding loaded at login
- **WHEN** a user logs in
- **THEN** the company data and logo are loaded from the database into the local cache

#### Scenario: Logo fallback when cache is absent
- **WHEN** the local cached logo does not exist (e.g. fresh install, never configured)
- **THEN** report generation uses the packaged default logo asset

#### Scenario: No contract change for reports
- **WHEN** a PDF is generated
- **THEN** it resolves the logo through `EmpresaBranding` exactly as before, without the report code reading the database directly
