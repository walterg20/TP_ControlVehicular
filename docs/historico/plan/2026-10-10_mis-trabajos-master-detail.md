# Plan — Mis Trabajos: rediseño a maestro-detalle (escritorio)

Spec formal: `openspec/changes/mis-trabajos-master-detail/`.

## Objetivo
Convertir la pantalla *Mis Trabajos* de una lista plana de tareas a un patrón
**maestro-detalle** de escritorio:
1. **Maestro (arriba)**: una fila por **orden** que tenga al menos una tarea del
   mecánico logueado. Columnas `Nº Orden`, `Fecha`, `Vehículo`, `Cliente`, `Km`,
   `Progreso`. Ordenado por nº de orden **descendente** (más reciente primero).
2. **Detalle (abajo)**: las **tareas propias** de la orden seleccionada, con edición
   inline (`Observaciones`, `Revisado / OK`) y `Guardar Cambios`.

## Problema que resuelve
Hoy cada fila es una tarea: una orden con N tareas del mismo mecánico se repite N veces
y se pierde el contexto vehículo/orden.

## Decisiones
- Orden del maestro: nº de orden **descendente**.
- Alcance del detalle: solo las tareas del mecánico logueado en esa orden.
- `Mostrar` por defecto: **Pendientes** (opciones Pendientes / Finalizados / Todos).
- Se elimina el combo de filtro por vehículo; la búsqueda libre cubre nº de orden,
  patente y cliente.
- Doble clic en una fila del maestro abre la orden completa (`CtlOrdenServicioForm`,
  origen `"MisTrabajos"`), además del botón `🔗 Ver Orden Completa`.
- Sin cambios en DTOs, repositorios, handlers ni permisos: `RegistroServicioDto` ya
  expone `ClienteDetalle`, `KmIngreso`, `Estado`, `VehiculoPatente`, `VehiculoDetalle`
  y `Detalles`.

## Fuera de alcance
- Cambios de backend, persistencia, esquema de base de datos o matriz de permisos.

## Tareas
Ver `openspec/changes/mis-trabajos-master-detail/tasks.md`.

## Notas
- `git add` con rutas explícitas; no arrastrar archivos ajenos.
- Edición de código/XAML con `[System.IO.File]::WriteAllText(...)` (UTF-8).
