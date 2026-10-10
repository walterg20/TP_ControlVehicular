# Authentication Capability Specification

## Purpose
Defines the functional and technical specifications for authenticating users within the TP_ControlVehicular application, including form validation, error handling, database credential verification, and main window layout state transitions.

## Requirements

### Requirement 1: Form Validation
- **SHALL** validate user inputs on the client side before triggering database authentication queries.
- **SHALL** verify that the username is neither empty nor whitespace.
- **SHALL** verify that the password is neither empty nor whitespace.
- **SHALL** display explicit error messages directly in the login view (`CtlLogin`) when validation fails, without executing database queries.

### Requirement 2: Database User and Password Error Handling
- **SHALL** query the `IUsuarioRepository` to locate the user record by username (`GetByNombreAsync`).
- **SHALL** handle the case where the user does not exist in the database, returning a distinct `UserNotFound` error state.
- **SHALL** handle the case where the user exists but the provided password does not match, returning a distinct `InvalidPassword` error state.
- **SHALL** handle the case where the user exists and is inactive (`Estado == false`), returning an `UserInactive` error state.
- **SHALL** catch unexpected database exceptions (e.g., connection timeouts, SQL Server unavailability) and return a friendly `DatabaseError` message without crashing the application.

### Requirement 3: Startup View State and Navigation Layout
- **SHALL** start the application inside `MainWindow` without displaying separate floating login windows.
- **SHALL** hide the primary navigation sidebar (`Visibility.Collapsed` or hidden grid column) on initial application startup.
- **SHALL** inject the `CtlLogin` control into the central work area (`grdContenido`) upon startup.
- **SHALL** upon successful authentication:
  - Reveal the navigation sidebar (`Visibility.Visible`).
  - Remove `CtlLogin` from `grdContenido`.
  - Store the authenticated `UsuarioDto` in the session state.
  - Render default initial dashboard or empty workplace controls.

---

## Scenarios (BDD Specifications)

### Scenario 1: Empty Username or Password
- **Given** the user is on the `CtlLogin` view inside `MainWindow`
- **When** the user leaves the Username or Password field empty and clicks "Iniciar Sesión"
- **Then** the application SHALL NOT perform a database query
- **And** SHALL display a validation error message: `"Por favor, ingrese usuario y contraseña."`

### Scenario 2: Non-existent User in Database
- **Given** the user enters a username `"usuario_inexistente"` and a password `"123456"`
- **When** the user submits the login form
- **Then** the authentication handler queries the database via `IUsuarioRepository.GetByNombreAsync("usuario_inexistente")`
- **And** finds no matching record
- **Then** the system SHALL display the error message: `"El usuario ingresado no existe."`

### Scenario 3: Incorrect Password
- **Given** a valid active user `"admin"` exists in the database
- **When** the user enters username `"admin"` and password `"clave_erronea"`
- **And** submits the login form
- **Then** the authentication handler verifies the password against the stored credential
- **And** detects a mismatch
- **Then** the system SHALL display the error message: `"Contraseña incorrecta."`

### Scenario 4: Database Connection Failure
- **Given** SQL Server is unreachable or offline
- **When** the user attempts to log in
- **Then** the application SHALL catch the `DbUpdateException` / `SqlException`
- **And** display a fallback error message: `"Error de conexión a la base de datos. Verifique la conexión."`

### Scenario 5: Successful Login & Window Layout Transition
- **Given** a valid active user enters correct credentials
- **When** the user submits the login form
- **Then** the authentication handler returns a success result containing the `UsuarioDto`
- **And** `MainWindow` collapses or removes the `CtlLogin` view
- **And** `MainWindow` sets the navigation sidebar border to `Visibility.Visible`
- **And** `grdContenido` is cleared and ready for regular application controls (`CtlCliente`, `CtlVehiculo`, etc.).
