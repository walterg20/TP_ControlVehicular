# Tasks: Managerial Report Bar Charts

- [x] 1. Create `IngresosBarChartGenerator` in `Negocio/Reportes/Documentos/` with SVG generation for vertical income bars.
- [x] 2. Create `ModelosBarChartGenerator` in `Negocio/Reportes/Documentos/` with SVG generation for horizontal ranking bars and XML character escaping.
- [x] 3. Integrate both chart generators into `ReporteGerencialViewModel.cs` inside `ComposeIngresosContent` and `ComposeModelosContent`.
- [x] 4. Add unit test suites in `TP_ControlVehicular.Tests`:
  - `IngresosBarChartGeneratorTests.cs`
  - `ModelosBarChartGeneratorTests.cs`
- [x] 5. Verify solution with `dotnet build "TP_ControlVehicular.slnx"` and `dotnet test "TP_ControlVehicular.slnx"`.
