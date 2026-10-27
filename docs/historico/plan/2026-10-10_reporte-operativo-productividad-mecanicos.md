# Plan — Reporte Operativo: productividad de mecánicos a nivel de tarea

Spec formal (inglés): `openspec/changes/reporte-operativo-productividad-mecanicos/`.
Capacidad afectada: `reportes`.

## Objetivo
Corregir la tabla "Productividad por Mecánico" del **Reporte Operativo** (rol Recepcionista):
1. **Aparecen todos los mecánicos con tareas** en el rango elegido (antes solo se veía uno).
2. **"Tareas Completadas" deja de ser siempre 0** y refleja el estado real de cada tarea.
3. Se agregan columnas de desglose (`Pendientes`, `En Curso`, `% Avance`) y un **resumen con totales**.
4. El **PDF** refleja la misma información (columnas + fila de totales).
5. Se **elimina el fallback silencioso de datos demostrativos** de `ObtenerReporteOrdenesHandler`
   (afecta al Reporte de Órdenes): si no hay datos reales, el reporte queda vacío.

## Diagnóstico (evidencia)
| Síntoma | Causa raíz |
|---|---|
| Solo aparece un mecánico | `ObtenerReporteOrdenesHandler` emite **una fila por orden** y toma únicamente el **primer detalle** (`r.Detalles.FirstOrDefault(...)`). Las tareas de un segundo mecánico nunca son el primer detalle → queda invisible. |
| "Tareas Completadas" siempre 0 | El VM comparaba el estado de la **orden** (`Abierta` / `En Proceso` / `Completada` / `Pagada`) contra `"Finalizada"`, que es un estado de **tarea** (`DetalleServicio`). La orden nunca vale `"Finalizada"`. |
| Conteos inflados/erróneos | `TareasAsignadas` contaba **órdenes**, no **tareas**. |

Estados reales:
- `RegistroServicio.Estado`: `Abierta`, `En Proceso`, `Completada`, `Pagada`.
- `DetalleServicio.Estado`: `Pendiente`, `En Curso`, `Finalizada`.

## Decisiones
- La unidad de agregación correcta es **`DetalleServicio` agrupada por `UsuarioId`**, uniendo
  `RegistroServicio` (fecha) y `Usuarios`.
- `TareasCompletadas` = `DetalleServicio.Estado = 'Finalizada'`; `Pendientes` = `'Pendiente'`;
  `EnCurso` = `'En Curso'`.
- **No** se cambia la semántica del Reporte de Órdenes (sigue siendo por orden). El Reporte
  Operativo recibe su **propia fuente** (SP + método de repositorio dedicados).
- El rango de fechas aplica a `RegistroServicio.Fecha` (los detalles no tienen fecha propia).
- `INNER JOIN Usuarios` excluye tareas sin mecánico asignado.
- Sin filtro por estado de orden: paridad con `GetReporteCompletoAsync` (el reporte de órdenes
  no filtra estado).
- Se listan **solo** mecánicos con tareas en el rango (no se rellenan con ceros).
- Nombre completo `Nombre + ' ' + Apellido`. El SP aliasa **cada columna al nombre exacto de la
  propiedad del DTO** (`MecanicoNombre`, `TareasCompletadas`, `EnCurso`, …) para el mapeo
  automático por nombre de Dapper.
- Sin permiso nuevo: la pantalla ya la gobierna `Reporte.Operativo.Ver`.

## Cambios
| Archivo | Cambio |
|---|---|
| `bd/13_SP_ReporteProductividadMecanicos.sql` | Nuevo SP idempotente `sp_ReporteProductividadMecanicos` (`CREATE OR ALTER`). |
| `Negocio/DTOs/Reportes/ProductividadMecanicoDto.cs` | Nuevo DTO top-level (con `PorcentajeAvance` / `PorcentajeAvanceTexto`). |
| `Negocio/Reportes/IReporteGerencialRepository.cs` | Nuevo método `ObtenerProductividadMecanicosAsync`. |
| `Datos/Repositories/ReporteGerencialRepository.cs` | Implementación Dapper + SP. |
| `Negocio/Services/ObtenerReporteOrdenesHandler.cs` | Se elimina `GetReportesDemostrativos()` y el fallback; vacío ante sin datos/error. |
| `Presentacion/ViewModels/ReporteOperativoViewModel.cs` | Quita dependencia de `ObtenerReporteOrdenesHandler`; usa el nuevo método; totales; PDF con desglose. |
| `Presentacion/Pantalla/Reporte/CtlReporteOperativo.xaml` | Columnas `Pendientes` / `En Curso` / `% Avance` y tarjeta de resumen. |

## Verificación
- `dotnet build "TP_ControlVehicular.slnx"` → 0 errores.
- `dotnet test "TP_ControlVehicular.Tests"` → 6/6.
- `bd/13_...sql` aplicado contra `ControlVehicular` (`sqlcmd`).
- Smoke test del SP: coincide con la agregación cruda; filtro de fechas vacío en rango futuro.
- `openspec validate reporte-operativo-productividad-mecanicos` (normal y `--strict`).

## Notas
- Convención `bd/`: `USE [ControlVehicular]; GO`, `CREATE OR ALTER`, idempotente.
- Nombres de tabla confirmados por EF: `RegistroServicio` y `DetalleServicio` (singular, vía
  `ToTable`) y `Usuarios`.
- `sp_ServiciosPorMecanico` (ya existente) es **distinto** y no se reutiliza.
