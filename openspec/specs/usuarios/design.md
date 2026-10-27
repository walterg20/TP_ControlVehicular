# 007 · Usuarios y Roles — Plan

_Cómo se implementa lo descrito en `spec.md`. Debe respetar `AGENTS.md`._

## Enfoque

Implementación en 5 capas seguras del ABM de Usuarios y Roles siguiendo el diagrama ER y las convenciones del proyecto con nombres explícitos:
1. **Entidad**: `Rol.cs` y `Usuario.cs` en `Entidad/`.
2. **Datos**: Actualización de `CVDbContext.cs`, repositorios base `Repository<T>`, interfaces e implementaciones (`RolRepository`, `UsuarioRepository`).
3. **Negocio**: DTOs (`RolDto`, `UsuarioDto`), AutoMapper `MappingProfile.cs`, Handlers de lectura y escritura para ambas entidades.
4. **Presentación**: `RolViewModel` y `UsuarioViewModel` (con inyección de repositorio/handlers), formularios modales `FrmRol`, `FrmUsuario` y UserControls `CtlRol`, `CtlUsuario`.
5. **DI & Database Migration**: Registro en `App.xaml.cs` y migración EF Core `AddUsuariosYRoles`.

## Implementación

1. Crear entidades `Rol.cs` y `Usuario.cs`.
2. Registrar `DbSet<Rol>` y `DbSet<Usuario>` en `CVDbContext.cs`.
3. Crear `IRolRepository.cs`, `RolRepository.cs`, `IUsuarioRepository.cs`, `UsuarioRepository.cs`.
4. Crear DTOs `RolDto.cs`, `UsuarioDto.cs` y configurar en `MappingProfile.cs`.
5. Crear Handlers en `Negocio/Services/`.
6. Crear `RolViewModel.cs` y `UsuarioViewModel.cs`.
7. Diseñar/Adaptar vistas XAML y code-behind en `Presentacion/Pantalla/Rol/` y `Presentacion/Pantalla/Usuario/`.
8. Registrar todas las dependencias en `App.xaml.cs`.
9. Generar y aplicar migración EF Core (`dotnet ef migrations add AddUsuariosYRoles`).
10. Probar compilación (`dotnet build "TP_ControlVehicular.slnx"`).

## Decisiones

- **Nombres explícitos de dominio**: Aplicar la convención refactorizada (`vmUsuario`, `vmRol`, `_usuarioRepository`, `_rolRepository`, `_cvDbContext`).
- **Combo de Roles en FormUsuario**: `UsuarioViewModel` inyecta `IRolRepository` para cargar los roles activos en la selección del usuario.

## Riesgos

- **Relación FK entre Usuario y Rol**: Mitigación: asegurar que `RolId` corresponda a un Rol existente; EF Core configurará la restricción referencial.
