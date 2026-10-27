# Design: Role-based report visibility and company logo

## Database
- New script `bd/12_PermisosReportesPorRol.sql` (idempotent):
  - Inserts `Reporte.Ordenes.Ver`, `Reporte.Operativo.Ver`, `Reporte.Gerencial.Ver`, `Reporte.Mecanico.Ver`.
  - Admin (1): all four. Receptionist (2): Ordenes + Operativo, and the legacy `Reporte.Ver` is removed. Mechanic (3): Mecanico.
  - The legacy `Reporte.Ver` row stays in `Permisos` for backward compatibility, but the UI no longer reads it.

## Presentation
- `MainWindow.AplicarRestriccionesPorRol`: each report button is bound to its own permission. `secReportes` and `sepReportes` are visible only if at least one report is visible.

## PDF header
- `ReporteOrdenesViewModel` and `ReporteOperativoViewModel`: replace the private `ComposeHeader` / `ComposeFooter` with `ComposeEncabezadoTaller(title)` / `ComposePieDePagina()`. The applied-filters line moves to the top of the content.
- `ComprobanteRecepcionDocument` and `ComprobantePagoDocument`: add the logo image at the left of the existing header. Their dynamic workshop data is kept.

## Documentation
- `AGENTS.md` and `MEMORY.md` get new rules: mandatory logo header, report permission matrix, MEMORY.md update workflow, and plans stored in `docs/plan/`.
