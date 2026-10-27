# Technical Design: Unificar Nomenclatura de Baja Lógica (`Estado` → `Activo`)

## Contexto
`Rol` y `Usuario` usan `Estado` (bool) como flag de baja lógica; el resto usa `Activo`. Se renombra para unificar. `Rol` además adopta el patrón `MostrarInactivos` + `Reactivar` introducido en `baja-logica-consistente`. `Usuario` solo se renombra (mantiene su toggle actual).

## Decisiones
1. Renombrado mecánico `Estado` → `Activo` en Rol y Usuario (entidad, DTO, repo, VM, handler, XAML, SQL).
2. `bd/01_estructura.sql`: `[Activo] bit NOT NULL DEFAULT (1)` en `Roles` y `Usuarios`.
3. `bd/04_datos.sql`: columnas `[Estado]` → `[Activo]` en INSERT de Roles/Usuarios.
4. `RolRepository`/`UsuarioRepository`: filtro por `Activo`.
5. `ListarRolHandler.HandleAsync(bool incluirInactivos = false)`.
6. Nuevo `ReactivarRolHandler`.
7. `RolViewModel`: `MostrarInactivos`, carga ramificada, `ReactivarAsync`, `ToggleActivoAsync` (renombrado).
8. `UsuarioViewModel`: renombrado `Estado`→`Activo`, `ToggleActivoAsync`.
9. UI `CtlRol`: checkbox + botón Reactivar; columna "Activo". `CtlUsuario`: columna "Activo".

## Verificación
- `dotnet build` sin errores; `dotnet test` sin regresiones.
- SQL: Roles/Usuarios con `[Activo]`.
