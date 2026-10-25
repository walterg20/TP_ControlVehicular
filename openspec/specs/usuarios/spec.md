# 007 · Usuarios y Roles (ABM)

**Estado:** implementado ✅

## Qué hace

Permite administrar el ABM (CRUD) completo de Usuarios y Roles en el sistema de control vehicular:
- **Gestión de Roles**: Alta, modificación, baja lógica/física y listado de Roles (`Nombre`, `Descripcion`, `Estado`).
- **Gestión de Usuarios**: Alta, modificación, baja lógica/física y listado de Usuarios (`Nombre`, `Contrasena`, `Estado`, `RolId`, `RolNombre`).

## Por qué

Proporciona la infraestructura de autenticación, autorización y control de acceso basada en roles (RBAC) para los operadores del sistema vehicular.

## Criterios de aceptación

- [x] Entidades `Rol` y `Usuario` definidas en `Entidad/` según el diagrama ER (Relación 1:N entre `Rol` y `Usuario`).
- [x] Mapeo EF Core en `CVDbContext` con tablas `Roles` y `Usuarios`.
- [x] Repositorios `IRolRepository` / `RolRepository` e `IUsuarioRepository` / `UsuarioRepository` implementados usando `_cvDbContext`.
- [x] DTOs `RolDto` y `UsuarioDto` mapeados en `MappingProfile` (incluyendo `RolNombre` en `UsuarioDto`).
- [x] Handlers de Negocio (`RegistrarRolHandler`, `ModificarRolHandler`, `ListarRolHandler`, `RegistrarUsuarioHandler`, `ModificarUsuarioHandler`, `ListarUsuariosHandler`) implementados.
- [x] `RolViewModel` y `UsuarioViewModel` implementados aplicando nombres explícitos de dominio (`_rolRepository`, `_usuarioRepository`, `_registrarUsuarioHandler`, etc.).
- [x] Pantallas XAML y Code-behind (`CtlRol`, `FrmRol`, `CtlUsuario`, `FrmUsuario`) adaptadas a la arquitectura MVVM e inyección de dependencias.
- [x] Migración EF Core aplicada exitosamente en la base de datos SQL Server.
- [x] Compilación exitosa de la solución `TP_ControlVehicular.slnx` sin errores.

## Fuera de alcance

- Encriptación avanzada de contraseñas / Hash de contraseña (se usará campo seguro en string por ahora, hash extensible en feature de Seguridad/Auth).
- Login / Gestión de Sesión activa (se implementará en la feature de Autenticación).
