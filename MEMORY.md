# TP_ControlVehicular
Aplicación WPF .NET 10 MVVM para control vehicular con EF Core y SQL Server. Capas: Datos, Entidad, Migracion, Negocio, Presentacion.

> `MEMORY.md` es una pista, no la evidencia: el estado real se verifica contra `git log` / `git status`; si discrepan, el repositorio gana y se corrige esta memoria.
> **Fuente canónica: `AGENTS.md`** (Stack, Comandos, Estructura, convenciones completas de UI/formularios/popups, No hagas, Flujo de trabajo, Documentación). Este archivo conserva solo el delta de alta señal; toda regla nueva se replica en ambos.

## Convenciones críticas (resumen; detalle en `AGENTS.md`)
- **Reportes PDF (QuestPDF) — logo obligatorio**: TODO PDF DEBE mostrar el logo. Reportes: `page.Header().Element(c => c.ComposeEncabezadoTaller("Título"))` + `page.Footer().Element(c => c.ComposePieDePagina())`; comprobantes: `.Element(c => c.ComposeLogo())` (`Negocio/Reportes/Documentos/ReporteExtensions.cs`). Prohibido headers privados sin logo o `Header().Text(...)`; subtítulos/filtros al inicio de `Content()`. Logo resuelto solo en `EmpresaBranding` (override `%LocalAppData%\TP_ControlVehicular\Empresa\logo.png`, sino asset `Presentacion/Assets/logo.png`; `AsegurarCarpeta()` lazy).
- **RBAC de reportes** (`Reporte.<Nombre>.Ver`; `Reporte.Ver` obsoleto):

  | Reporte | Permiso | Admin | Recepcionista | Mecánico |
  |---|---|---|:-:|:-:|:-:|
  | Órdenes | `Reporte.Ordenes.Ver` | ✅ | ✅ | ❌ |
  | Operativo | `Reporte.Operativo.Ver` | ✅ | ✅ | ❌ |
  | Gerencial | `Reporte.Gerencial.Ver` | ✅ | ❌ | ❌ |
  | Historial / Tareas del día | `Reporte.Mecanico.Ver` | ✅ | ❌ | ✅ |

  Reporte nuevo → permiso en script SQL idempotente en `bd/` + chequeo RBAC + actualizar esta tabla.
- **No mezclar granularidades**: Reporte de Órdenes = por **orden** (`RegistroServicio`, `ObtenerReporteOrdenesHandler`/`GetReporteCompletoAsync`); Reporte Operativo = por **tarea** (`DetalleServicio` por `UsuarioId`, `sp_ReporteProductividadMecanicos`/`ObtenerProductividadMecanicosAsync`; solo mecánicos `u.RolId = 3`, excluye órdenes `Cancelada`). Estados: orden ∈ {`Abierta`,`En Proceso`,`Completada`,`Pagada`,`Cancelada`}; tarea ∈ {`Pendiente`,`En Curso`,`Finalizada`}. **Una orden nunca vale `Finalizada`**.
- **Prohibido fallback a datos demostrativos** en reportes/dashboard: sin datos o error → colección vacía. Los SP de reportes aliasan cada columna al **nombre exacto de la propiedad del DTO** (mapeo Dapper por nombre).
- **`sp_ReporteIngresos` vs `sp_ReporteIngresosAdmin` — no reusar nombres**: general (3 params `@StartDate,@EndDate,@WorkshopId`; `Fecha/TallerNombre/TotalIngresos` → `ReporteIngresosDto`) vía `ReporteRepository.ObtenerReporteIngresosAsync`; gerencial (2 params `@FechaDesde,@FechaHasta`; `Fecha/CantidadFacturas/IngresosTotales` → `ReporteIngresoDto`) vía `ReporteGerencialRepository.ObtenerIngresosPorFechaAsync`. Ver `bd/16_Fix_Colision_sp_ReporteIngresos.sql`.
- **Cancelación de órdenes**: constante `Entidad.RegistroServicio.EstadoCancelada` (`"Cancelada"`); nunca el literal `"Cancelado"`. `DetalleServicio` no tiene estado `Cancelada`.
- **Artefactos test E2E**: patentes `TDD%` y clientes `EndToEnd` son datos de prueba; `bd/15_Limpieza_Artefactos_TestE2E.sql` los purga idempotente y FK-safe.
- **`ProcesarPagoHandler` es código muerto** (lanza `NotImplementedException`); el cobro real usa `IBillingService.RegistrarPago`.

## No hagas (resumen; detalle en `AGENTS.md`)
- Scripts temporales (.py, .ps1…) → siempre a `docs/script/`.
- No escribir código/XAML con `Set-Content`/`>`/`>>` en PowerShell: usar `[System.IO.File]::WriteAllText(ruta, contenido, [System.Text.Encoding]::UTF8)` (proyecto rigurosamente UTF-8).
- No usar `&&` en PowerShell (usar `;`); rutas con espacios/acentos siempre entre comillas.
- No modificar `AGENTS_Template.md` ni `spec_template/`; no agregar dependencias sin avisar.
- No asumir patrones genéricos: fuentes ejecutables (config, scripts, código) sobre prosa.
- Commits atómicos Conventional Commits; sin secretos en el repo.

## Flujo de trabajo
- **Rama por spec**: `git checkout -b feature/<nombre>`; se trabaja ahí hasta terminar y se fusiona.
- **Uso de `MEMORY.md`**: leerla al iniciar una tarea y contrastarla con `git log -n 10` y `git status`; si discrepan, el repositorio manda. Al cerrar cada spec: actualizar esta memoria con las nuevas convenciones/scripts/permisos/decisiones y commitearla junto con la spec (`docs(memory): ...`). Mantener alineada con `AGENTS.md`.
- **Planes y walkthroughs** → `docs/plan/AAAA-MM-DD_<spec>.md` (y `..._walkthrough.md`), en español, commiteados junto con la spec (`docs(plan): ...`). `openspec/` = especificación formal (inglés); `docs/plan/` = bitácora de ejecución (español).
- Antes de tarea no trivial: proponer plan y esperar confirmación. Una tarea a la vez; si no hay certeza ≥80%, preguntar (no inventar); solo lo pedido; cambios pequeños y enfocados; al terminar, resumir cambios y decisiones a revisar.

## Especificaciones: OpenSpec (SDD) + TDD
- Toda feature se define primero en `openspec/`: `proposal.md` → `spec.md` → `design.md` → `tasks.md`. Las `spec.md` van en **inglés** con Criterios de Aceptación estilo BDD (Given/When/Then). No se escribe implementación sin specs aprobadas.
- **Verificación obligatoria** antes de marcar completada una tarea: `dotnet build "TP_ControlVehicular.slnx"` sin errores + validación del comportamiento (tests cuando aplique).

## Documentación
- `docs/DER.md` — DER a respetar en todo feature; `docs/plan/` — bitácoras; `README.md` — solo placeholders; `spec_template/` y `AGENTS_Template.md` — plantillas de referencia.

## Agent Guidelines
- Usar **CodeGraph** (MCP `codegraph_explore` o CLI `codegraph explore`) antes de grep/lecturas manuales; si no hay `.codegraph/`, omitir.
- Manipulación de archivos vía scripts Python (`docs/script/`) si hace falta para evitar corrupción UTF-8 en PowerShell.
- OpenSpec obligatorio antes de modificar código de implementación; specs en inglés; validar acceptance criteria con build.