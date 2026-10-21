# Proposal: Filtros por Rango de Fechas en Listados y Reportes

## Why
El sistema de control vehicular maneja un volumen creciente de órdenes de servicio y asignaciones de trabajo. Sin embargo, la pantalla principal de gestión de órdenes (`CtlOrdenServicio`) y la pantalla de operaciones del mecánico (`CtlMisTrabajos`) carecían de filtrado por rango de fechas (`Desde` / `Hasta`), permitiendo únicamente búsqueda textual libre. Esto dificultaba la consulta operativa de trabajos del mes, semana o día, e impedía generar rápidamente un reporte impreso o en PDF de las órdenes resultantes del filtro aplicado.
Por su parte, los reportes existentes requerían botones para limpiar filtros con un solo clic y validaciones consistentes cuando el rango de fechas es invertido.

## What
1. **Listado Principal de Órdenes (`CtlOrdenServicio`)**:
   - Agregar selectores `DatePicker` para `Desde:` y `Hasta:`, inicializados por defecto en el mes actual (del 1° del mes a la fecha de hoy).
   - Agregar filtro por `Estado:` ("Todos", "Abierta", "En Proceso", "Completada", "Pagada", "Cancelada").
   - Agregar botón `🔄 Limpiar Filtros`.
   - Agregar botón `📄 Imprimir Listado` para exportar el listado filtrado a PDF en formato horizontal A4 con membrete oficial, logo institucional y pie de página.
2. **Mis Trabajos (`CtlMisTrabajos`)**:
   - Agregar selectores `DatePicker` para `Desde:` y `Hasta:`, inicializados por defecto en el mes actual.
   - Sincronizar el filtrado de fechas en el maestro de órdenes asignadas.
   - Permitir que la generación de "Tareas del Día" e "Historial Clínico" respete las fechas seleccionadas en pantalla.
3. **Consistencia en Reportes (`CtlReporteOrdenes`)**:
   - Inicializar por defecto en el mes actual.
   - Agregar botón `🔄 Limpiar Filtros`.
   - Validar rango de fechas y mostrar advertencia amigable si `FechaDesde > FechaHasta`.

## Impact
- **Archivos modificados**:
  - `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicio.xaml` y `OrdenServicioViewModel.cs`
  - `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml` y `MisTrabajosViewModel.cs`
  - `Presentacion/Pantalla/Reporte/CtlReporteOrdenes.xaml` y `ReporteOrdenesViewModel.cs`
- **Tests**: Nuevas pruebas unitarias de filtrado por rango de fechas en ViewModels.
