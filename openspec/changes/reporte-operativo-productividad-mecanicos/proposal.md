# Proposal: Reporte Operativo — Task-Level Mechanic Productivity

## Why
The "Reporte Operativo" productivity table groups by **order**, and each order contributes only the **first** detail's mechanic, while it compares the **order** state (`Abierta` / `En Proceso` / `Completada` / `Pagada`) against the **task** state `"Finalizada"`. As a result, a second mechanic whose tasks are never the first detail disappears from the table, and "Tareas Completadas" is always `0`.

## What Changes
- **Fix the aggregation unit**: mechanic productivity is computed per **task** (`DetalleServicio`) grouped by the assigned mechanic (`UsuarioId`), not per order. Each mechanic with at least one task in the selected range appears once.
- **Classify by task state**: completion is `DetalleServicio.Estado == "Finalizada"`; the table adds `Pendientes` (`Pendiente`), `En Curso` and `% Avance` (`Completadas / TareasAsignadas`).
- **Only mechanics with tasks in range** are listed (no `0`-task mechanics).
- **Totals**: the table gets a totals row and the screen a summary card.
- **Dedicated productivity source**: a new stored procedure + repository method returns the per-mechanic breakdown directly from the database (Dapper), independent from the order report.
- **Remove silent demo data**: `ObtenerReporteOrdenesHandler` no longer falls back to `GetReportesDemostrativos()` when there are no rows or an error occurs; it returns only real data (empty when there is none).
- **PDF**: the Reporte Operativo PDF shows the new breakdown columns and the totals row.

## Capabilities
- **Modified Capabilities**: `reportes` — Reporte Operativo productivity is task-level with a breakdown and totals; the order report no longer fabricates demonstration rows.

## Impact
- `bd/13_SP_ReporteProductividadMecanicos.sql` (new idempotent script).
- `Negocio/DTOs/Reportes/ProductividadMecanicoDto.cs` (new).
- `Negocio/Reportes/IReporteGerencialRepository.cs` + `Datos/Repositories/ReporteGerencialRepository.cs` (new method).
- `Negocio/Services/ObtenerReporteOrdenesHandler.cs` (remove demo fallback).
- `Presentacion/ViewModels/ReporteOperativoViewModel.cs` (rewire source, move DTO, totals).
- `Presentacion/Pantalla/Reporte/CtlReporteOperativo.xaml` (new columns + summary card).
- `docs/plan/2026-10-10_reporte-operativo-productividad-mecanicos.md` (execution plan).
