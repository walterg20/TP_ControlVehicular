# Plan de Ejecución — Paginación en BD y Agrupación Mensual en Reporte Gerencial

**Fecha:** 2026-10-28  
**Rama:** `feature/reporte-gerencial-paginacion-agrupacion-ingresos`  
**Spec OpenSpec:** `reporte-gerencial-paginacion-agrupacion-ingresos`  
**Objetivo:** Implementar la paginación a nivel de base de datos (`OFFSET ... FETCH NEXT ... ROWS ONLY`) para la grilla de Ingresos Totales en el Reporte Gerencial y la agrupación mensual de ingresos (con resumen ejecutivo y exportación PDF agrupada por mes).

---

## 1. Contexto y Justificación

Con la reciente población del catálogo y los datos históricos (más de 60 órdenes de servicio y 50 facturas desde Junio de 2026), la consulta de ingresos totales en el Reporte Gerencial devolvía todas las fechas en una única lista en memoria. Para mejorar el rendimiento, escalabilidad y legibilidad para la gerencia, se incorporan dos capacidades:
1. **Paginación desde la base de datos** para la grilla diaria de ingresos.
2. **Agrupación mensual de ingresos** (Año-Mes, Cantidad de Facturas, Ingresos Totales del mes) tanto en la consulta como en el PDF gerencial.

---

## 2. Decisiones Técnicas y Arquitectura

### 2.1 Base de Datos (`bd/03_procedimientos.sql`)
1. **`sp_ReporteIngresosAdminPaginado`**:
   - Parámetros: `@FechaDesde DATE = NULL`, `@FechaHasta DATE = NULL`, `@Pagina INT = 1`, `@TamanoPagina INT = 10`.
   - Consulta CTE que calcula en un único viaje a la base de datos el total de registros coincidentes (`TotalRegistros`) y realiza `OFFSET (@Pagina - 1) * @TamanoPagina ROWS FETCH NEXT @TamanoPagina ROWS ONLY`.
2. **`sp_ReporteIngresosMensualesAdmin`**:
   - Parámetros: `@FechaDesde DATE = NULL`, `@FechaHasta DATE = NULL`.
   - Agrupa por `FORMAT(f.Fecha, 'yyyy-MM')` y `DATEFROMPARTS(YEAR(f.Fecha), MONTH(f.Fecha), 1)`, calculando cantidad de facturas y monto total recaudado por mes en orden cronológico.

### 2.2 Capa de Negocio y Datos
1. **DTOs (`Negocio/DTOs/Reportes/`)**:
   - `ReporteIngresoMensualDto`: contiene `PeriodoMes` (ej: "2026-06"), `FechaMes`, `CantidadFacturas`, `IngresosTotales`.
   - `ReporteIngresoPaginadoDto` (o tupla de retorno `(IEnumerable<ReporteIngresoDto> Items, int TotalRegistros)`).
2. **Repositorio (`IReporteGerencialRepository`, `ReporteGerencialRepository`)**:
   - `Task<(IEnumerable<ReporteIngresoDto> Items, int TotalRegistros)> ObtenerIngresosPorFechaPaginadoAsync(DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina);`
   - `Task<IEnumerable<ReporteIngresoMensualDto>> ObtenerIngresosMensualesAsync(DateTime? fechaDesde, DateTime? fechaHasta);`
   - Se mantiene `ObtenerIngresosPorFechaAsync` sin alterar para retrocompatibilidad con el Dashboard.

### 2.3 ViewModel (`ReporteGerencialViewModel.cs`)
- Propiedades:
  - `PaginaActual` (int, default 1)
  - `TotalPaginas` (int, calculado: `Math.Ceiling(TotalRegistros / TamanoPagina)`)
  - `TotalRegistros` (int)
  - `TamanoPagina` (int, default 10)
  - `OpcionesTamanoPagina` (5, 10, 20, 50)
  - `IngresosMensuales` (`ObservableCollection<ReporteIngresoMensualDto>`)
- Comandos:
  - `IrPrimeraPaginaCommand`
  - `PaginaAnteriorCommand`
  - `PaginaSiguienteCommand`
  - `IrUltimaPaginaCommand`
  - `CambiarTamanoPaginaCommand`

### 2.4 Interfaz de Usuario (`CtlReporteGerencial.xaml`)
- Barra de paginación debajo del DataGrid con botones estilizados según las reglas del proyecto (`CornerRadius="6"`, sombras suaves, estado habilitado/deshabilitado dinámico).
- Selector de tamaño de página (5, 10, 20, 50 registros por página).
- Tarjeta de desglose mensual o selector para ver los ingresos agregados por mes.

### 2.5 Reporte en PDF
- `ComposeIngresosContent` incorpora la tabla resumen mensual previa al detalle diario, permitiendo a la administración observar rápidamente la evolución mes a mes.

---

## 3. Criterios de Aceptación (BDD)

- **Dado** un rango de fechas con múltiples facturas, **cuando** el usuario consulta el reporte gerencial, **entonces** la grilla solo solicita a la base de datos la página activa (ej. 10 filas) y muestra el contador exacto de páginas y registros.
- **Dado** que el usuario se encuentra en la página 1, **cuando** visualiza los controles de navegación, **entonces** los botones *Primero* y *Anterior* permanecen deshabilitados.
- **Dado** que se cambia el tamaño de página a 20, **cuando** se actualiza, **entonces** se resetea a la página 1 y se recalculan las páginas totales.
- **Dado** que se exporta el reporte a PDF, **cuando** se genera el documento, **entonces** se visualiza la tabla agregada por mes con totales y facturas emitidas por cada período.
