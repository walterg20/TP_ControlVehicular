# Referencia — Pantalla *Mis Trabajos* (mecánico)

Patrón maestro-detalle de la pantalla del mecánico. Se enlaza desde `AGENTS.md` → *Referencia por dominio*.

- `MisTrabajosViewModel` expone:
  - `Ordenes` / `OrdenesView` — filas `MiOrdenItemViewModel`, una por orden con tareas propias; progreso `hechas/total`.
  - `TareasDeOrden` — filas `MiTareaItemViewModel`, las tareas del mecánico de la orden seleccionada; `Observaciones` y `Realizado` editables inline.
- Orden por nº de orden descendente; filtro `Mostrar` = Pendientes / Finalizados / Todos.
- Doble clic en una orden abre `CtlOrdenServicioForm` (origen `"MisTrabajos"`).
