# Referencia — Reportes

Conocimiento de dominio de la capa de reportes. Se enlaza desde `AGENTS.md` → *Referencia por dominio*.
Leer solo cuando la tarea toque reportes, PDFs, permisos de reportes o los SP asociados.

## Reportes PDF (QuestPDF) — logo obligatorio
- TODO PDF generado por el sistema (reportes, hojas de trabajo, comprobantes) DEBE mostrar el logo de la empresa en el encabezado.
- Reportes: usar siempre `page.Header().Element(c => c.ComposeEncabezadoTaller("Título"))` y `page.Footer().Element(c => c.ComposePieDePagina())` de `Negocio/Reportes/Documentos/ReporteExtensions.cs`.
- Documentos con encabezado propio (comprobantes): incluir el logo con `.Element(c => c.ComposeLogo())`.
- Prohibido crear `ComposeHeader`/`ComposeFooter` privados que no muestren el logo, o usar `page.Header().Text(...)`. Los subtítulos y filtros van al inicio de `page.Content()`.
- El logo se resuelve en un único punto: `EmpresaBranding` (`Negocio/Reportes/Documentos/EmpresaBranding.cs`). Busca primero el override por usuario `%LocalAppData%\TP_ControlVehicular\Empresa\logo.png`; si no existe, usa el asset embebido `Presentacion/Assets/logo.png` (Content, copiado al output). `ReporteExtensions.RutaLogo` delega en él; no leer rutas de logo por fuera ni duplicar la imagen.
- `EmpresaBranding.AsegurarCarpeta()` crea la carpeta override de forma perezosa en el primer uso (por usuario; no la crea el instalador). Un futuro feature "carga de logo/dirección/teléfono del taller" poblará esa carpeta (`empresa.json`).

## Visibilidad de reportes por rol (RBAC)
Cada reporte tiene su propio permiso `Reporte.<Nombre>.Ver`. El permiso genérico `Reporte.Ver` está obsoleto y no debe usarse.

| Reporte | Permiso | Admin | Recepcionista | Mecánico |
|---|---|---|:-:|:-:|:-:|
| Reporte Órdenes | `Reporte.Ordenes.Ver` | ✅ | ✅ | ❌ |
| Reporte Operativo | `Reporte.Operativo.Ver` | ✅ | ✅ | ❌ |
| Reporte Gerencial | `Reporte.Gerencial.Ver` | ✅ | ❌ | ❌ |
| Historial clínico / Tareas del día (en Mis Trabajos) | `Reporte.Mecanico.Ver` | ✅ | ❌ | ✅ |

Un reporte nuevo requiere: permiso nuevo en un script SQL idempotente en `bd/`, chequeo en `MainWindow.AplicarRestriccionesPorRol` (o en el VM si el botón vive dentro de una pantalla) y actualizar esta tabla.

## Fuente de datos — no mezclar granularidades
Cada reporte define su unidad de agregación y usa una fuente dedicada.
- **Reporte de Órdenes** → por **orden** (`RegistroServicio`, vía `ObtenerReporteOrdenesHandler` / `GetReporteCompletoAsync`).
- **Reporte Operativo** → por **tarea** (`DetalleServicio` agrupada por `UsuarioId`, vía `sp_ReporteProductividadMecanicos` / `IReporteGerencialRepository.ObtenerProductividadMecanicosAsync`). El SP de productividad cuenta **solo** tareas de mecánicos (`u.RolId = 3`) y **excluye órdenes canceladas** (`rs.Estado <> 'Cancelada'`).
- Estados: `RegistroServicio.Estado` ∈ {`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`}; `DetalleServicio.Estado` ∈ {`Pendiente`, `En Curso`, `Finalizada`} (no existe `Cancelada` a nivel detalle). **Una orden nunca vale `Finalizada`** — la completitud de tareas se mide en `DetalleServicio`. No contar órdenes donde corresponden tareas.

## Prohibido el fallback a datos demostrativos
Los handlers/repositorios de reportes NO deben inventar filas de demostración cuando no hay datos o hay error. `ObtenerReporteOrdenesHandler` fue limpiado (devuelve vacío). Si un reporte no tiene datos, se muestra vacío.
- Los SP de reportes aliasan cada columna al **nombre exacto de la propiedad del DTO** (ej. `MecanicoNombre`, `TareasCompletadas`, `EnCurso`) para el mapeo automático por nombre de Dapper.

## `sp_ReporteIngresos` — dos variantes con consumidores distintos (no reusar el nombre)
- `sp_ReporteIngresos` (3 params `@StartDate, @EndDate, @WorkshopId`; columnas `Fecha`/`TallerNombre`/`TotalIngresos` → DTO `ReporteIngresosDto`) alimenta el reporte general vía `ReporteRepository.ObtenerReporteIngresosAsync`.
- `sp_ReporteIngresosAdmin` (2 params `@FechaDesde, @FechaHasta`; columnas `Fecha`/`CantidadFacturas`/`IngresosTotales` → DTO `ReporteIngresoDto`) alimenta el reporte gerencial vía `ReporteGerencialRepository.ObtenerIngresosPorFechaAsync`.
- Compartir nombre rompe al consumidor pisado (ver `bd/16_Fix_Colision_sp_ReporteIngresos.sql`).

## Reporte Gerencial — cantidad de servicios por rango
- `sp_ReporteServiciosPorRango` (`bd/18`; 2 params `@FechaDesde, @FechaHasta`) agrega a nivel **tarea** (`DetalleServicio`): `SUM(Cantidad)` por servicio (`ServicioId`/`Servicio`/`Cantidad` → DTO `ReporteServicioCantidadDto`), excluye órdenes canceladas (`rs.Estado <> 'Cancelada'`); vía `ReporteGerencialRepository.ObtenerServiciosPorRangoAsync`.
- Alimenta el 4.º tab "Servicios por Rango" y el PDF homónimo.
- El **gráfico de torta va solo en el PDF** (nunca en la vista WPF): `ServiciosPieChartGenerator` (`Negocio/Reportes/Documentos/`) produce un **SVG** (`Paleta`, `Normalizar` top-N + "Otros", `BuildSvg`) y la leyenda se compone con elementos nativos QuestPDF — QuestPDF no expone `Canvas`; usar `QuestPDF.Fluent.SvgExtensions.Svg(IContainer, string)`.
- `ReporteModeloReparadoDto` expone `MarcaModelo` compuesto (no `Marca`/`Modelo` por separado). La vista muerta `ReporteGerencialView` fue eliminada; el reporte vive en `CtlReporteGerencial`.
