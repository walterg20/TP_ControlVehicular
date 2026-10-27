# Tasks: reportes-rbac
status: PHASE A DONE — Phase B moved to `reportes-rbac-mecanico` (done there)

## Phase A
- [x] Task 1: Create `bd/12_PermisosReportesPorRol.sql`. (commit `22fe4e8`)
- [x] Task 2: Per-report visibility in `MainWindow.AplicarRestriccionesPorRol`. (commit `56c10a2`)
- [x] Task 3: Use `ComposeEncabezadoTaller` in the Ordenes and Operativo report PDFs. (commit `83147fa`)
- [x] Task 4: Add the logo to the Recepcion and Pago receipts. (commit `83147fa`)
- [x] Task 5: Update `AGENTS.md` and `MEMORY.md` (logo rule, report matrix, MEMORY.md workflow, `docs/plan`). (commit `c4466af`)
- [x] Task 6: Build verification. (`dotnet build` 0 errors; `dotnet test` 6/6 passed)

## Phase B — moved to `reportes-rbac-mecanico` (DONE there)
- [x] Task 7: Gate the mechanic report buttons with `Reporte.Mecanico.Ver`. → `reportes-rbac-mecanico` Tasks 3/4
- [x] Task 8: Use `ComposeEncabezadoTaller` in the Vehicle history and Daily tasks PDFs. → Task 5
- [x] Task 9: Add the "Tareas del Día" button and the folder-based logo source (`EmpresaBranding`). → Tasks 1/2/4
