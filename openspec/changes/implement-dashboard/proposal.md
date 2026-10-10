# Proposal: Executive Dashboard Implementation

## Why
Users require a high-level overview of workshop operations upon logging in, including vehicle statistics, active work orders in progress, and delivered orders, as well as a quick view of vehicles currently present in the workshop.

## What
1. **Sidebar Menu Entry**: Add a `"📊 Dashboard"` button positioned at the top of the sidebar menu above the OPERACIONES section in `MainWindow.xaml`.
2. **Dashboard UserControl (`CtlDashboard.xaml`)**: Create a modern user control with 3 metric cards ("Vehículos Activos", "En Proceso", "Entregadas Hoy") and a DataGrid listing vehicles in the workshop.
3. **Backend Metric Handler**: Implement `ObtenerDashboardHandler` in `Negocio/Services` that retrieves aggregated counts and vehicle lists from `IVehiculoRepository`.
4. **ViewModel (`DashboardViewModel`)**: Expose reactive properties and asynchronous data loading for `CtlDashboard`.

## Out of Scope
- Interactive chart rendering (charts can be added in future iterations if requested).
- Custom date range filters for historical metrics beyond today's summary.
