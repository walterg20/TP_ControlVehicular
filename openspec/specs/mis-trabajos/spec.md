# Especificación: Panel "Mis Trabajos"

## 1. Objetivo
Proveer una interfaz optimizada para los mecánicos donde puedan ver rápidamente los servicios (ítems) que les fueron asignados, conocer el vehículo y registrar su finalización.

## 2. Requerimientos
* Mostrar una grilla (DataGrid) enfocada en DetalleServicio en lugar de RegistroServicio completo.
* Filtro automático: si el usuario es Mecánico, la grilla solo carga los detalles asignados a su ID.
* Si el usuario es Administrador (o Recepcionista con permisos), incluir un ComboBox para filtrar por un mecánico específico o ver "Todos".
* La grilla debe mostrar: Fecha de Orden, Patente, Tarea (Servicio), Estado (Pendiente, En Proceso, Finalizada).
* Botón/Checkbox rápido en la fila para marcar el ítem como "Terminado" directamente desde esta pantalla sin abrir la orden completa.

## 3. Criterios de Aceptación
- [ ] El mecánico solo ve los trabajos asignados a él.
- [ ] Se puede marcar un trabajo como finalizado desde la grilla.
- [ ] Al cambiar el estado de un trabajo desde aquí, se evalúa si toda la Orden debe cambiar a estado "Completada".