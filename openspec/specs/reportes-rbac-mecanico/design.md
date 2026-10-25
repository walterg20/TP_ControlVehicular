# Design: Mechanic report visibility, logo and folder-based branding

## Presentation — gating in "Mis Trabajos"
- `MisTrabajosViewModel : BaseViewModel` reuses the existing
  `TienePermiso("Reporte.Mecanico.Ver")` helper (which reads
  `MainWindow.UsuarioSesionActual.Permisos`). It exposes a flag, e.g.
  `PuedeVerReportesMecanico => TienePermiso("Reporte.Mecanico.Ver")`.
- "Historial Clínico" button: visible only when `PuedeVerReportesMecanico` **and** a task
  is selected. The existing `PuedeImprimirHistorial` getter is extended to include the
  permission, or a new combined property is added.
- "Tareas del Día" button: visible when `PuedeVerReportesMecanico`. It does not depend on
  the selection. It is bound to the existing `ImprimirHojaTrabajoCommand`.
- `CtlMisTrabajos.xaml`: add the "Tareas del Día" button next to "Imprimir Historial
  Clínico", following the project's button style (`#2980B9` blue / `#27AE60` green with
  `CornerRadius="6"`) and the existing `BooleanToVisibilityConverter`.

## PDF header — mechanic reports
- `MisTrabajosViewModel.GenerarHistorialPdfAsync`:
  - `page.Header().Element(c => c.ComposeEncabezadoTaller("Historial Clínico"))`.
  - `page.Footer().Element(c => c.ComposePieDePagina())`.
  - The dynamic line (vehicle + mechanic + date) moves to the beginning of `page.Content()`.
- `MisTrabajosViewModel.GenerarHojaTrabajoPdfAsync`: same pattern with
  `ComposeEncabezadoTaller("Tareas del Día")`.
- Remove any private `ComposeHeader` / `ComposeFooter` that does not show the logo.

## Branding — single resolution point
- New static class `EmpresaBranding` in `Negocio/Reportes/Documentos/`:

```csharp
public static class EmpresaBranding
{
    public static string CarpetaEmpresa => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TP_ControlVehicular", "Empresa");

    public static string RutaLogoOverride => Path.Combine(CarpetaEmpresa, "logo.png");

    public static string RutaLogoCompartido => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "Presentacion", "Assets", "logo.png");

    public static string RutaLogo =>
        File.Exists(RutaLogoOverride) ? RutaLogoOverride : RutaLogoCompartido;

    public static void AsegurarCarpeta() => Directory.CreateDirectory(CarpetaEmpresa);
}
```

- `ReporteExtensions.RutaLogo` delegates to `EmpresaBranding.RutaLogo`. `ComposeLogo`
  keeps its `File.Exists` guard.
- The override folder is **per-user** and created lazily (`AsegurarCarpeta`). It is not
  created by the installer; on another machine it starts empty and the bundled logo is
  used. The cross-machine source of truth is intended to be the database in the future
  `taller-branding` spec.

## Documentation
- `AGENTS.md` / `MEMORY.md`: document the logo override folder and that `ReporteExtensions`
  / `EmpresaBranding` is the single resolution point.

## Out of scope
- `empresa.json` (address/phone) and the DB-backed branding provider — reserved for the
  `taller-branding` spec.
