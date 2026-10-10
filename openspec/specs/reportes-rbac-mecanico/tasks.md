# Tasks: Mechanic report visibility, logo and folder-based branding

## Phase A — Branding foundation (no touch on pending files)
- [ ] Task 1: Add `EmpresaBranding` in `Negocio/Reportes/Documentos/` (override folder +
      bundled fallback + `AsegurarCarpeta`).
- [ ] Task 2: Make `ReporteExtensions.RutaLogo` delegate to `EmpresaBranding.RutaLogo`.

## Phase B — Mechanic screen (requires the pending files to be committed)
- [ ] Task 3: `MisTrabajosViewModel`: add the `Reporte.Mecanico.Ver` permission check and
      expose it to the view (`PuedeVerReportesMecanico`); extend `PuedeImprimirHistorial`
      to include the permission.
- [ ] Task 4: `CtlMisTrabajos.xaml`: bind the "Historial Clínico" button visibility to the
      combined property and add the "Tareas del Día" button bound to
      `ImprimirHojaTrabajoCommand` and gated by `PuedeVerReportesMecanico`.
- [ ] Task 5: `MisTrabajosViewModel`: replace the ad-hoc text headers in
      `GenerarHistorialPdfAsync` and `GenerarHojaTrabajoPdfAsync` with
      `ComposeEncabezadoTaller(...)` + `ComposePieDePagina()`; move the dynamic subtitle to
      the content.

## Phase C — Verification & docs
- [ ] Task 6: `dotnet build "TP_ControlVehicular.slnx"` → 0 errors; `dotnet test
      "TP_ControlVehicular.Tests"` → all pass.
- [ ] Task 7: Manual check — mechanics' buttons gated by permission, both PDFs show logo,
      override-folder logo is used when present and the bundled logo otherwise.
- [ ] Task 8: Update `AGENTS.md` / `MEMORY.md` with the logo override folder and the single
      resolution point.

## Notes
- Phase B **must not start** until the user commits
  `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml` and
  `Presentacion/ViewModels/MisTrabajosViewModel.cs`.
- Commits use explicit paths only (never stage the user's pending files inadvertently).
