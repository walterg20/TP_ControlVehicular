# Reportes por rol + logo obligatorio + flujo MEMORY.md

## Objetivo
1. **Visibilidad de reportes por rol**: hoy un solo permiso `Reporte.Ver` habilita los 3 botones del menú (Órdenes, Gerencial, Operativo). Se reemplaza por **un permiso por reporte**, para que cada rol vea solo lo que le corresponde (el Admin, todo).
2. **Regla del logo**: todo PDF que genere el sistema tiene que usar el encabezado con el logo de la empresa (`ComposeEncabezadoTaller`). La regla se escribe en `AGENTS.md`/`MEMORY.md` y se aplica a los reportes que hoy no la cumplen.
3. **Flujo MEMORY.md**: se agrega a `AGENTS.md` cuándo y cómo se actualiza `MEMORY.md`.

## Estado actual (evidencia)
| Reporte / PDF | Dónde | ¿Usa logo? | ¿Quién lo ve hoy? |
|---|---|---|---|
| Reporte Órdenes | `ReporteOrdenesViewModel` (header propio) | ❌ | Admin, Recepcionista |
| Reporte Operativo | `ReporteOperativoViewModel` (header propio) | ❌ | Admin, Recepcionista |
| Reporte Gerencial (ingresos, top clientes, modelos) | `ReporteGerencialViewModel` | ✅ `ComposeEncabezadoTaller` | Admin, **Recepcionista** ⚠️ |
| Historial clínico / Tareas del día | `MisTrabajosViewModel` (sin commitear) | ❌ (`page.Header().Text(...)`) | Mecánico (vía Mis Trabajos) |
| Comprobante Recepción / Pago | `Comprobante*Document` | ❌ (texto del taller) | desde Órdenes |

## Matriz propuesta de visibilidad

| Reporte | Permiso nuevo | Admin | Recepcionista | Mecánico |
|---|---|:-:|:-:|:-:|
| 📋 Reporte Órdenes | `Reporte.Ordenes.Ver` | ✅ | ✅ | ❌ |
| 📊 Reporte Operativo | `Reporte.Operativo.Ver` | ✅ | ✅ | ❌ |
| 💰 Reporte Gerencial | `Reporte.Gerencial.Ver` | ✅ | ❌ | ❌ |
| 🔧 Historial clínico + Tareas del día | `Reporte.Mecanico.Ver` | ✅ | ❌ | ✅ |

> [!NOTE]
> Matriz **confirmada** por el usuario.

## Decisiones tomadas
| # | Tema | Decisión |
|---|---|---|
| D1 | Matriz de visibilidad | Admin: todos · Recepcionista: Órdenes y Operativo · Mecánico: Historial y Tareas del día |
| D2 | Reportes del mecánico | Siguen como botones en "Mis Trabajos"; la sección REPORTES del menú le queda oculta |
| D3 | Logo en comprobantes | Sí: se agrega el logo y se mantienen los datos del taller que ya muestran |
| D4 | Cambios sin commitear (`CtlMisTrabajos.xaml`, `MisTrabajosViewModel.cs`, `bd/09_...sql`) | **No se tocan**: los revisa el usuario |
| D5 | `AGENTS.md` | Se agrega al repo en el commit de documentación |

> [!WARNING]
> **Consecuencia de D4:** los cambios en `CtlMisTrabajos.xaml` y `MisTrabajosViewModel.cs` (botones con permiso `Reporte.Mecanico.Ver` y logo en Historial/Tareas del día) quedan **bloqueados**. Se hacen como **Fase B**, después de que commitees tus cambios.
> - **Fase A (ahora):** OpenSpec, script SQL, menú, logo en Órdenes/Operativo/Comprobantes, reglas en AGENTS/MEMORY y `docs/plan`.
> - Creo la rama `feature/reportes-rbac-logo` sin hacer stash ni commit de tus 3 archivos: siguen modificados en tu working tree y **nunca los incluyo** en mis commits (`git add` solo con rutas explícitas).
> - **Fase B (cuando me avises):** gating y logo en los PDF del mecánico.

---

## Cambios propuestos

### 0. OpenSpec (antes de tocar código, en inglés)
#### [NEW] `openspec/specs/reportes-rbac/` → `proposal.md`, `spec.md`, `design.md`, `tasks.md`
Escenarios BDD, por ejemplo:
```gherkin
Scenario: Receptionist does not see the management report
  Given a user with role "Recepcionista" is logged in
  When the sidebar menu is rendered
  Then "Reporte Órdenes" and "Reporte Operativo" are visible
  And "Reporte Gerencial" is collapsed

Scenario: Every generated PDF shows the company logo
  Given any report PDF is generated
  Then its page header is composed with ComposeEncabezadoTaller
```

---

### 1. Base de datos
#### [NEW] `bd/12_PermisosReportesPorRol.sql`
Script idempotente (mismo estilo que el script 10):
```sql
-- 1. Nuevos permisos
INSERT INTO Permisos (Nombre)
SELECT v.Nombre FROM (VALUES ('Reporte.Ordenes.Ver'),('Reporte.Operativo.Ver'),
                             ('Reporte.Gerencial.Ver'),('Reporte.Mecanico.Ver')) v(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Permisos p WHERE p.Nombre = v.Nombre);

-- 2. Admin (1): todos
INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 1, IdPermiso FROM Permisos
WHERE Nombre LIKE 'Reporte.%.Ver'
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 1 AND rp.IdPermiso = Permisos.IdPermiso);

-- 3. Recepcionista (2): Ordenes + Operativo; se quita el genérico Reporte.Ver
DELETE rp FROM RolPermisos rp JOIN Permisos p ON p.IdPermiso = rp.IdPermiso
WHERE rp.IdRol = 2 AND p.Nombre = 'Reporte.Ver';
INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 2, IdPermiso FROM Permisos
WHERE Nombre IN ('Reporte.Ordenes.Ver','Reporte.Operativo.Ver')
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 2 AND rp.IdPermiso = Permisos.IdPermiso);

-- 4. Mecánico (3): Reporte.Mecanico.Ver
INSERT INTO RolPermisos (IdRol, IdPermiso)
SELECT 3, IdPermiso FROM Permisos
WHERE Nombre = 'Reporte.Mecanico.Ver'
  AND NOT EXISTS (SELECT 1 FROM RolPermisos rp WHERE rp.IdRol = 3 AND rp.IdPermiso = Permisos.IdPermiso);
```
`Reporte.Ver` no se borra de la tabla `Permisos`, para no romper datos existentes. Solo deja de usarse en la UI.

---

### 2. Presentación – menú
#### [MODIFY] [MainWindow.xaml.cs](file:///e:/UNNE/Taller%20de%20Programación%20II/Proyecto/TP_ControlVehicular/Presentacion/MainWindow.xaml.cs#L137-L141)
```diff
-            bool puedeVerReportes = permisos.Contains("Reporte.Ver");
-            secReportes.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
-            btnReporteOrdenes.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
-            btnReporteGerencial.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
-            btnReporteOperativo.Visibility = puedeVerReportes ? Visibility.Visible : Visibility.Collapsed;
+            bool verOrdenes    = permisos.Contains("Reporte.Ordenes.Ver");
+            bool verOperativo  = permisos.Contains("Reporte.Operativo.Ver");
+            bool verGerencial  = permisos.Contains("Reporte.Gerencial.Ver");
+            btnReporteOrdenes.Visibility   = verOrdenes   ? Visibility.Visible : Visibility.Collapsed;
+            btnReporteOperativo.Visibility = verOperativo ? Visibility.Visible : Visibility.Collapsed;
+            btnReporteGerencial.Visibility = verGerencial ? Visibility.Visible : Visibility.Collapsed;
+            bool algunReporte = verOrdenes || verOperativo || verGerencial;
+            secReportes.Visibility  = algunReporte ? Visibility.Visible : Visibility.Collapsed;
+            sepReportes.Visibility  = algunReporte ? Visibility.Visible : Visibility.Collapsed;
```
#### [MODIFY] `CtlMisTrabajos.xaml` / `MisTrabajosViewModel.cs`
Propiedad `PuedeVerReportes => UserSession.CurrentUser?.Permisos.Contains("Reporte.Mecanico.Ver")`, enlazada a la `Visibility` de los botones "Historial Clínico" y "Tareas del Día" (con el `BooleanToVisibilityConverter` que ya existe).

---

### 3. Logo obligatorio en los PDF
#### [MODIFY] `ReporteOrdenesViewModel.cs`, `ReporteOperativoViewModel.cs`
Reemplazar `page.Header().Element(ComposeHeader)` por `page.Header().Element(c => c.ComposeEncabezadoTaller("…título…"))` y borrar el `ComposeHeader` privado.
#### [MODIFY] `MisTrabajosViewModel.cs`
`page.Header().Text(...)` → `ComposeEncabezadoTaller("Historial Clínico")` / `ComposeEncabezadoTaller("Tareas del Día")`. El subtítulo (vehículo, mecánico, fecha) pasa al inicio de `page.Content()`.
#### [MODIFY] `Comprobante*Document.cs` *(solo si P2 = sí)*
Se agrega la imagen del logo a la izquierda del header. Los datos dinámicos del taller no se tocan.

---

### 4. Reglas y documentación
#### [MODIFY] `AGENTS.md` y `MEMORY.md`, nueva sección en *Convenciones*
```markdown
- **Reportes PDF (QuestPDF)**:
  - TODO PDF generado por el sistema DEBE usar el encabezado con el logo de la empresa:
    `page.Header().Element(c => c.ComposeEncabezadoTaller("Título"))` (Negocio/Reportes/Documentos/ReporteExtensions.cs).
  - Pie de página: `page.Footer().Element(c => c.ComposePieDePagina())`.
  - Prohibido crear `ComposeHeader` propios o usar `page.Header().Text(...)`.
  - El logo es `Presentacion/Assets/logo.png` (Content, copiado al output). No duplicarlo.
- **Visibilidad de reportes por rol**: cada reporte tiene su propio permiso `Reporte.<Nombre>.Ver`.
  Matriz vigente: Admin = todos; Recepcionista = Órdenes, Operativo; Mecánico = Historial/Tareas del Día.
  Un reporte nuevo requiere un permiso nuevo en script SQL + chequeo en `AplicarRestriccionesPorRol`.
```
#### [MODIFY] `AGENTS.md` y `MEMORY.md`, sección *Flujo de trabajo*
```markdown
- **Uso de MEMORY.md**:
  - Al INICIAR una tarea: leer `MEMORY.md` como pista y contrastarlo con `git log -n 10` / `git status`.
    Si discrepan, el repositorio gana y se corrige `MEMORY.md`.
  - Al CERRAR una spec (antes del merge): actualizar `MEMORY.md` con nuevas convenciones,
    reglas, scripts SQL agregados, permisos y decisiones tomadas, y commitearlo junto con la spec
    (`docs(memory): ...`).
  - `MEMORY.md` debe mantenerse alineado con `AGENTS.md`: toda regla nueva en uno se replica en el otro.
- **Planes de implementación (.md)**:
  - Todo plan, walkthrough o documento de trabajo en Markdown generado durante el desarrollo
    DEBE guardarse en `docs/plan/` dentro del repo (no solo en carpetas externas del agente).
  - Nombre: `AAAA-MM-DD_<nombre-spec>.md` (ej: `2026-10-10_reportes-rbac-logo.md`); walkthrough: `..._walkthrough.md`.
  - Se commitea junto con la spec correspondiente (`docs(plan): ...`).
  - Diferencia con OpenSpec: `openspec/` = especificación formal (inglés); `docs/plan/` = plan/bitácora de ejecución (español).
```
Además, como primer paso de la ejecución, se copia **este mismo plan** a `docs/plan/2026-10-10_reportes-rbac-logo.md` (y al final su walkthrough).
Además se sincroniza `MEMORY.md` con lo que hoy tiene `AGENTS.md` y le falta (scripts en `docs/script`, regla UTF-8 con `[System.IO.File]::WriteAllText`, ramas por spec, CodeGraph, `docs/DER.md`).

> [!NOTE]
> `AGENTS.md` aparece como *untracked* en git. Propongo incluirlo en el commit de documentación. Avísame si lo mantenías fuera del repo a propósito.

---

## Commits previstos (Conventional Commits)
1. `docs(openspec): spec reportes-rbac`
2. `feat(bd): permisos granulares por reporte (script 12)`
3. `feat(ui): visibilidad de reportes por permiso`
4. `refactor(reportes): encabezado con logo unificado en todos los PDF`
5. `docs(agents): regla de logo, matriz de reportes, flujo MEMORY.md y carpeta docs/plan`
6. `docs(plan): plan y walkthrough de reportes-rbac-logo`

## Plan de verificación
### Automatizado
```powershell
dotnet build "TP_ControlVehicular.slnx"
dotnet test "TP_ControlVehicular.Tests"   # si sigue compilando el proyecto de tests existente
```
### Manual
1. Ejecutar `bd/12_PermisosReportesPorRol.sql` en `ControlVehicular`.
2. Entrar como **Admin**: se ven los 3 reportes en el menú y los botones de reportes en Mis Trabajos.
3. Entrar como **Recepcionista**: se ven Órdenes y Operativo; el Gerencial no aparece.
4. Entrar como **Mecánico**: la sección REPORTES no aparece, y en Mis Trabajos están Historial y Tareas del Día.
5. Generar cada PDF (Órdenes, Operativo, los 3 gerenciales, Historial, Tareas del Día y los comprobantes si P2 = sí) y verificar que todos tengan el logo y el encabezado "TALLER PRO".
