# backup-base-datos Specification (delta)

## ADDED Requirements

### Requirement: Production Database Restore with Double Factor
The system SHALL allow an Administrator holding the `Configuracion.Backup.Gestionar` permission to restore the production database (`ControlVehicular`) from a selected `.bak` backup file. The restore SHALL connect to `master`, put the production database in `SINGLE_USER WITH ROLLBACK IMMEDIATE`, execute `RESTORE DATABASE ... FROM DISK = ... WITH MOVE ..., REPLACE, RECOVERY`, and return the database to `MULTI_USER`. The restore SHALL require a second-factor authorization by a different, active user whose password matches and whose role holds the `Configuracion.Backup.Gestionar` permission. The original administrator SHALL NOT be allowed to authorize their own restore. The restore SHALL NEVER be performed without successful second-factor validation.

#### Scenario: Successful restore with valid second factor
- **WHEN** an Administrator selects a backup, triggers **Restaurar Backup**, confirms the operation, and provides a valid second user (different from the current session) with correct password and the `Configuracion.Backup.Gestionar` permission
- **THEN** the production database is restored from the selected `.bak` and a success message is shown suggesting an application restart

#### Scenario: Restore requires a selected backup
- **WHEN** an Administrator triggers **Restaurar Backup** with no backup selected
- **THEN** a warning is shown asking to select a backup and no restore is performed

#### Scenario: Second factor is mandatory
- **WHEN** an Administrator triggers **Restaurar Backup** and cancels the authorization dialog
- **THEN** no restore is performed

#### Scenario: Self-authorization is rejected
- **WHEN** the second factor user is the same as the current session user
- **THEN** the restore is rejected with a clear message and no restore is performed

#### Scenario: Invalid second-factor credentials are rejected
- **WHEN** the second factor user does not exist, is inactive, or the password is incorrect
- **THEN** the restore is rejected with a clear message and no restore is performed

#### Scenario: Unauthorized second factor is rejected
- **WHEN** the second factor user's role does not hold the `Configuracion.Backup.Gestionar` permission
- **THEN** the restore is rejected with a clear message and no restore is performed

#### Scenario: Restore failure is reported
- **WHEN** the `RESTORE DATABASE` statement fails
- **THEN** the failure is reported to the user and no success is shown

#### Scenario: Restore never modifies the test database
- **WHEN** the production restore runs
- **THEN** only the production database is affected; `ControlVehicular_Test` is not touched

### Requirement: Restore Action on Backup Screen
The system SHALL expose a **Restaurar Backup** action on the Respaldos screen, enabled only when a backup is selected and the user holds the `Configuracion.Backup.Gestionar` permission. The action SHALL be visually distinct (red) to convey its destructive nature.

#### Scenario: Button enabled with selection
- **WHEN** a backup is selected and the user has the permission
- **THEN** the **Restaurar Backup** button is enabled

#### Scenario: Button disabled without selection
- **WHEN** no backup is selected
- **THEN** the **Restaurar Backup** button is disabled