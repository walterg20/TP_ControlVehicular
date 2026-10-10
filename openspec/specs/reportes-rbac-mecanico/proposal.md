# Proposal: Mechanic report visibility, logo and folder-based branding

## Why
- The mechanic reports (Vehicle clinical history and Daily tasks) are reachable only from the "Mis Trabajos" screen. Their buttons are not gated by `Reporte.Mecanico.Ver`, so any user who reaches that screen can generate them.
- Both mechanic PDFs (`MisTrabajosViewModel`) build their own text header with `page.Header().Text(...)` and therefore do not show the company logo. This breaks the mandatory-logo rule established in `reportes-rbac` (Phase A).
- There is no "Tareas del Día" button in the UI, even though the view model already has the command to generate that PDF.
- The logo path is hardcoded to the bundled asset (`Presentacion/Assets/logo.png`). A workshop cannot replace it with its own logo.

## What changes
- Gate the mechanic report buttons in "Mis Trabajos" with the `Reporte.Mecanico.Ver` permission.
- Add the "Tareas del Día" button, bound to the existing daily-tasks command and gated by the same permission.
- Replace the private text headers in both mechanic PDFs with the shared `ComposeEncabezadoTaller(title)` header (logo + workshop data) and `ComposePieDePagina()` footer. The dynamic subtitle (vehicle / mechanic / date) moves to the top of the content.
- Introduce a single branding resolution point, `EmpresaBranding`, that reads the logo from an override folder (`%LocalAppData%\TP_ControlVehicular\Empresa\logo.png`) and falls back to the bundled `Presentacion/Assets/logo.png`. `ReporteExtensions` delegates to it.
- Document the new logo source in `AGENTS.md` and `MEMORY.md`.

## Out of scope
- A database-backed per-workshop branding provider (logo, address, phone) and the upload UI in the "Taller" screen. That is planned as a separate spec (`taller-branding`).
- Adding an `empresa.json` file to the override folder. The folder is only read for `logo.png` in this spec; address/phone remain the current hardcoded values.

## Relationship to `reportes-rbac`
This spec supersedes the "Phase B" block of `reportes-rbac`, which was left blocked because the target files had uncommitted work under review by the user.
