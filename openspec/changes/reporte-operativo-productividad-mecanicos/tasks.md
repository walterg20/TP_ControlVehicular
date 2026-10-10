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

## 7. Fixes applied during validation
- [x] 7.1 Refine `sp_ReporteProductividadMecanicos`: list only Mechanics (`u.RolId = 3`) and exclude tasks of cancelled orders (`rs.Estado <> 'Cancelada'`).
- [x] 7.2 Add `bd/14_Fix_ReporteOperativo.sql`: correct `DetalleServicio.UsuarioId` (details 7, 9, 11 → mechanic 2) and set detail 19 → `Finalizada`.
- [x] 7.3 Add constant `EstadoCancelada` in `Entidad/RegistroServicio.cs` and use it in `Presentacion/ViewModels/OrdenServicioViewModel.cs` (the old literal `"Cancelado"` never matched the persisted state).
- [x] 7.4 Add `bd/16_Fix_Colision_sp_ReporteIngresos.sql`: rename the admin variant to `sp_ReporteIngresosAdmin` and restore the 3-parameter `sp_ReporteIngresos`; update `bd/06`, `bd/07` and `Datos/Repositories/ReporteGerencialRepository.cs`.
- [x] 7.5 Add `bd/15_Limpieza_Artefactos_TestE2E.sql`: idempotent, FK-safe purge of E2E artifacts (`TDD%` plates / `EndToEnd` clients); fix the `EndToEndTallerTests` E2E test (sibling project, not tracked) to clean up after itself.
