# Manuales Specification — Delta

## ADDED Requirements

### Requirement: Manuals Artifact Structure
The repository SHALL contain a `docs/manuales/` folder holding a master index (`README.md`), a user/operative manual (`manual_usuario_roles.md`) and a technical administration manual (`manual_tecnico_administracion.md`), plus `assets/roles/` and `assets/tecnico/` folders for screenshots.

#### Scenario: Artifacts exist
- **WHEN** the documentation change is complete
- **THEN** the three Markdown files and both asset folders exist under `docs/manuales/`

#### Scenario: Screenshots are referenced by relative path
- **WHEN** a manual references a screenshot
- **THEN** it uses a path relative to the manual (`assets/roles/...` or `assets/tecnico/...`) so the image renders once the `.png` is saved

### Requirement: Mandatory Screenshot Markers
Every documented screen, modal, dialog or critical step SHALL be preceded by a standardized capture marker block (`> 📸 CAPTURA REQUERIDA:`) describing the screen/control name, the recommended state, the elements to highlight and a caption, immediately followed by the Markdown image link.

#### Scenario: Marker precedes every screen
- **WHEN** a screen, form, dialog or critical step is documented
- **THEN** a marker block and its image link appear immediately above the section content

#### Scenario: No separate screenshot inventory
- **WHEN** the manuals are reviewed
- **THEN** screenshots are listed inline as markers only, with no standalone inventory table

#### Scenario: Real control names
- **WHEN** a marker names the screen or modal
- **THEN** it uses the real XAML control class name (e.g. `CtlOrdenServicioForm`, `CtlBackupConfiguracion`)

### Requirement: User/Role Operative Manual
The user manual SHALL cover the operative flow by role (Administrador, Recepcionista, Mecánico) with a cover, a table of contents, a first-steps/navigation chapter and per-role chapters, and SHALL be grounded on the real RBAC of the application.

#### Scenario: Roles covered
- **WHEN** the user manual is read
- **THEN** Administrador, Recepcionista and Mecánico each have a dedicated chapter with step-by-step procedures

#### Scenario: Quick vehicle assignment documented
- **WHEN** the reception flow is documented
- **THEN** the `+ Rápido` flow (register/transfer a vehicle by license plate) is explained, including the automatic transfer of ownership

#### Scenario: Company data is attributed correctly
- **WHEN** company identity (razón social, CUIT, logo) is documented
- **THEN** it is attributed to `CtlConfiguracion` (Configuración de Empresa), while `CtlTaller` is documented as the workshop ABM

#### Scenario: Role menu visibility is accurate
- **WHEN** the menu/modules per role are documented
- **THEN** they match the real permission gating in `MainWindow.xaml.cs`

### Requirement: Technical Administration Manual
The technical manual SHALL cover environment requirements, database connectivity, schema deployment, the backup + Google Drive mechanism, disaster recovery, NTFS/RBAC security and troubleshooting.

#### Scenario: Connectivity documented
- **WHEN** the database chapter is read
- **THEN** it explains the `appsettings.json` connection string, TCP/IP + port 1433 and the Windows Firewall rule

#### Scenario: Scripts order documented
- **WHEN** schema deployment is read
- **THEN** it lists the idempotent `bd/` scripts in application order (`01` through `22`)

#### Scenario: Backup mechanism documented
- **WHEN** the backup chapter is read
- **THEN** it explains the dual server/UNC paths, the omission of `WITH COMPRESSION` on SQL Server Express, the test restore into `ControlVehicular_Test`, and the Google Drive resguardo with `credenciales.json` and DPAPI

#### Scenario: No production data touched by test restore
- **WHEN** the test-restore procedure is documented
- **THEN** it states the production database is never affected

### Requirement: Master README Index
The master `README.md` SHALL present the system, its architecture, a profiles-versus-modules matrix and direct links to both manuals.

#### Scenario: Navigation links work
- **WHEN** the README is opened
- **THEN** it links to `manual_usuario_roles.md` and `manual_tecnico_administracion.md`

#### Scenario: Modules documented
- **WHEN** the coverage audit runs
- **THEN** Login, Dashboard, Órdenes de Servicio, Mis Trabajos, Servicios, Clientes, Vehículos, Marcas, Modelos, Taller, Usuarios/Roles, Configuración de Empresa, Respaldos y Reportes are all referenced
