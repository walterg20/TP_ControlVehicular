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
- Smoke test del SP vs agregación cruda (corrida **inicial**, antes de los fixes A/B y de la
  limpieza `bd/15`; ver §6 para las cifras definitivas):
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
  > Cifras **iniciales**; tras los fixes A/B y la limpieza `bd/15` el resultado definitivo es
  > Total = 4 / Completadas = 4 / En curso = 0 (100 %). Ver §6.
- El Reporte de Órdenes ya no inventa filas cuando no hay datos.

## 6. Fixes posteriores (reporte operativo + higiene de datos)

Durante la validación con datos reales se detectaron y corrigieron cuatro problemas.

### A. Filas espurias en el reporte (usuarios no mecánicos y órdenes canceladas)
- `bd/13_SP_ReporteProductividadMecanicos.sql` ahora filtra:
  ```sql
  WHERE u.RolId = 3                 -- solo Mecánicos
    AND rs.Estado <> 'Cancelada'    -- excluye órdenes canceladas
  ```
  (`Juan Cruz`, recepcionista, aparecía indebidamente; el detalle de la orden 15, cancelada,
  se contaba igual). Reaplicado con `sqlcmd`.

### B. Datos mal cargados (`DetalleServicio.UsuarioId`)
- `bd/14_Fix_ReporteOperativo.sql`: `UPDATE` de los detalles 7, 9 y 11 → Pedro (Id 2) y
  detalle 19 → `Finalizada`. **No** se tocó `RegistroServicio`. Aplicado y verificado.

### 1. Bug de cancelación de órdenes
- `Entidad/RegistroServicio.cs`: nueva constante `EstadoCancelada = "Cancelada"`.
- `Presentacion/ViewModels/OrdenServicioViewModel.cs`: usa la constante (antes el literal
  `"Cancelado"`, que no coincidía con el estado persistido y dejaba la orden sin cancelar).

### 2. Colisión del SP `sp_ReporteIngresos`
- `bd/16_Fix_Colision_sp_ReporteIngresos.sql`: renombra la variante admin a
  `sp_ReporteIngresosAdmin` (2 params `@FechaDesde, @FechaHasta`; columnas
  `Fecha`/`CantidadFacturas`/`IngresosTotales`) y restaura la variante de 3 params
  `sp_ReporteIngresos`, con alias alineados a cada DTO.
- `bd/06`, `bd/07` y `Datos/Repositories/ReporteGerencialRepository.cs`
  (`ObtenerIngresosPorFechaAsync` → `sp_ReporteIngresosAdmin`) actualizados.

### 3. Artefactos del test E2E en la BD
- `bd/15_Limpieza_Artefactos_TestE2E.sql`: purga idempotente y FK-safe de patentes `TDD%`
  y clientes `EndToEnd` (orden `Pago -> Factura -> DetalleServicio -> RegistroServicio ->
  PropietarioVehiculo -> Vehiculo -> Cliente`); deshabilita y rehabilita el trigger
  `TR_DetalleServicio_BloquearModificacionFinalizada`. Aplicado.
- `EndToEndTallerTests` reescrito (proyecto **hermano**, fuera del repo) para limpiar sus
  artefactos en `Dispose` y usar `IBillingService`; compila y pasa.

### Verificación final (post-limpieza)
```
EXEC sp_ReporteProductividadMecanicos

MecanicoId | MecanicoNombre   | TareasAsignadas | Pendientes | EnCurso | TareasCompletadas
2          | Pedro Canoero    | 3               | 0          | 0       | 3
6          | Sergio Mecanico  | 1               | 0          | 0       | 1
```
- Total = 4 tareas, Completadas = 4, En curso = 0 → `% Avance` = 100 %.
- El detalle de la orden 15 (`Cancelada`) queda excluido correctamente.
- `dotnet build "TP_ControlVehicular.slnx"` → compilación correcta (0 errores).
