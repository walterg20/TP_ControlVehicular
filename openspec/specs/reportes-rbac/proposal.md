# Proposal: Role-based report visibility and mandatory company logo

## Why
- A single `Reporte.Ver` permission currently unlocks every report in the sidebar. As a result, the Receptionist can open the Management report, which exposes income figures.
- Only the management PDFs use the shared header with the company logo (`ComposeEncabezadoTaller`). The other PDFs each build their own header, so the generated documents look inconsistent.

## What changes
- One permission per report: `Reporte.Ordenes.Ver`, `Reporte.Operativo.Ver`, `Reporte.Gerencial.Ver`, `Reporte.Mecanico.Ver`.
- The sidebar shows each report button according to its own permission.
- Every PDF header uses `ComposeEncabezadoTaller`, which includes the company logo.
- The new rules (logo, report matrix, MEMORY.md workflow, `docs/plan`) are documented in `AGENTS.md` and `MEMORY.md`.

## Out of scope (Phase B)
- Gating and logo for the mechanic PDFs in `MisTrabajosViewModel` / `CtlMisTrabajos`. These files have uncommitted work under review by the user.
