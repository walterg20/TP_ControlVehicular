# Proposal: Mis Trabajos — Desktop Master–Detail Layout

## Why
The current "Mis Trabajos" screen is a single flat grid where **each row is a task**. An order with N tasks assigned to the same mechanic is repeated N times, so the mechanic cannot see the order as a unit, and the physical vehicle/order context is lost among duplicated rows. On a desktop form factor with wide screens and a mouse, the appropriate pattern is **Master–Detail**: a compact list of orders on top and the tasks of the selected order below.

## What
1. **Master grid (top)** — one row per order that has at least one task assigned to the logged-in mechanic. Columns: `Nº Orden`, `Fecha`, `Vehículo` (brand + model + plate), `Cliente`, `Km`, `Progreso` (own tasks done/total). Sorted by **order number descending** (newest first).
2. **Detail grid (bottom)** — the mechanic's own tasks for the selected order. Keeps the existing editing affordances: editable `Observaciones` and a `Revisado / OK` checkbox.
3. **Filter bar** — free-text search (order number, plate, client) and a `Mostrar` selector (`Pendientes` / `Finalizados` / `Todos`) applied to **orders** based on whether the mechanic still has pending tasks. Default: `Pendientes`.
4. **Open full order** — a `Ver Orden` button in the detail header and a double-click on a master row open `CtlOrdenServicioForm` for that order, keeping the `"MisTrabajos"` origin so navigation returns to this screen.
5. **Save** — `Guardar Cambios` persists every modified task across every order, then reloads preserving the selected order.

## Non-goals
- No changes to backend handlers, repositories, DTOs, or AutoMapper profiles. `RegistroServicioDto` already exposes `ClienteDetalle`, `KmIngreso`, `Estado`, `VehiculoPatente`, `VehiculoDetalle` and `Detalles`.
- No change to the `MisTrabajos.Ver` / `Reporte.Mecanico.Ver` permissions or role gating.
- No change to the generated PDF reports (Historial Clínico, Tareas del Día), beyond sourcing the vehicle from the selected order instead of the selected task.

## Impact
- `Presentacion/ViewModels/MisTrabajosViewModel.cs` — add `MiOrdenItemViewModel` (master) and `MiTareaItemViewModel` (detail), refactor `MisTrabajosViewModel` to master–detail.
- `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml` — rewrite layout, add master DataGrid + detail DataGrid + splitter.
- `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml.cs` — update `Ver Orden` to use the selected order and add master double-click.
