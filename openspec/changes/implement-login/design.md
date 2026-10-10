# Technical Design: User Login & Dynamic MainWindow Layout

## Architecture & Class Design

```
+-----------------------------------------------------------------------+
|                              PRESENTACION                             |
|                                                                       |
|  +------------------+         +------------------+                    |
|  |   MainWindow     | <-----> |   CtlLoginView   |                    |
|  +------------------+         +------------------+                    |
|          |                             |                              |
|          v                             v                              |
|  +-----------------------------------------------+                    |
|  |               LoginViewModel                  |                    |
|  +-----------------------------------------------+                    |
+--------------------------|--------------------------------------------+
                           |
                           v
+-----------------------------------------------------------------------+
|                                NEGOCIO                                |
|  +-----------------------------------------------+                    |
|  |           AutenticarUsuarioHandler            |                    |
|  +-----------------------------------------------+                    |
|                          |                                            |
|                          v                                            |
|  +-----------------------------------------------+                    |
|  |           ResultadoAutenticacion (Enum)        |                    |
|  +-----------------------------------------------+                    |
+--------------------------|--------------------------------------------+
                           |
                           v
+-----------------------------------------------------------------------+
|                                 DATOS                                 |
|  +-----------------------------------------------+                    |
|  |              IUsuarioRepository               |                    |
|  +-----------------------------------------------+                    |
|                          |                                            |
|                          v                                            |
|  +-----------------------------------------------+                    |
|  |               UsuarioRepository               |                    |
|  +-----------------------------------------------+                    |
+-----------------------------------------------------------------------+
```

## Layer Specifications

### 1. Negocio Layer (Business Logic)
- **`ResultadoAutenticacion` Enum**:
  - `Exitoso`
  - `UsuarioNoEncontrado`
  - `ContrasenaIncorrecta`
  - `UsuarioInactivo`
  - `ErrorBaseDatos`
- **`AuthResponseDto` DTO**:
  - `Resultado`: `ResultadoAutenticacion`
  - `Usuario`: `UsuarioDto?`
  - `Mensaje`: `string`
- **`AutenticarUsuarioHandler` Service**:
  - Injects `IUsuarioRepository` and `IMapper`.
  - Method: `Task<AuthResponseDto> HandleAsync(string nombre, string contrasena)`
  - Executes DB query with exception handling (`try / catch (Exception ex)`).

### 2. Presentacion Layer (UI & ViewModels)
- **`LoginViewModel`**:
  - Inherits `BaseViewModel`.
  - Properties: `NombreUsuario` (string), `Contrasena` (string), `ErrorMessage` (string), `IsLoading` (bool).
  - Commands: `IniciarSesionCommand` (`RelayCommand`).
  - Event: `event Action<UsuarioDto>? OnLoginSuccess`.
  - Client-side validation: Checks `string.IsNullOrWhiteSpace` for username and password.
- **`CtlLogin.xaml` & `CtlLogin.xaml.cs`**:
  - UserControl styled according to design rules:
    - `BorderCornerRadius: 12` (rounded-xl equivalent in WPF: `CornerRadius="12"`)
    - Padding: `Margin="24"` (p-6 equivalent)
    - Effect: `DropShadowEffect` (shadow-md equivalent)
  - Uses `PasswordBox` with bound password handler or password parameter.
- **`MainWindow.xaml` & `MainWindow.xaml.cs`**:
  - `Border x:Name="pnlSidebar"` for navigation menu.
  - Method `MostrarLogin()`: Sets `pnlSidebar.Visibility = Visibility.Collapsed`, clears `grdContenido`, injects `CtlLogin`.
  - Method `MostrarMenuPrincipal(UsuarioDto usuario)`: Sets `pnlSidebar.Visibility = Visibility.Visible`, clears `grdContenido`, sets active session user.

### 3. Dependency Injection (App.xaml.cs)
- Register `AutenticarUsuarioHandler` (`AddScoped`).
- Register `LoginViewModel` (`AddScoped`).
- Register `CtlLogin` (`AddScoped`).
