# Tasks

## 1. Database Layer (Stored Procedures)

- [ ] 1.1 Add `sp_ReporteIngresosAdminPaginado` to `bd/03_procedimientos.sql` with `@Pagina` and `@TamanoPagina` using CTE `OFFSET ... FETCH NEXT` and `TotalRegistros`, and verify syntax.
- [ ] 1.2 Add `sp_ReporteIngresosMensualesAdmin` to `bd/03_procedimientos.sql` aggregating total revenues and invoice count by calendar month (`PeriodoMes`, `FechaMes`), and verify syntax.

## 2. DTOs and Data Access Layer

- [ ] 2.1 Create `ReporteIngresoMensualDto.cs` and `ReporteIngresoPaginadoDto.cs` in `Negocio/DTOs/Reportes/`, and verify build succeeds.
- [ ] 2.2 Add `ObtenerIngresosPorFechaPaginadoAsync` and `ObtenerIngresosMensualesAsync` to `IReporteGerencialRepository` and implement them with Dapper in `ReporteGerencialRepository.cs`.
- [ ] 2.3 Add unit tests in `TP_ControlVehicular.Tests` verifying repository pagination and monthly grouping mapping, and verify tests pass.

## 3. ViewModel & Pagination Commands

- [ ] 3.1 Add pagination properties (`PaginaActual`, `TotalPaginas`, `TotalRegistros`, `TamanoPagina`, `OpcionesTamanoPagina`) and navigation commands (`IrPrimeraPaginaCommand`, `PaginaAnteriorCommand`, `PaginaSiguienteCommand`, `IrUltimaPaginaCommand`) to `ReporteGerencialViewModel.cs`.
- [ ] 3.2 Add `IngresosMensuales` observable collection and loading logic in `ReporteGerencialViewModel.cs`.
- [ ] 3.3 Add unit tests in `TP_ControlVehicular.Tests` for `ReporteGerencialViewModel` testing page clamping, next/previous state, and page size resets.

## 4. User Interface & PDF Export

- [ ] 4.1 Add pagination navigation toolbar and monthly summary toggle/card to Tab 1 ("Ingresos Totales") in `Presentacion/Pantalla/Reporte/CtlReporteGerencial.xaml` conforming to UI conventions.
- [ ] 4.2 Update `ComposeIngresosContent` in `ReporteGerencialViewModel.cs` to include the monthly aggregated breakdown table and chart in the generated PDF export.
- [ ] 4.3 Run `dotnet build "TP_ControlVehicular.slnx"` and `dotnet test "TP_ControlVehicular.slnx"` ensuring 100% build success and test pass rate.
