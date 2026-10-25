# Design: Login con DNI

## Arquitectura y Componentes Afectados

El flujo de inicio de sesión actual es:
`CtlLogin (View)` -> `LoginViewModel (VM)` -> `AutenticarUsuarioHandler (Handler)` -> `UsuarioRepository / EF (DB)`.

Para migrar a DNI se deben modificar los siguientes archivos:

### 1. `Presentacion/Pantalla/Login/CtlLogin.xaml`
* **Cambio Visual**: Cambiar `<TextBlock Text="Usuario"...>` a `<TextBlock Text="DNI"...>`.
* **Binding**: Cambiar el `Binding` del `TextBox` de `NombreUsuario` a `Dni`.

### 2. `Presentacion/ViewModels/LoginViewModel.cs`
* **Refactor de Propiedad**: Renombrar la propiedad y su campo de respaldo `_nombreUsuario` / `NombreUsuario` a `_dni` / `Dni`.
* **Validación**: Actualizar el chequeo `string.IsNullOrWhiteSpace(Dni)` y el mensaje `ErrorMessage = "Por favor, ingrese DNI y contraseña."`.
* **Llamada al Handler**: Actualizar la invocación de `await _autenticarUsuarioHandler.HandleAsync(Dni.Trim(), Contrasena)`.

### 3. `Negocio/Services/AutenticarUsuarioHandler.cs` (o donde se aloje el handler)
* **Firma del método**: Actualizar el parámetro de entrada de `string nombreUsuario` a `string dni`.
* **Consulta de BD**: Cambiar la consulta LINQ que actualmente hace `.FirstOrDefaultAsync(u => u.Nombre == nombreUsuario && u.Estado)` para que haga `.FirstOrDefaultAsync(u => u.Dni == dni && u.Estado)`.
* **Mensajes de Error**: Si el usuario no existe, cambiar el mensaje a "El DNI ingresado no se encuentra registrado."

## Riesgos y Consideraciones
* **DNI Duplicados**: Si existiera más de un usuario con el mismo DNI, el login tomaría el primero (`FirstOrDefaultAsync`). En la especificación CRUD de usuarios se debió validar que el DNI sea único. Es recomendable asegurarse de que la base de datos tenga restricciones de DNI único o que la capa Negocio evite registrar DNIs duplicados.
* **Actualización en el Dashboard/Header**: Actualmente el `MainWindow` puede que muestre `UsuarioSesionActual.Nombre`. El cambio en el login **NO** implica cambiar el "Nombre" de la sesión que se muestra arriba. Se seguirá mostrando el Nombre Real del usuario (`Nombre` y `Apellido`), sólo cambiaremos la credencial de acceso.
