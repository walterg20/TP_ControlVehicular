# Delta Specification: Role-Based Menu Restrictions

## Added Requirements

### Requirement: Dynamic Menu Control by Role
- **SHALL** inspect `UsuarioDto.RolNombre` inside `MainWindow.xaml.cs` during `OnLoginExitoso`.
- **SHALL** hide prohibited navigation buttons based on role:
  - If role is not `Administrador`, hide `btnUsuario`, `btnRol`, and `btnTaller`.
  - If role is `Mecanico`, hide `btnCliente`, `btnVehiculo`, `btnModelo`, `btnMarca`.
- **SHALL** ensure `btnDashboard` and `btnReporteOrdenes` remain accessible according to operational permissions.
