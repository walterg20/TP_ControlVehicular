# Proposal: Managerial Report — Explicit Query, Service Breakdown Pie Chart, and Cleanup

## Why
The managerial report queries the database on every date change, never exposes the quantity of each service performed in the selected range, and the "most repaired models" table shows the literal text `{Marca} {Modelo}` instead of the real model name because of a broken string interpolation. On top of that, a dead `ReporteGerencialView` control duplicates the report and is unreachable.

## What Changes
- Add an explicit **Consultar** button to `CtlReporteGerencial`. Date range changes no longer trigger automatic queries. An invalid range (start date after end date) is rejected with a validation message and does not query.
- Add a **service quantity breakdown** (the sum of the quantity registered per service line over the selected range) as a new report section: shown as a table on the screen and as a **pie chart only in the managerial PDF**.
- Fix the **most repaired models** table so each row displays the composed model name `"<Brand> <Name>"`.
- Remove the dead `ReporteGerencialView` control and its dependency-injection registration.
- The managerial PDF continues to show the company logo in its header.

## Capabilities
### New Capabilities
(none)

### Modified Capabilities
- `reportes`: The **Managerial Report** requirement gains an explicit-query interaction, a corrected model-name column, and a new **Service Quantity Breakdown** requirement (table on screen, pie chart in the PDF).

## Impact
- ViewModels/UI: `Presentacion/ViewModels/ReporteGerencialViewModel.cs`, `Presentacion/Pantalla/Reporte/CtlReporteGerencial.xaml(.cs)`
- Domain/data: `Negocio/DTOs/Reportes/`, `Negocio/Reportes/IReporteGerencialRepository.cs`, `Datos/Repositories/ReporteGerencialRepository.cs`
- Database: new stored procedure `sp_ReporteServiciosPorRango` (`bd/18_...sql`)
- PDF: new chart helper under `Negocio/Reportes/Documentos/`
- Removal: `Presentacion/Pantalla/Reporte/ReporteGerencialView.xaml(.cs)` and its registration in `App.xaml.cs`
- No new business permission is required; the report stays behind `Reporte.Gerencial.Ver` (Administrator only).
