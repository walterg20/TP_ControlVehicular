# Proposal: Baja Lógica Consistente — Cliente, Taller y Servicio

## Why
La baja lógica del sistema está implementada de forma inconsistente: cada entidad usa una convención distinta (`Cliente.Activo`, `Taller.Activo`, `Servicio.Activo`, pero `Rol`/`Usuario` usan `Estado`), los defaults al crear registros son inconsistentes (`Cliente` y `Taller` nacen con `Activo = false` por falta de default en entidad y BD, enmascarado solo por la UI), y no existe una forma explícita de listar inactivos ni de reactivar un registro dado de baja. Además, `ListarServiciosHandler` lista **todos** los servicios (incluidos inactivos) porque usa `GetAllAsync()` sin filtro, de modo que un servicio dado de baja sigue apareciendo en la grilla. Esto hace que la baja lógica sea frágil, no auditable y, en la práctica, casi irreversible desde la UI.

## What Changes
- Unificar el default de alta: `Cliente.Activo` y `Taller.Activo` pasan a `= true` en la entidad, y `[Activo] bit NOT NULL DEFAULT (1)` en `bd/01_estructura.sql` (Servicio ya lo tiene).
- Agregar `GetTodosAsync()` al contrato base `IRepository<T>`/`Repository<T>` para listar explícitamente incluyendo inactivos, manteniendo `GetAllAsync()` y los `GetActivosAsync()` existentes.
- `ListarServiciosHandler.HandleAsync()` y `ListarTallerHandler.HandleAsync()` aceptan `incluirInactivos` (default `false`) y filtran por `GetActivosAsync()` por defecto.
- Nuevos handlers `ReactivarServicioHandler` y `ReactivarTallerHandler` (patrón de `EliminarServicioHandler`); en `ClienteViewModel` la reactivación se hace vía repositorio directo (ya inyectado).
- ViewModels `ClienteViewModel`, `TallerViewModel`, `ServicioViewModel`: propiedad `MostrarInactivos`, carga condicional y método `ReactivarAsync(int id)`.
- UI: control "Mostrar inactivos" en la barra superior (fila 1) y botón "♻️ Reactivar" en el pie (fila 3) de `CtlCliente`, `CtlTaller` y `CtlServicio`.
- Registro DI de los nuevos handlers en `App.xaml.cs`.

## Impact
- **Capacidad**: `baja-logica` (nueva).
- **Archivos modificados**: `Entidad/Cliente.cs`, `Entidad/Taller.cs`, `Datos/Repositories/Repository.cs`, `Negocio/Interfaces/IRepository.cs`, `Negocio/Services/ListarServiciosHandler.cs`, `Negocio/Services/ListarTallerHandler.cs`, nuevos `ReactivarServicioHandler.cs`/`ReactivarTallerHandler.cs`, `Presentacion/ViewModels/{Cliente,Taller,Servicio}ViewModel.cs`, `Presentacion/Pantalla/{Cliente,Taller,Servicio}/Ctl*.xaml` y `.xaml.cs`, `App.xaml.cs`, `bd/01_estructura.sql`.
- **Sin cambios de esquema físico** más allá del `DEFAULT (1)` en `[Activo]`; no se agregan tablas ni columnas.
- **No toca** `Rol`/`Usuario` (flag `Estado`): se abordan en la spec 2 `unificar-nomenclatura-activo`.

## Non-goals (fuera de alcance)
- Renombrar `Estado` → `Activo` en `Rol`/`Usuario` (spec 2).
- Baja lógica en `Marca`, `Modelo` y `Vehiculo` (hoy es borrado físico por FK; requiere rediseño aparte).
- Reportes/SPs: los 14 SP de reportes no filtran por `Activo`; se evalúan en spec 2.
- Migraciones EF Core (la fuente sigue siendo `bd/`).
