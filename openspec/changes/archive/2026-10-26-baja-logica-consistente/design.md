# Technical Design: Baja Lógica Consistente — Cliente, Taller y Servicio

## Contexto
La baja lógica existe hoy en tres variantes: `Cliente` y `Taller` la implementan con `Activo` pero sin default; `Servicio` la implementa con `Activo = true` por defecto; la UI de `CtlServicio` invoca `DeleteAsync` (baja lógica) pero `ListarServiciosHandler` usa `GetAllAsync()` y por eso los inactivos siguen apareciendo. No hay forma de listar inactivos ni de reactivar. Este cambio introduce el patrón uniforme `GetActivosAsync()` por defecto + `GetTodosAsync()` explícito + acción "Reactivar", sin tocar `Rol`/`Usuario` (spec 2).

## Decisiones de diseño

### 1. Defaults de alta
- `Entidad/Cliente.cs`: `public bool Activo { get; set; } = true;`
- `Entidad/Taller.cs`: `public bool Activo { get; set; } = true;`
- `Servicio.Activo` ya es `= true`.
- `bd/01_estructura.sql`: columna `[Activo] bit NOT NULL DEFAULT (1)` en `Cliente`, `Taller` y `Servicio` (idempotente vía `DEFAULT`).

### 2. Contrato base `IRepository<T>`
Se agrega `Task<IEnumerable<T>> GetTodosAsync();` a la interfaz y a `Repository<T>` (implementación `AsNoTracking().ToListAsync()`), sin modificar `GetAllAsync()` (se mantiene por compatibilidad; los handlers migran a `GetActivosAsync`/`GetTodosAsync` según corresponda).

### 3. Handlers de listado con parámetro opcional
- `ListarServiciosHandler.HandleAsync(bool incluirInactivos = false)`: si `false` usa `GetActivosAsync()`; si `true` usa `GetTodosAsync()`.
- `ListarTallerHandler.HandleAsync(bool incluirInactivos = false)`: idem.
- `Cliente` no tiene handler de listado (usa repositorio directo en el VM); el VM decide entre `GetActivosAsync()`/`GetTodosAsync()`.

### 4. Handlers de reactivación
- `ReactivarServicioHandler` y `ReactivarTallerHandler`, análogos a `EliminarServicioHandler`: `GetByIdAsync` → `Activo = true` → `UpdateAsync`.
- `Cliente`: reactivación directa en `ClienteViewModel.ReactivarClienteAsync(int id)` usando `_clienteRepository`.

### 5. ViewModels
Cada VM (`ClienteViewModel`, `TallerViewModel`, `ServicioViewModel`) incorpora:
- `bool MostrarInactivos` (con setter que dispara recarga).
- `LoadAsync()` ramificado por `MostrarInactivos`.
- `ReactivarAsync(int id)` (o `ReactivarClienteAsync`) que pone `Activo = true` y recarga.

`ServicioViewModel` pasa a inyectar `IServicioRepository`? No: se inyecta `ReactivarServicioHandler` y se modifica `ListarServiciosHandler` con el parámetro. Para listar todos se llama `HandleAsync(true)`.

### 6. UI
- Fila 1: `CheckBox "Mostrar inactivos"` vinculado a `MostrarInactivos` (con recarga al cambiar).
- Fila 3: botón `♻️ Reactivar` (color `#2980B9`) junto a Editar/Eliminar, habilitado cuando hay selección de un inactivo (o siempre, validando en handler).

### 7. DI
Registrar `ReactivarServicioHandler` y `ReactivarTallerHandler` como `AddScoped` en `App.xaml.cs`.

## Riesgos y mitigación
| Riesgo | Mitigación |
|---|---|
| Cambiar firma de `HandleAsync` rompe llamadas existentes | Parámetro opcional con default `false` mantiene compatibilidad. |
| `GetTodosAsync` en base genérica requiere que T tenga `Activo` | Solo se invoca para entidades que tienen `Activo` (Cliente, Taller, Servicio); el contrato es genérico pero el uso es específico. |
| Recarga concurrente al togglear | El setter dispara `LoadAsync` fire-and-forget; los UC registran `Loaded` una sola vez (convención existente). |

## Verificación
1. `dotnet build "TP_ControlVehicular.slnx"` sin errores.
2. `dotnet test "TP_ControlVehicular.slnx"` sin regresiones.
3. Verificación manual: crear Cliente/Taller/Servicio → `Activo=true`; dar de baja → desaparece del listado por defecto; activar "Mostrar inactivos" → aparece; "Reactivar" → vuelve al listado por defecto.
