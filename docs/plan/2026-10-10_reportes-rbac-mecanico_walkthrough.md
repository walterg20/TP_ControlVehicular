# Walkthrough — Reportes del mecánico (Fase B): permiso, logo y branding por carpeta

Bitácora de ejecución del plan [`2026-10-10_reportes-rbac-mecanico.md`](./2026-10-10_reportes-rbac-mecanico.md).
Continúa la Fase A de [`2026-10-10_reportes-rbac-logo_walkthrough.md`](./2026-10-10_reportes-rbac-logo_walkthrough.md).
Spec formal: `openspec/specs/reportes-rbac-mecanico/`.

> [!NOTE]
> `openspec/` = especificación formal (inglés). `docs/plan/` = plan y bitácora de ejecución (español).

## Resumen

La Fase B cierra la visibilidad y el branding de los reportes del **mecánico** en la
pantalla *Mis Trabajos*:

1. **Gating por permiso**: los botones "Imprimir Historial Clínico" y "Tareas del Día" se
   muestran solo con `Reporte.Mecanico.Ver`.
2. **Botón "Tareas del Día"**: se agrega el botón (el comando ya existía:
   `ImprimirHojaTrabajoCommand`).
3. **Logo obligatorio**: ambos PDF (`GenerarHistorialPdfAsync` y
   `GenerarHojaTrabajoPdfAsync`) pasan a `ComposeEncabezadoTaller(...)` +
   `ComposePieDePagina()`; el subtítulo dinámico se mueve al contenido.
4. **Logo desde carpeta**: fuente única `EmpresaBranding` que prioriza el override por
   usuario `%LocalAppData%\TP_ControlVehicular\Empresa\logo.png` y cae al asset embebido
   `Presentacion/Assets/logo.png`.

## Estado final (commits)

| # | Commit | Mensaje | Alcance |
|---|---|---|---|
| 0 | `8bee811` | `feat(mistrabajos): scaffolding de PDFs de historial clinico y hoja de trabajo diaria` | Cambios pendientes del usuario (desbloquea Fase B) |
| 1 | `2a74bef` | `feat(reportes): reportes del mecanico con permiso, logo y branding por carpeta` | `EmpresaBranding`, `ReporteExtensions`, VM y XAML del mecánico |
| 2 | `83bc9ee` | `test(rbac): corrige RbacTests para usar UsuarioSesionActual` | `RbacTests.cs` (fallo preexistente) |
| 3 | *(este)* | `docs(reportes): cierra Fase B, branding por carpeta y tests RBAC` | `AGENTS.md`, `MEMORY.md`, spec y walkthrough |

Rama de trabajo: `feature/reportes-rbac-logo`.

## Tareas de la Fase B

### Task 1 — `EmpresaBranding` (fuente única del logo) · `2a74bef`

Nuevo `Negocio/Reportes/Documentos/EmpresaBranding.cs`:

| Miembro | Valor |
|---|---|
| `CarpetaEmpresa` | `%LocalAppData%\TP_ControlVehicular\Empresa` |
| `RutaLogoOverride` | `CarpetaEmpresa\logo.png` (subido por el taller) |
| `RutaLogoCompartido` | `Presentacion/Assets/logo.png` (asset embebido, copiado al output) |
| `RutaLogo` | `RutaLogoOverride` si `File.Exists`, si no `RutaLogoCompartido` |
| `AsegurarCarpeta()` | `Directory.CreateDirectory(CarpetaEmpresa)` perezoso |

La carpeta la crea la **app** en el primer uso (es por usuario); el instalador **no** la
crea. Si está vacía, se usa el logo embebido. Un futuro feature *Taller* poblará
`empresa.json` (dirección/teléfono) — spec aparte (`taller-branding`).

### Task 2 — `ReporteExtensions.RutaLogo` delega en `EmpresaBranding` · `2a74bef`

```csharp
public static string RutaLogo => EmpresaBranding.RutaLogo;
```

`ComposeLogo()` (usado por encabezados de reportes y comprobantes) pasa a respetar el
override sin cambios adicionales: todo el sistema resuelve el logo en un único punto.

### Task 3 — Permiso en el ViewModel · `2a74bef`

`Presentacion/ViewModels/MisTrabajosViewModel.cs`:

```csharp
/// <summary>Habilita los reportes del mecanico segun el permiso Reporte.Mecanico.Ver.</summary>
public bool PuedeVerReportesMecanico => TienePermiso("Reporte.Mecanico.Ver");

/// <summary>Historial clinico: requiere el permiso y una tarea seleccionada.</summary>
public bool PuedeImprimirHistorial => PuedeVerReportesMecanico && TareaSeleccionada != null;
```

`TienePermiso(...)` vive en `BaseViewModel` y lee `MainWindow.UsuarioSesionActual.Permisos`.
El setter de `TareaSeleccionada` notifica `PuedeImprimirHistorial`.

### Task 4 — Botón "Tareas del Día" y gating · `2a74bef`

`Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml`:

| Botón | Comando | Visibilidad |
|---|---|---|
| 📋 Tareas del Día (nuevo) | `ImprimirHojaTrabajoCommand` | `PuedeVerReportesMecanico` |
| 🕒 Imprimir Historial Clínico | `ImprimirHistorialCommand` | `PuedeImprimirHistorial` |

Estilo `#2980B9` (Azul, acción), `CornerRadius="6"`, `BoolToVis`.

### Task 5 — Encabezado con logo en ambos PDF · `2a74bef`

| PDF | Antes | Después |
|---|---|---|
| Historial Clínico (`GenerarHistorialPdfAsync`) | `page.Header().Text(...)` → sin logo | `ComposeEncabezadoTaller("Historial Clínico")` + `ComposePieDePagina()` |
| Tareas del Día (`GenerarHojaTrabajoPdfAsync`) | `page.Header().Text(...)` → sin logo | `ComposeEncabezadoTaller("Tareas del Día")` + `ComposePieDePagina()` |

El subtítulo dinámico (`Vehículo: …` / `Mecánico: … - fecha`) se movió al inicio de
`page.Content()`, en `Grey.Darken2`; ya no se usa `page.Header().Text(...)`.

### Task 6 — Verificación

```powershell
dotnet build "TP_ControlVehicular.slnx"   # 0 Errores
dotnet test "TP_ControlVehicular.Tests"   # 6/6 superadas
```

Durante la verificación se detectó un **fallo preexistente** de `RbacTests` (ajeno a la
Fase B): el test fijaba la estática `UserSession.CurrentUser`, pero
`MainWindow.AplicarRestriccionesPorRol` lee `UsuarioSesionActual`; la excepción en el hilo
STA abortaba toda la corrida. Al no poder satisfacerse la Tarea 6 ("todos los tests
pasan") con el test roto, se corrigió el **test** (no la producción) en `83bc9ee`:

```csharp
// AplicarRestriccionesPorRol lee la fuente real de la sesion (UsuarioSesionActual).
typeof(MainWindow).GetProperty("UsuarioSesionActual")!.SetValue(window, UserSession.CurrentUser);
```

### Task 7 — Verificación manual (inspección)

- Botones del mecánico: `Visibility` ligada a `PuedeVerReportesMecanico` / `PuedeImprimirHistorial`.
- Ambos PDF usan `ComposeEncabezadoTaller` + `ComposePieDePagina` (logo).
- Logo: `EmpresaBranding.RutaLogo` prioriza el override y cae al embebido.
- La comprobación **visual interactiva** (abrir la app, generar ambos PDF, probar con/sin
  `logo.png` en la carpeta) queda a cargo del usuario.

### Task 8 — Reglas en `AGENTS.md` / `MEMORY.md` · *(este commit)*

- Regla del logo actualizada: fuente única `EmpresaBranding`, override por usuario
  `%LocalAppData%\TP_ControlVehicular\Empresa\logo.png` con respaldo embebido;
  `ReporteExtensions.RutaLogo` delega en él.
- Nota de la carpeta perezosa (`AsegurarCarpeta`) y del futuro feature de carga del taller.

## Notas

- **D4**: `git add` con rutas explícitas en cada commit; los archivos pendientes del
  usuario se commitearon por separado (`8bee811`) antes de tocar la Fase B.
- Sin commitear (no es alcance): `.agents/`, `docs/script/update_vm_11.py`,
  `docs/script/update_xaml_11.py`.
- `tasks.md` de `reportes-rbac` (Fase A) se actualizó para apuntar su Phase B al spec
  `reportes-rbac-mecanico`.
