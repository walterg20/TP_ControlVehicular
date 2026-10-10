# Delta Specification: Implement User Login

## Added Requirements

### Requirement: Form Validation
- **SHALL** perform client-side input validation on `LoginViewModel` prior to calling authentication handlers.
- **SHALL** ensure `NombreUsuario` and `Contrasena` are non-empty strings.
- **SHALL** set `ErrorMessage` property when validation fails and display it on `CtlLogin`.

### Requirement: Database Authentication & Error Handling
- **SHALL** introduce `AutenticarUsuarioHandler` in `Negocio/Services` executing authentication logic via `IUsuarioRepository`.
- **SHALL** return an `AuthResult` enum with cases: `Exitoso`, `UsuarioNoEncontrado`, `ContrasenaIncorrecta`, `UsuarioInactivo`, `ErrorBaseDatos`.
- **SHALL** display specific error feedback:
  - If user is not found: `"El usuario ingresado no existe."`
  - If password is invalid: `"Contraseña incorrecta."`
  - If database fails: `"Error de conexión con la base de datos."`

### Requirement: MainWindow Dynamic UI Layout State
- **SHALL** manage `IsAuthenticated` state in `MainWindow` (or `MainWindowViewModel`).
- **SHALL** default `SidebarMenu.Visibility` to `Visibility.Collapsed` when `IsAuthenticated` is `false`.
- **SHALL** display `CtlLogin` in `grdContenido` (spanning across columns) on startup.
- **SHALL** toggle `SidebarMenu.Visibility` to `Visibility.Visible` and clear `CtlLogin` from `grdContenido` upon receiving the `OnLoginSuccess` event/command.
