# Plan de ejecución — Dashboard por perfil: métricas reales e ingresos mensuales de Admin

- **Spec:** `openspec/changes/dashboard-por-perfil/` (proposal / design / tasks / delta `dashboard`)
- **Rama:** `feature/dashboard-por-perfil` (base `develop`)
- **Fecha:** 2026-10-10

## Alcance
A1. Métricas **reales** en `ObtenerDashboardHandler` (eliminar la fabricación `Vehiculos.Count * 0.5` / `* 0.25`).
   - **Vehículos Activos** = vehículos distintos con al menos una orden en {Abierta, En Proceso}.
   - **En Proceso** = órdenes de trabajo en {Abierta, En Proceso}.
   - **Entregadas Hoy** = órdenes en {Completada, Pagada} con `Fecha` = hoy.
A2. **Scoping por rol**: Administrador y Recepcionista ven todo el taller; el Mecánico solo sus órdenes.
A3. Tarjeta **Ingresos del Mes**, exclusiva del Administrador (`sp_ReporteIngresosAdmin`).
A4. Botón **🔄 Actualizar** (faltaba en la vista, requerido por la spec).
A5. Sin datos de demostración: si no hay datos, se muestra cero/vacío.

## Fuera de alcance
- Gráficos en la vista WPF (el de torta vive solo en el PDF gerencial).
- Permisos nuevos (la tarjeta de ingresos reutiliza el rol Administrador).
- Cambios de esquema o SP nuevo (los ingresos reutilizan `sp_ReporteIngresosAdmin`).

## Decisiones clave
- Unidad de las métricas: la **orden de trabajo** (`RegistroServicio`), consistente con las convenciones de reportes.
- `EnProcesoCount` es a nivel orden, no a nivel tarea (`DetalleServicio`); no mezclar granularidades.
- **Entregadas Hoy** usa la fecha de la orden (`Fecha`) como aproximación: el modelo no tiene marca de entrega/finalización. Limitación documentada.
- Fila de resumen como `UniformGrid` con `Columns="{Binding TarjetasResumenVisible}"` (3 o 4 según el rol) para evitar espacios vacíos; la 4.ª tarjeta se controla además con `Visibility` ↔ `MostrarIngresos`.
- `ObtenerDashboardHandler` inyecta `IRegistroServicioRepository` + `IReporteGerencialRepository` + `IMapper`; se deja de usar `IVehiculoRepository`.

## Bitácora
- [x] Artefactos OpenSpec creados; `openspec validate dashboard-por-perfil` → *valid*, 4/4.
- [x] Main spec `dashboard` normalizada al formato `### Requirement:` + `#### Scenario:` (commit `477bee6`).
- [x] A1..A4 en código (`DashboardMetricsDto`, `ObtenerDashboardHandler`, `DashboardViewModel`, `CtlDashboard.xaml`).
- [x] Build (`dotnet build "TP_ControlVehicular.slnx"`) → 0 errores; tests → 6/6.
- [x] Verificación de encoding UTF-8 de la XAML (acentos + emoji, sin caracteres de reemplazo).
- [x] Merge a `develop`, actualizar `MEMORY.md` + `AGENTS.md`, archivar el change.

## Commits
- `2139eee` docs(openspec): add dashboard-por-perfil change artifacts
- `ce9003f` feat(dashboard): metricas reales por rol e ingresos mensuales de Admin
