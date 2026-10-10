# Proposal: Implement User Login and Single-Window Navigation State

## Why
Currently, the application starts directly showing `MainWindow` with the full administration sidebar menu visible, without requiring user authentication. To secure application features and establish user session identity, a Login flow must be integrated on initial application startup.

## What
1. **Single-Window Login Integration**: Implement `CtlLogin` user control rendered inside `MainWindow` on startup while hiding the navigation sidebar.
2. **Form Validation**: Validate empty/whitespace fields on client-side before attempting database operations.
3. **Database Error & Credential Handling**: Differentiate between "User does not exist in DB", "Incorrect password", "Inactive user", and "Database connection error".
4. **Layout Transition**: Upon successful login, clear the login control from `MainWindow`, reveal the navigation sidebar, and allow normal navigation to other views (`CtlCliente`, `CtlVehiculo`, etc.).

## Out of Scope
- Password hashing upgrades (will compare against stored user passwords or hash if implemented in repository).
- Multi-factor authentication (MFA).
- Remember-me persistent token storage across app restarts.
