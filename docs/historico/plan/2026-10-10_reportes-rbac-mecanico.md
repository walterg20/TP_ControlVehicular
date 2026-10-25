# Plan — Reportes del mecánico (Fase B): permiso, logo y branding por carpeta

Continúa la Fase A de [`2026-10-10_reportes-rbac-logo.md`](./2026-10-10_reportes-rbac-logo.md).
Spec formal: `openspec/specs/reportes-rbac-mecanico/`.

## Objetivo
1. **Visibilidad de reportes del mecánico por permiso**: los botones de "Historial Clínico"
   y "Tareas del Día" (en *Mis Trabajos*) deben mostrarse solo con `Reporte.Mecanico.Ver`.
2. **Logo en los PDF del mecánico**: ambos PDF deben usar el encabezado con logo
   (`ComposeEncabezadoTaller`) y el pie (`ComposePieDePagina`).
3. **Botón "Tareas del Día"**: agregarlo en la pantalla del mecánico.
4. **Logo desde carpeta**: fuente única `EmpresaBranding` que lee
   `%LocalAppData%\TP_ControlVehicular\Empresa\logo.png` y cae al asset embebido
   `Presentacion/Assets/logo.png`. Anticipa un futuro feature de carga de logo/dirección/
   teléfono del taller.

## Estado actual (evidencia)
| Elemento | Dónde | Estado hoy |
|---|---|---|
| Botón Historial Clínico | `CtlMisTrabajos.xaml` | Visible según `PuedeImprimirHistorial` (solo selección, **sin permiso**) |
| Botón Tareas del Día | `CtlMisTrabajos.xaml` | **No existe** (el comando sí: `ImprimirHojaTrabajoCommand`) |
| PDF Historial Clínico | `MisTrabajosViewModel.GenerarHistorialPdfAsync` | `page.Header().Text(...)` → **sin logo** |
| PDF Tareas del Día | `MisTrabajosViewModel.GenerarHojaTrabajoPdfAsync` | `page.Header().Text(...)` → **sin logo** |
| Ruta del logo | `ReporteExtensions.RutaLogo` | Hardcodeada al asset embebido |

## Alcance de la Fase B
- Gating de los botones del mecánico con `Reporte.Mecanico.Ver`.
- Botón "Tareas del Día".
- Encabezado/pie con logo en ambos PDF del mecánico.
- `EmpresaBranding` (carpeta override + respaldo embebido) y `ReporteExtensions.RutaLogo`
  delegando a ella.
- Actualización de `AGENTS.md` / `MEMORY.md`.

## Fuera de alcance
- `empresa.json` (dirección/teléfono) y el proveedor de branding respaldado por base de
  datos con su UI en la pantalla *Taller*. Queda como spec aparte (`taller-branding`).

## Bloqueo
La Fase B **no arranca** hasta que el usuario commitee sus cambios pendientes en:
- `Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml`
- `Presentacion/ViewModels/MisTrabajosViewModel.cs`

La Fase A (foundation de branding: `EmpresaBranding` + `ReporteExtensions`) no toca esos
archivos y puede hacerse antes, si el usuario lo autoriza.

## Tareas
Ver `openspec/specs/reportes-rbac-mecanico/tasks.md` (Phases A/B/C).

## Notas
- `git add` siempre con rutas explícitas; nunca arrastrar los archivos pendientes del usuario.
- Edición de código/XAML con `[System.IO.File]::WriteAllText(...)` (UTF-8).
