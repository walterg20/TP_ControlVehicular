# 007 · Usuarios y Roles — Tareas

_Checklist accionable derivada del `plan.md`. Marca `[x]` al completarlas._

- [x] Crear entidad `Rol.cs` (`Id`, `Nombre`, `Descripcion`, `Estado`, `Usuarios`).
- [x] Crear entidad `Usuario.cs` (`Id`, `RolId`, `Nombre`, `Contrasena`, `Estado`, `Rol`).
- [x] Agregar `DbSet<Rol>` y `DbSet<Usuario>` en `CVDbContext.cs`.
- [x] Crear `IRolRepository` + `RolRepository` e `IUsuarioRepository` + `UsuarioRepository`.
- [x] Crear DTOs `RolDto.cs` y `UsuarioDto.cs` y mapeo en `MappingProfile.cs`.
- [x] Crear Handlers de Negocio para Rol (`RegistrarRolHandler`, `ModificarRolHandler`, `ListarRolHandler`).
- [x] Crear Handlers de Negocio para Usuario (`RegistrarUsuarioHandler`, `ModificarUsuarioHandler`, `ListarUsuariosHandler`).
- [x] Crear `RolViewModel.cs` y `UsuarioViewModel.cs`.
- [x] Crear/Adaptar vistas `CtlRol`, `FrmRol`, `CtlUsuario`, `FrmUsuario`.
- [x] Registrar dependencias en `App.xaml.cs`.
- [x] Crear y aplicar migración EF Core `AddUsuariosYRoles`.
- [x] Validar compilación limpia (`dotnet build "TP_ControlVehicular.slnx"`).
- [x] Validar criterios de aceptación de `spec.md`.

## Mantenimiento (checklist recurrente)

- [ ] Verificar que la adición de nuevos campos a `Usuario` o `Rol` se mantenga sincronizada en DTOs y Mappings.
