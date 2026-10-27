# Proposal: Unificar Nomenclatura de Baja Lógica (`Estado` → `Activo`) en Rol y Usuario

## Why
`Rol` y `Usuario` usan `Estado` (bool) como flag de baja lógica, mientras el resto del sistema (`Cliente`, `Taller`, `Servicio`) usa `Activo`. Esta dualidad dificulta la lectura y el mantenimiento, y obliga a recordar qué convención aplica a cada entidad. Además, `Rol` no tiene forma explícita de listar inactivos ni de reactivar (solo un toggle), inconsistente con el patrón introducido en `baja-logica-consistente`.

## What Changes
- Renombrar `Estado` → `Activo` en `Rol` y `Usuario`: entidad, DTO (`RolDto`, `UsuarioDto`), repositorios (`RolRepository`, `UsuarioRepository`), ViewModels (`RolViewModel`, `UsuarioViewModel`), handlers (`AutenticarUsuarioHandler`), XAML (`CtlRol`, `CtlUsuario`, `FrmRol`, `FrmUsuario`) y scripts SQL (`bd/01_estructura.sql`, `bd/04_datos.sql`).
- `bd/01_estructura.sql`: columna `[Activo] bit NOT NULL DEFAULT (1)` en `Roles` y `Usuarios`.
- `RolViewModel`: propiedad `MostrarInactivos`, carga ramificada, `ReactivarAsync` (vía nuevo `ReactivarRolHandler`) y `ListarRolHandler.HandleAsync(bool incluirInactivos = false)`.
- UI de `CtlRol`: checkbox "Mostrar inactivos" en fila 1 y botón "♻️ Reactivar" en fila 3.
- Tests: actualizar referencias `dto.Estado` → `dto.Activo`.

## Impact
- **Capacidad**: `unificar-nomenclatura-activo` (nueva).
- **Archivos modificados**: `Entidad/Rol.cs`, `Entidad/Usuario.cs`, `Negocio/DTOs/RolDto.cs`, `Negocio/DTOs/UsuarioDto.cs`, `Datos/Repositories/RolRepository.cs`, `Datos/Repositories/UsuarioRepository.cs`, `Negocio/Services/AutenticarUsuarioHandler.cs`, `Negocio/Services/ListarRolHandler.cs`, nuevo `ReactivarRolHandler.cs`, `Presentacion/ViewModels/RolViewModel.cs`, `Presentacion/ViewModels/UsuarioViewModel.cs`, `Presentacion/Pantalla/Rol/*.xaml(.cs)`, `Presentacion/Pantalla/Usuario/*.xaml(.cs)`, `bd/01_estructura.sql`, `bd/04_datos.sql`, `App.xaml.cs`, tests.
- **No toca** `RegistroServicio.Estado`/`DetalleServicio.Estado` (estado de flujo, no flag de baja lógica).

## Non-goals
- No agregar "mostrar inactivos"/"reactivar" a `Usuario` (solo `Rol`, según alcance).
- No tocar `Marca`/`Modelo`/`Vehiculo` (sin flag).
- No migraciones EF Core (fuente sigue siendo `bd/`).
