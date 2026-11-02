# Tasks

## 1. Spec y datos

- [x] 1.1 Crear `Negocio/DTOs/DashboardSerieDto.cs` con `Etiqueta` (string) y `Valor` (decimal).
- [x] 1.2 Ampliar `Negocio/DTOs/DashboardMetricsDto.cs` con las series por rol (`IngresosPorMes`, `ModelosMasReparados`, `ServiciosPorRango`, `OrdenesPorEstado`, `TareasPorEstado`) y el avance del mecánico (`TareasAsignadas`, `TareasCompletadas`, `PorcentajeAvance`). Eliminar `VehiculosEnTaller`.
- [x] 1.3 Cargar las series en `ObtenerDashboardHandler` según el rol: gerenciales para Administrador (últimos 6 meses para ingresos, modelos y servicios), órdenes por estado para Recepcionista, tareas por estado + avance para Mecánico (sin filtro de fecha).

## 2. Lógica de gráfico y controles

- [x] 2.1 Crear `Presentacion/Graficos/GraficoCalculos.cs` con: escala del eje Y y cortes, formato abreviado de montos (`$150k`, `$1.2M`), agrupamiento "top N + Otros" y geometría de los sectores de la torta (ángulos) y del anillo de progreso.
- [x] 2.2 Crear `Presentacion/Pantalla/Compartido/CtlGraficoBarras.xaml(.cs)` (barras verticales).
- [x] 2.3 Crear `Presentacion/Pantalla/Compartido/CtlGraficoBarrasHorizontal.xaml(.cs)` (barras horizontales).
- [x] 2.4 Crear `Presentacion/Pantalla/Compartido/CtlGraficoTorta.xaml(.cs)` (torta con leyenda).
- [x] 2.5 Crear `Presentacion/Pantalla/Compartido/CtlGraficoAnilloProgreso.xaml(.cs)` (anillo de avance).

## 3. Dashboard

- [x] 3.1 Exponer en `DashboardViewModel` las colecciones de series, el avance del mecánico y las banderas `MostrarGraficosGerenciales`, `MostrarGraficoOrdenes`, `MostrarGraficoMecanico`; eliminar `VehiculosEnTaller`.
- [x] 3.2 Reemplazar la grilla de `CtlDashboard.xaml` por tres zonas con `Visibility` por rol y título por rol, organizando para el Administrador Ingresos a la izquierda y Modelos/Servicios a la derecha.

## 4. Pruebas

- [x] 4.1 Pruebas unitarias de `GraficoCalculos` (escala, cortes, formato de montos, top N + Otros, ángulos de la torta y del anillo).
- [x] 4.2 Pruebas unitarias del handler por rol: Administrador ve las tres series gerenciales y ninguna otra; Recepcionista ve órdenes por estado; Mecánico ve tareas por estado y avance, y ninguna serie gerencial.
- [ ] 4.3 Prueba STA que instancia los cuatro controles y verifica que enlazan datos sin fallar.

## 5. Cierre

- [ ] 5.1 Escribir `docs/plan/2026-10-29_dashboard-graficos-por-rol.md`.
- [ ] 5.2 Actualizar `docs/referencia/dashboard.md`, `MEMORY.md` y `AGENTS.md` (convención de gráficos WPF nativos).
- [ ] 5.3 Verificar `dotnet build` y `dotnet test`.
- [ ] 5.4 Merge a `develop` con `--no-ff` y archivar el change.
