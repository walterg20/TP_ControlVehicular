# Proposal: Role-Based Dashboard with Real Metrics and Admin Monthly Revenue

## Why
The current dashboard is a single, workshop-wide view that fabricates part of its numbers: `ObtenerDashboardHandler` derives `EnProcesoCount` and `EntregadasHoyCount` from the vehicle list (multiplying by fixed factors) instead of counting real work orders, so the cards do not match reality. There is no role scoping: a mechanic sees the whole workshop instead of only their own orders. The `"🔄 Actualizar"` button required by the spec does not exist, and the Administrador has no revenue view even though the managerial income data (`sp_ReporteIngresosAdmin`) is already available.

## What Changes
- Compute every dashboard metric from real data (no fabricated/demonstration values):
  - **Vehículos Activos** = distinct vehicles with at least one order in {Abierta, En Proceso}.
  - **En Proceso** = work orders in {Abierta, En Proceso}.
  - **Entregadas Hoy** = work orders in {Completada, Pagada} whose order date is today (the order date is used because the model has no delivery timestamp).
- Add role scoping:
  - Administrador and Recepcionista: whole-workshop data.
  - Mecánico: only work orders where they have at least one service detail.
- Add an Administrador-only **Ingresos del Mes** card (billed income from the first day of the current month through today).
- Add the missing **🔄 Actualizar** refresh button.
- No demo/fallback data: empty data shows zero/empty, never invented rows.

## Impact
- Affected capability: `dashboard`.
- Affected code: `ObtenerDashboardHandler`, `DashboardMetricsDto`, `DashboardViewModel`, `CtlDashboard.xaml`/`.xaml.cs`, DI in `App.xaml.cs`.
- No schema change and no new stored procedure are expected (Admin revenue reuses `sp_ReporteIngresosAdmin`).
