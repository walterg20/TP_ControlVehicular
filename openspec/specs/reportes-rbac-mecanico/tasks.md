# Tasks: Mechanic report visibility, logo and folder-based branding

status: DONE

## Phase A — Branding foundation (no touch on pending files)
- [x] Task 1: Add `EmpresaBranding` in `Negocio/Reportes/Documentos/` (override folder +
      bundled fallback + `AsegurarCarpeta`). (commit `2a74bef`)
- [x] Task 2: Make `ReporteExtensions.RutaLogo` delegate to `EmpresaBranding.RutaLogo`.
      (commit `2a74bef`)

## Phase B — Mechanic screen (requires the pending files to be committed)
- [x] Task 3: `MisTrabajosViewModel`: add the `Reporte.Mecanico.Ver` permission check and
      expose it to the view (`PuedeVerReportesMecanico`); extend `PuedeImprimirHistorial`
      to include the permission. (commit `2a74bef`)
- [x] Task 4: `CtlMisTrabajos.xaml`: bind the "Historial Clínico" button visibility to the
      combined property and add the "Tareas del Día" button bound to
      `ImprimirHojaTrabajoCommand` and gated by `PuedeVerReportesMecanico`. (commit `2a74bef`)
- [x] Task 5: `MisTrabajosViewModel`: replace the ad-hoc text headers in
      `GenerarHistorialPdfAsync` and `GenerarHojaTrabajoPdfAsync` with
      `ComposeEncabezadoTaller(...)` + `ComposePieDePagina()`; move the dynamic subtitle to
      the content. (commit `2a74bef`)

## Phase C — Verification & docs
- [x] Task 6: `dotnet build "TP_ControlVehicular.slnx"` → 0 errors; `dotnet test
      "TP_ControlVehicular.Tests"` → 6/6 pass. (pre-existing `RbacTests` fix: `83bc9ee`)
- [x] Task 7: Manual check — mechanic buttons gated by permission, both PDFs show the logo,
      override-folder logo used when present and bundled logo otherwise. Verified by code
      inspection + build; the interactive visual check is left to the user.
- [x] Task 8: Update `AGENTS.md` / `MEMORY.md` with the logo override folder and the single
      resolution point. (this docs commit)

## Notes
- Phase B **must not start** until the user commits
  `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml` and
  `Presentacion/ViewModels/MisTrabajosViewModel.cs`. (Done in `8bee811`.)
- Commits use explicit paths only (never stage the user's pending files inadvertently).
