# Tasks: Reporte Operativo — Task-Level Mechanic Productivity

## 1. Data layer
- [x] 1.1 Add `bd/13_SP_ReporteProductividadMecanicos.sql`: `CREATE OR ALTER PROCEDURE sp_ReporteProductividadMecanicos` grouping `DetalleServicio` by `UsuarioId` over `RegistroServicio.Fecha`, returning `MecanicoId`, `MecanicoNombre`, `TareasAsignadas`, `Pendientes`, `EnCurso`, `TareasCompletadas`.
- [x] 1.2 Add `Negocio/DTOs/Reportes/ProductividadMecanicoDto.cs` (top-level DTO with `PorcentajeAvance` / `PorcentajeAvanceTexto`).
- [x] 1.3 Add `ObtenerProductividadMecanicosAsync(DateTime?, DateTime?)` to `IReporteGerencialRepository` and implement it in `Datos/Repositories/ReporteGerencialRepository.cs` (Dapper + SP, mirroring `ObtenerModelosMasReparadosAsync`).

## 2. Order report cleanup (point 7)
- [x] 2.1 Remove the silent `GetReportesDemostrativos()` fallback and the method itself from `Negocio/Services/ObtenerReporteOrdenesHandler.cs`; return only real data (empty on none/error).

## 3. ViewModel
- [x] 3.1 `ReporteOperativoViewModel.cs`: drop `ObtenerReporteOrdenesHandler` from the constructor; source `Productividad` from `ObtenerProductividadMecanicosAsync`.
- [x] 3.2 Remove the nested `ProductividadMecanicoDto` class (now lives in `Negocio/DTOs/Reportes`).
- [x] 3.3 Add totals properties + `OnPropertyChanged` after reload.

## 4. UI
- [x] 4.1 `CtlReporteOperativo.xaml`: add `Pendientes`, `En Curso`, `% Avance` columns and a summary card with totals.
- [x] 4.2 `ComposeContent` (PDF): add the breakdown columns and a totals row; keep logo header/footer helpers.

## 5. Verification
- [x] 5.1 `dotnet build "TP_ControlVehicular.slnx"` — 0 errors.
- [x] 5.2 Apply `bd/13_SP_ReporteProductividadMecanicos.sql` against the local DB.
- [x] 5.3 Smoke test (Recepcionista): all mechanics with tasks appear with non-zero completed counts for a range with real data; totals and `% Avance` are consistent; PDF mirrors the screen.
- [x] 5.4 Order report with no data is empty (no demo rows).

## 6. Documentation
- [x] 6.1 Write `docs/plan/2026-10-10_reporte-operativo-productividad-mecanicos.md` (+ walkthrough) and update `MEMORY.md`.
- [x] 6.2 `openspec validate --change reporte-operativo-productividad-mecanicos`.
