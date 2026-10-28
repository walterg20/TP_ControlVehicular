# Proposal

## Why
As the volume of service orders and billing records grows (now with dozens of monthly records since June 2026), displaying all daily revenue rows at once in the Managerial Report degrades usability and responsiveness. Additionally, management needs executive visibility into total revenues grouped by month rather than solely flat daily entries, enabling clearer trend analysis across billing periods.

## What Changes
- **Database Pagination for Total Income Grid**: Implement server-side pagination powered by SQL Server (`OFFSET ... FETCH NEXT ... ROWS ONLY`) for the Managerial Income grid (`CtlReporteGerencial.xaml`), with pagination controls (Previous, Next, page indicator, and page size selection).
- **Monthly Income Grouping**:
  - Support monthly aggregation of revenues (`sp_ReporteIngresosMensualesAdmin` or grouping parameter in `sp_ReporteIngresosAdmin`) showing Year-Month, invoice count, and total billed amount.
  - In the Managerial Report PDF export and on-screen views, provide grouped monthly summaries of total revenue and subtotals.
- **Data Access & ViewModels**: Extend `IReporteGerencialRepository` with paginated and monthly grouped query methods, updating `ReporteGerencialViewModel` with pagination state commands (`PaginaActual`, `TotalPaginas`, `CambiarPaginaCommand`).

## Capabilities

### New Capabilities
None.

### Modified Capabilities
- `reportes`: Add requirements for database-level pagination on the Managerial Total Income grid and monthly revenue aggregation in the managerial report.

## Impact
- **Database (`bd/03_procedimientos.sql`)**: Add or update stored procedures to support paginated queries (`@Pagina`, `@TamanoPagina`, `TotalRegistros`) and monthly grouping (`YearMonth`, `CantidadFacturas`, `IngresosTotales`).
- **Data & Repository (`Datos/Repositories/ReporteGerencialRepository.cs`, `Negocio/Reportes/IReporteGerencialRepository.cs`)**: New repository contracts and DTOs (`ReporteIngresoPaginadoDto`, `ReporteIngresoMensualDto`).
- **Presentation (`Presentacion/ViewModels/ReporteGerencialViewModel.cs`, `Presentacion/Pantalla/Reporte/CtlReporteGerencial.xaml`)**: Pagination controls, page size selector, and monthly grouping view in the PDF export.
- **Unit & Integration Tests**: Update/add tests in `TP_ControlVehicular.Tests` to validate pagination logic and monthly grouping calculations.
