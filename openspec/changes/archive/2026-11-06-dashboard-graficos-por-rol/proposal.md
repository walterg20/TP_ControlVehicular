# Proposal: Dashboard con gráficos por rol

## Why

El DashboardGeneral termina siempre en la misma grilla: **"🚗 Vehículos en el Taller"**. Ese bloque es idéntico para los tres roles, y por lo tanto no aporta nada a quien lo mira:

- El **Administrador** (que es el único con permiso de *Reporte Gerencial*) no ve ningún gráfico de los reportes que puede consultar: los ingresos por mes, los modelos más reparados y el desglose de servicios solo existen dentro de los PDF.
- El **Recepcionista** manages la entrada de vehículos, pero el dashboard no le muestra cómo se distribuyen las órdenes por estado, que es su consulta diaria.
- El **Mecánico** no tiene ninguna visual de su propio avance: el *Reporte Mecánico* (productividad) es una tabla dentro del *Reporte Operativo*, en otra pantalla.

El espacio inferior del dashboard es, en resumen, el único que no se adapta al rol, cuando es justo el espacio con más espacio en pantalla.

## What Changes

- El bloque **"Vehículos en el Taller" se reemplaza por visuales según el rol**, en los tres casos:
  - **Administrador**: los tres gráficos del Reporte Gerencial — ingresos por mes (barras verticales), modelos más reparados (barras horizontales) y servicios por rango (torta).
  - **Recepcionista**: barras con las órdenes por estado (Abierta, En Proceso, Completada, Pagada, Cancelada).
  - **Mecánico**: barras con sus tareas por estado (Pendiente, En Curso, Finalizada) más un anillo con su porcentaje de avance.
- Los gráficos se dibujan con **controles WPF nativos**, sin dependencias nuevas: los gráficos que existen hoy son cadenas SVG embebidas en los PDF de QuestPDF y WPF no puede renderizar SVG.
- Se **elimina la grilla** de vehículos en el dashboard (y su lista `VehiculosEnTaller`), pero **se conserva la tarjeta "Vehículos Activos"** de la fila de resumen, que cuenta los mismos vehículos.
- No se requiere SQL nuevo: los tres ranking gerenciales ya tienen consulta paginada en base de datos (`sp_ReporteTopClientesPaginado`, `sp_ReporteModelosReparadosPaginado`, `sp_ReporteServiciosPorRangoPaginado`) y las series de órdenes y tareas se calculan desde las órdenes que el handler ya trae.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `dashboard`: el bloque inferior pasa de ser una grilla de vehículos a ser un conjunto de gráficos que dependen del rol; se ajusta la coherencia entre las tarjetas de resumen y el bloque inferior, y se extiende el alcance por rol a los nuevos visuales.

## Impact

- **Datos** (`Negocio/DTOs/DashboardMetricsDto.cs`, `Negocio/DTOs/DashboardSerieDto.cs` — nuevo, `Negocio/Services/ObtenerDashboardHandler.cs`): nuevas series por rol y el avance del mecánico; se elimina `VehiculosEnTaller`.
- **Presentación** (`Presentacion/ViewModels/DashboardViewModel.cs`, `Presentacion/Pantalla/Dashboard/CtlDashboard.xaml`): colecciones de series, banderas de visibilidad por rol y tres zonas en la tarjeta inferior.
- **Nuevo** (`Presentacion/Pantalla/Compartido/CtlGraficoBarras.xaml`, `CtlGraficoBarrasHorizontal.xaml`, `CtlGraficoTorta.xaml`, `CtlGraficoAnilloProgreso.xaml`, `Presentacion/Graficos/GraficoCalculos.cs`): controles de gráfico reutilizables y la lógica de cálculo aislada y testeable.
- **Tests**: unitarios de la lógica de cálculo y del handler por rol; una prueba STA que instancia los controles.
- **Docs**: `docs/referencia/dashboard.md`, `MEMORY.md`, `AGENTS.md`.

## Non-Goals

- No se agrega ningún selector de rango de fechas al dashboard: cada gráfico usa su rango fijo.
- No se modifican las pantallas de reportes: el dashboard los representa, no los reemplaza.
- No se agregan paquetes NuGet: los gráficos son controles WPF nativos.
