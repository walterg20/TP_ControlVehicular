# Cambiar Inicio de Sesión de Nombre a DNI

## Overview
Actualmente el sistema utiliza el campo `Nombre` y `Contraseña` para autenticar a los usuarios. Se requiere migrar este comportamiento para que la autenticación se realice usando el `DNI` (Documento Nacional de Identidad) como identificador principal, en lugar del nombre.

## Functional Requirements
1. **Interfaz de Usuario (UI)**:
   - En la pantalla de inicio de sesión (`CtlLogin.xaml`), el campo que solicita "Usuario" debe cambiar su etiqueta a "DNI".
   - El *placeholder* o texto de ayuda (si lo hubiera) y los mensajes de error de UI deben mencionar "DNI" en lugar de "Usuario".
   - El campo DNI debe aceptar solamente caracteres numéricos o validarse adecuadamente según el formato de DNI local (ej. sin espacios ni puntos).

2. **Capa de Presentación (ViewModels)**:
   - En `LoginViewModel.cs`, la propiedad `NombreUsuario` debe renombrarse a `DniUsuario` o `Dni` para mantener coherencia semántica.
   - Ajustar los mensajes de error: "Por favor, ingrese DNI y contraseña." en lugar del texto actual.

3. **Capa de Negocio (Handlers / Servicios)**:
   - `AutenticarUsuarioHandler` debe recibir el parámetro `dni` en su método `HandleAsync(string dni, string contrasena)`.
   - Modificar la búsqueda en la base de datos (mediante Entity Framework Core) para encontrar el registro donde `u.Dni == dni` en lugar de `u.Nombre == usuario`.
   - Si el DNI no se encuentra, devolver el mensaje "El DNI ingresado no existe" o similar.

4. **Entidad y Base de Datos**:
   - La entidad `Usuario` ya cuenta con el campo `Dni`. Asegurar que las consultas y las validaciones del login apuntan a este campo.

## Acceptance Criteria
- [ ] La pantalla de login muestra "DNI" en lugar de "Usuario".
- [ ] Al intentar ingresar, el sistema valida las credenciales contra la columna `Dni` de la tabla `Usuarios` en lugar de la columna `Nombre`.
- [ ] Mensajes de validación y de error se han actualizado para mencionar el DNI.
- [ ] El proceso de inicio de sesión, una vez validado con DNI, permite entrar correctamente al sistema y carga el menú acorde al Rol (sin regresiones de seguridad).
