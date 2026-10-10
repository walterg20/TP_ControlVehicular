# Walkthrough — Reporte Operativo: productividad de mecánicos a nivel de tarea

Bitácora de ejecución. Plan: [`2026-10-10_reporte-operativo-productividad-mecanicos.md`](./2026-10-10_reporte-operativo-productividad-mecanicos.md).
Spec: `openspec/changes/reporte-operativo-productividad-mecanicos/`.

## Rama
`feature/reporte-operativo-productividad-mecanicos`.

## Pasos ejecutados

### 1. Capa de datos
- Se creó `bd/13_SP_ReporteProductividadMecanicos.sql` con
  `CREATE OR ALTER PROCEDURE sp_ReporteProductividadMecanicos @FechaDesde DATE = NULL, @FechaHasta DATE = NULL`,
  agregando `DetalleServicio` por `UsuarioId`:
  ```sql
  SELECT
      u.Id AS MecanicoId,
      u.Nombre + ' ' + u.Apellido AS MecanicoNombre,
      COUNT(*) AS TareasAsignadas,
      SUM(CASE WHEN ds.Estado = 'Pendiente'  THEN 1 ELSE 0 END) AS Pendientes,
      SUM(CASE WHEN ds.Estado = 'En Curso'   THEN 1 ELSE 0 END) AS EnCurso,
      SUM(CASE WHEN ds.Estado = 'Finalizada' THEN 1 ELSE 0 END) AS TareasCompletadas
  FROM DetalleServicio ds
  INNER JOIN RegistroServicio rs ON ds.RegistroServicioId = rs.Id
  INNER JOIN Usuarios u ON ds.UsuarioId = u.Id
  WHERE (@FechaDesde IS NULL OR CAST(rs.Fecha AS DATE) >= @FechaDesde)
    AND (@FechaHasta IS NULL OR CAST(rs.Fecha AS DATE) <= @FechaHasta)
  GROUP BY u.Id, u.Nombre, u.Apellido;
  ```
- `Negocio/DTOs/Reportes/ProductividadMecanicoDto.cs`: DTO top-level con propiedades
  `MecanicoId`, `MecanicoNombre`, `TareasAsignadas`, `Pendientes`, `EnCurso`, `TareasCompletadas`
  y las calculadas `PorcentajeAvance` / `PorcentajeAvanceTexto`.
- `IReporteGerencialRepository` + `ReporteGerencialRepository`: nuevo
  `ObtenerProductividadMecanicosAsync(DateTime?, DateTime?)` (Dapper, `CommandType.StoredProcedure`),
  espejo de `ObtenerModelosMasReparadosAsync`.

### 2. Limpieza del Reporte de Órdenes (punto 7)
- `Negocio/Services/ObtenerReporteOrdenesHandler.cs`: se eliminaron `GetReportesDemostrativos()`
  y su invocación. Ahora, si no hay registros o hay error, devuelve `new List<OrdenTrabajoReporteDto>()`
  (con un `try/catch` para no romper la UI). No se fabrican filas.

### 3. ViewModel
- `Presentacion/ViewModels/ReporteOperativoViewModel.cs`:
  - El constructor ahora recibe **solo** `IReporteGerencialRepository`.
  - `Productividad` se llena desde `ObtenerProductividadMecanicosAsync(FechaDesde, FechaHasta)`.
  - Se eliminó la clase anidada `ProductividadMecanicoDto` (ahora vive en `Negocio/DTOs/Reportes`).
  - Nuevas propiedades de totales: `TotalTareas`, `TotalPendientes`, `TotalEnCurso`,
    `TotalTareasCompletadas`, `PorcentajeAvanceGeneral`, `PorcentajeAvanceGeneralTexto`,
    con `ActualizarTotalesProductividad()` y `NotificarPorcentajeGeneral()`.
  - Se eliminó la propiedad/duplicado `IsLoading` (usa la de `BaseViewModel`).
  - `ComposeContent` (PDF): 6 columnas + fila de totales; se conservan
    `ComposeEncabezadoTaller` / `ComposePieDePagina` / `ObtenerTextoFiltros`.

### 4. UI (XAML)
- `Presentacion/Pantalla/Reporte/CtlReporteOperativo.xaml`: panel de productividad más ancho
  (`3*` vs `2*`), tarjeta de resumen (`Total` / `Pendientes` / `En curso` / `Completadas` / `Avance`)
  y columnas `Pendientes`, `En Curso`, `% Avance` en el `DataGrid`.

### 5. Verificación
- `dotnet build "TP_ControlVehicular.slnx"` → **0 errores** (65 warnings preexistentes).
- `dotnet test "TP_ControlVehicular.Tests"` → **6/6**.
- Aplicado el SP:
  `sqlcmd -S "WALTERG20\SQLEXPRESS2019" -d ControlVehicular -E -C -i "bd\13_SP_ReporteProductividadMecanicos.sql"`.
- Smoke test del SP vs agregación cruda (coinciden):
  ```
  MecanicoId | MecanicoNombre   | TareasAsignadas | Pendientes | EnCurso | TareasCompletadas
  2          | Pedro Canoero    | 13              | 0          | 1       | 12
  3          | Juan Cruz        | 3               | 0          | 0       | 3
  6          | Sergio Mecanico  | 4               | 0          | 0       | 4
  ```
  Filtro amplio (2020→2030) devuelve lo mismo; filtro futuro (2030→2031) devuelve vacío.
- `openspec validate reporte-operativo-productividad-mecanicos` (normal y `--strict`).

## Resultado
- Todos los mecánicos con tareas aparecen (antes solo uno).
- "Tareas Completadas" refleja `DetalleServicio.Estado = 'Finalizada'` (antes siempre 0).
- Total = 20 tareas, Completadas = 19, En curso = 1 → `% Avance` = 95 % (consistente fila a fila).
- El Reporte de Órdenes ya no inventa filas cuando no hay datos.
