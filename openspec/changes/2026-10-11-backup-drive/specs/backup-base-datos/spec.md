# Backup de Base de Datos Specification — Delta

## ADDED Requirements

### Requirement: Backup Configuration Management
The system SHALL persist the database backup configuration as a single logical record ("singleton", `Id = 1`) holding the database name, the server-side backup path, the shared (UNC) backup path, the destination Google Drive folder and the last uploaded file. Reading this configuration SHALL NOT create duplicates, and saving SHALL update the existing record instead of inserting a new one.

#### Scenario: Singleton record is created on first read
- **WHEN** the backup configuration is requested and no record exists
- **THEN** the system creates and returns a single default record (`Id = 1`)

#### Scenario: Saving updates the existing record
- **WHEN** an Administrator saves new backup configuration values
- **THEN** the existing record is updated (no second row is inserted)

#### Scenario: Two paths distinguish writer from reader
- **WHEN** the Administrator configures the backup location
- **THEN** the server path (where the SQL service writes the `.bak`) and the UNC path (which the application reads) are stored separately, allowing the app to run on a PC other than the SQL Server host

### Requirement: Full Database Backup
The system SHALL create a full backup of the configured database by executing `BACKUP DATABASE` on the SQL Server instance, writing a `ControlVehicular_YYYYMMDD_HHMMSS_FULL.bak` file to the configured server-side path, followed by a `RESTORE VERIFYONLY` integrity check. Creating a backup SHALL NOT modify the production data.

#### Scenario: Administrator creates a backup
- **WHEN** an Administrator triggers **Hacer backup**
- **THEN** a `.bak` file is written to the configured server path and the backup is verified with `RESTORE VERIFYONLY`

#### Scenario: Backup settings missing
- **WHEN** the Administrator triggers **Hacer backup** and the server backup path is empty
- **THEN** the operation is rejected with a clear validation message and no file is written

#### Scenario: Backup failure is reported
- **WHEN** the `BACKUP DATABASE` statement fails
- **THEN** the failure is reported to the user and no success is shown

### Requirement: Backup Listing
The system SHALL list the available backup files by scanning the effective backup folder (the UNC path, falling back to the server path) for `*.bak` files, exposing each file's name, full path, size and last modification date, ordered by date descending.

#### Scenario: List shows existing backups
- **WHEN** a user with the backup permission opens the **Respaldos** screen
- **THEN** every `.bak` file in the effective folder is listed with its name, size and date

#### Scenario: Empty folder
- **WHEN** the effective folder contains no `.bak` files
- **THEN** the list is shown empty without errors

### Requirement: Backup Verification by Test Restore
The system SHALL verify a selected backup by restoring it into an isolated test database (`ControlVehicular_Test`). The verification SHALL read the backup's logical file names with `RESTORE FILELISTONLY`, drop any existing test database, restore the backup using `MOVE` for both data and log files with `REPLACE` and `RECOVERY`, and confirm the test database is online. The verification SHALL NEVER modify or drop the production database.

#### Scenario: Valid backup passed verification
- **WHEN** an Administrator triggers **Probar backup** on a valid `.bak`
- **THEN** `ControlVehicular_Test` is recreated from the backup, ends online, and a success result is reported

#### Scenario: Production database is never touched
- **WHEN** the test restore runs
- **THEN** the production database remains untouched (only `ControlVehicular_Test` is dropped and recreated)

#### Scenario: Corrupt backup is detected
- **WHEN** the selected `.bak` cannot be restored
- **THEN** the failure is reported and no success is shown

### Requirement: Cloud Resguardo to Google Drive
The system SHALL upload a backup to Google Drive using the Google Drive API with the `drive.file` (least-privilege) scope. The OAuth client credentials SHALL be read from a `credenciales.json` (Desktop app) file located next to the executable and SHALL NOT be committed to the repository; when the file is missing the operation SHALL fail with a clear message indicating where to place it. The system SHALL resolve the destination folder from the configuration, and when no folder id is configured it SHALL find or create a `ControlVehicularBackups` folder and persist its id. It SHALL upload the most recent `.bak` as `ControlVehicular_YYYYMMDD_FULL.bak`, producing one file per day and overwriting the file when a backup for the same day already exists. The upload SHALL be skipped when the most recent file was already uploaded. The OAuth refresh token SHALL be stored per PC, encrypted with DPAPI, and SHALL NOT be persisted in the database or in plain text.

#### Scenario: First-time authorization
- **WHEN** an Administrator connects to Google Drive for the first time on a PC
- **THEN** the interactive OAuth consent runs and an encrypted refresh token is stored locally for that PC

#### Scenario: Missing client credentials file
- **WHEN** an Administrator connects to Google Drive and `credenciales.json` is not next to the executable
- **THEN** a clear error indicates that the credentials file was not found and must be placed next to the executable

#### Scenario: Destination folder is created automatically
- **WHEN** an Administrator triggers **Resguardar en Drive** and no destination folder id is configured
- **THEN** the system finds or creates the `ControlVehicularBackups` folder in the user's Drive, stores its id, and uploads the backup into it

#### Scenario: Upload the latest backup
- **WHEN** an Administrator triggers **Resguardar en Drive** and the latest `.bak` has not been uploaded
- **THEN** the file is uploaded to the resolved Drive folder as `ControlVehicular_YYYYMMDD_FULL.bak`

#### Scenario: Same-day re-upload overwrites
- **WHEN** a backup is uploaded on a day that already has a Drive file for that date
- **THEN** the existing Drive file is overwritten (a single file per day is kept)

#### Scenario: Already-uploaded file is skipped
- **WHEN** the most recent `.bak` is the one recorded as already uploaded for the current day
- **THEN** the upload is skipped and a message indicates the backup is already resguardado

#### Scenario: Authorization required before upload
- **WHEN** **Resguardar en Drive** is triggered and no local credential exists
- **THEN** the user is prompted to connect to Google Drive first

### Requirement: Backup Administration Screen (Admin Only)
The system SHALL provide a **Respaldos** screen reachable only by a user holding the `Configuracion.Backup.Gestionar` permission. The screen SHALL list the available backups and offer the actions **Hacer backup**, **Probar backup** (on the selected file) and **Resguardar en Drive**, along with a refresh action.

#### Scenario: Administrator can open the screen
- **WHEN** a user with the `Configuracion.Backup.Gestionar` permission navigates to the menu
- **THEN** the **Respaldos** entry is visible and opens the screen

#### Scenario: Non-administrator cannot see the entry
- **WHEN** a user without the `Configuracion.Backup.Gestionar` permission is logged in
- **THEN** the **Respaldos** menu entry is not visible

#### Scenario: Test requires a selected backup
- **WHEN** the Administrator triggers **Probar backup** with no backup selected
- **THEN** a warning is shown asking to select a backup

### Requirement: Backup Configuration Screen (Admin Only)
The system SHALL provide a **Configuración de Respaldo** screen reachable only by a user holding the `Configuracion.Backup.Gestionar` permission, allowing the Administrator to edit the database name, the server-side backup path, the UNC backup path and the destination Drive folder, to save them, and to connect the PC to Google Drive.

#### Scenario: Administrator edits and saves settings
- **WHEN** the Administrator saves the backup configuration
- **THEN** the values are persisted and used by subsequent backup, listing and upload operations

#### Scenario: Connecting the PC to Google Drive
- **WHEN** the Administrator clicks **Conectar con Google Drive**
- **THEN** the interactive OAuth consent runs and the PC is left authorized for future uploads

#### Scenario: Database name is required
- **WHEN** the Administrator attempts to save with an empty database name
- **THEN** a validation message is shown and the configuration is not saved
