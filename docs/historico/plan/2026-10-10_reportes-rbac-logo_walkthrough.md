# Walkthrough — Reportes RBAC + Logo (Fase A)

Bitácora de ejecución de la Fase A del plan [`2026-10-10_reportes-rbac-logo.md`](./2026-10-10_reportes-rbac-logo.md).

> [!NOTE]
> `openspec/` = especificación formal (inglés). `docs/plan/` = plan y bitácora de ejecución (español).

## Resumen

La Fase A reemplaza el permiso genérico `Reporte.Ver` por **un permiso por reporte**,
aplica la **regla del logo** a todos los PDF (reportes y comprobantes) y documenta el
**flujo de `MEMORY.md`** y la carpeta `docs/plan/` en `AGENTS.md` / `MEMORY.md`.

La Fase B (gating y logo en los PDF del mecánico, botón "Tareas del Día" y fuente del
logo por carpeta) queda **bloqueada** hasta que el usuario commitee sus cambios
pendientes en `CtlMisTrabajos.xaml` y `MisTrabajosViewModel.cs`.

## Estado final (commits)

| # | Commit | Mensaje | Alcance |
|---|---|---|---|
| 1 | `178d7ce` | `docs(openspec): spec reportes-rbac y plan en docs/plan` | Spec + plan |
| 2 | `22fe4e8` | `feat(bd): permisos granulares por reporte (script 12)` | `bd/12_PermisosReportesPorRol.sql` |
| 3 | `56c10a2` | `feat(ui): visibilidad de reportes por permiso granular` | `MainWindow.AplicarRestriccionesPorRol` |
| 4 | `83147fa` | `refactor(reportes): encabezado con logo unificado en PDFs y comprobantes` | Reportes + comprobantes |
| 5 | `c4466af` | `docs(agents): regla de logo, matriz de reportes, flujo MEMORY.md y carpeta docs/plan` | `AGENTS.md` + `MEMORY.md` |
| — | `d946a21` | `fix(bd): unifica USE [ControlVehicular] en 09_PermisosAvanzados` | Corrección previa de `bd/09` |

Rama de trabajo: `feature/reportes-rbac-logo`.

## Tareas de la Fase A

### Task 1 — Script de permisos (`bd/12_PermisosReportesPorRol.sql`) · `22fe4e8`
Script idempotente que:
- inserta los 4 permisos nuevos (`Reporte.Ordenes.Ver`, `Reporte.Operativo.Ver`,
  `Reporte.Gerencial.Ver`, `Reporte.Mecanico.Ver`);
- asigna **todos** a Admin (rol 1);
- asigna Órdenes + Operativo a Recepcionista (rol 2) y quita el genérico `Reporte.Ver`;
- asigna `Reporte.Mecanico.Ver` a Mecánico (rol 3).

`Reporte.Ver` **no** se borra de la tabla `Permisos` (no rompe datos existentes); solo
deja de usarse en la UI.

### Task 2 — Visibilidad granular en el menú · `56c10a2`
`MainWindow.AplicarRestriccionesPorRol` dejó de usar el permiso único y ahora evalúa
cada reporte por separado:

```csharp
bool verOrdenes   = permisos.Contains("Reporte.Ordenes.Ver");
bool verOperativo = permisos.Contains("Reporte.Operativo.Ver");
bool verGerencial = permisos.Contains("Reporte.Gerencial.Ver");
btnReporteOrdenes.Visibility   = verOrdenes   ? Visibility.Visible : Visibility.Collapsed;
btnReporteOperativo.Visibility = verOperativo ? Visibility.Visible : Visibility.Collapsed;
btnReporteGerencial.Visibility = verGerencial ? Visibility.Visible : Visibility.Collapsed;
bool algunReporte = verOrdenes || verOperativo || verGerencial;
secReportes.Visibility = algunReporte ? Visibility.Visible : Visibility.Collapsed;
sepReportes.Visibility = algunReporte ? Visibility.Visible : Visibility.Collapsed;
```

La sección **REPORTES** se colapsa si el rol no tiene ningún reporte (caso Mecánico).

### Task 3 y 4 — Logo obligatorio en los PDF · `83147fa`
Reemplazo del encabezado propio por `ComposeEncabezadoTaller("…")` y uso del pie
`ComposePieDePagina()`:

| PDF | Antes | Después |
|---|---|---|
| Reporte Órdenes (`ReporteOrdenesViewModel.cs:156`) | `ComposeHeader` propio | `ComposeEncabezadoTaller("Reporte de Órdenes de Trabajo")` |
| Reporte Operativo (`ReporteOperativoViewModel.cs:110`) | `ComposeHeader` propio | `ComposeEncabezadoTaller("Reporte Operativo de Taller")` |
| Reporte Gerencial (`ReporteGerencialViewModel.cs:96/152/208`) | ya cumplía | se mantiene `ComposeEncabezadoTaller` (3 subreportes) |
| Comprobante Recepción (`ComprobanteRecepcionDocument.cs`) | sin logo | `ComposeHeader` incluye `ComposeLogo()` |
| Comprobante Pago (`ComprobantePagoDocument.cs:41`) | sin logo | `ComposeHeader` incluye `ComposeLogo()` |

Los datos dinámicos del taller en los comprobantes **no** se tocaron (decisión D3).

### Task 5 — Reglas en `AGENTS.md` / `MEMORY.md` · `c4466af`
- Nueva sección de **Reportes PDF (QuestPDF) — logo obligatorio** y **Visibilidad de
  reportes por rol (RBAC)** con la matriz vigente.
- **Flujo de trabajo**: reglas de *Uso de MEMORY.md* y *Planes de implementación (.md)*.
- `MEMORY.md` regenerado como superconjunto alineado con `AGENTS.md`.
- Capa `Migracion/` agregada a la estructura; corregida la línea obsoleta de Tests;
  `docs/plan/` agregado a Documentación.

### Task 6 — Verificación

```powershell
dotnet build "TP_ControlVehicular.slnx"   # 0 Errores (34 warnings preexistentes)
dotnet test "TP_ControlVehicular.Tests"   # 6/6 superadas
```

Verificación manual de estructura (evidencia en el repo):
- `MainWindow.xaml.cs` usa los permisos granulares.
- `ReporteOrdenesViewModel` / `ReporteOperativoViewModel` usan `ComposeEncabezadoTaller`.
- `ComprobantePagoDocument` / `ComprobanteRecepcionDocument` incluyen `ComposeLogo()`.

## Pendiente — Fase B (bloqueada)

Se hará como spec aparte (`reportes-rbac-mecanico`) cuando el usuario commitee sus
cambios pendientes. Incluye:

- [ ] **Task 7**: gating de los botones de reportes del mecánico con `Reporte.Mecanico.Ver`.
- [ ] **Task 8**: `ComposeEncabezadoTaller` en los PDF de Historial clínico y Tareas del Día.
- [ ] **Task 9**: botón "Tareas del Día" + fuente del logo por carpeta
      (`%LocalAppData%\TP_ControlVehicular\Empresa\logo.png` con respaldo en
      `Presentacion/Assets/logo.png`; punto único `EmpresaBranding`), anticipando un
      futuro feature de carga de logo/dirección/teléfono del taller.

## Notas

- **D4**: los archivos sin commitear del usuario
  (`Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml`,
  `Presentacion/ViewModels/MisTrabajosViewModel.cs`) **no** se tocaron ni se incluyeron
  en ningún commit (`git add` solo con rutas explícitas).
- `MisTrabajosViewModel.cs:292/347` sigue con `page.Header().Text(...)` (sin logo):
  es alcance de Fase B, no de Fase A.
- Scripts de apoyo sin commitear: `docs/script/update_vm_11.py`,
  `docs/script/update_xaml_11.py`; directorio de herramientas `.agents/`.
