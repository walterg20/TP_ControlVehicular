# Design

## Context
See `proposal.md` for background and motivation.
Currently, `ReporteGerencialViewModel` loads all daily revenue rows at once via `sp_ReporteIngresosAdmin` (`IReporteGerencialRepository.ObtenerIngresosPorFechaAsync`). When filtered over months or an entire year, dozens of rows are loaded into an in-memory `ObservableCollection<ReporteIngresoDto>`. There is no server-side pagination, and the PDF export only renders a flat daily table.

## Goals / Non-Goals

**Goals:**
- Implement database-level pagination using SQL Server `OFFSET ... FETCH NEXT ... ROWS ONLY` for daily income records.
- Provide a clean, accessible pagination bar in `CtlReporteGerencial.xaml` adhering to project UI conventions (dark headers, standard button colors, border radius, responsive state).
- Provide monthly aggregation of revenues in both database procedures and C# DTOs.
- Enhance the Managerial Income PDF export to include a monthly summary breakdown table alongside period totals.

**Non-Goals:**
- Paginating other tabs in the Managerial Report (Top Clientes and Modelos Más Reparados already use `@TopN = 10`, and Servicios por Rango is bounded by the 15 official services).
- Changing the authorization or RBAC model (Managerial Report remains restricted to Administrator via `Reporte.Gerencial.Ver`).

## Decisions

### 1. Database Paging via CTE with Count Window vs Separate Count Query
- **Decision:** Use a single CTE query with `CROSS JOIN` on `COUNT(*)` in `sp_ReporteIngresosAdminPaginado`:
  ```sql
  WITH Base AS (
      SELECT
          CAST(f.Fecha AS DATE) AS Fecha,
          COUNT(f.Id) AS CantidadFacturas,
          SUM(f.Total) AS IngresosTotales
      FROM Factura f
      WHERE (@FechaDesde IS NULL OR CAST(f.Fecha AS DATE) >= @FechaDesde)
        AND (@FechaHasta IS NULL OR CAST(f.Fecha AS DATE) <= @FechaHasta)
      GROUP BY CAST(f.Fecha AS DATE)
  ),
  Contado AS (
      SELECT COUNT(*) AS TotalRegistros FROM Base
  )
  SELECT 
      b.Fecha,
      b.CantidadFacturas,
      b.IngresosTotales,
      ISNULL(c.TotalRegistros, 0) AS TotalRegistros
  FROM Base b
  CROSS JOIN Contado c
  ORDER BY b.Fecha DESC
  OFFSET (@Pagina - 1) * @TamanoPagina ROWS
  FETCH NEXT @TamanoPagina ROWS ONLY;
  ```
- **Rationale:** Executes in a single database roundtrip, preventing discrepancies between count and data under concurrent invoice registrations, and works reliably with Dapper.
- **Alternative considered:** Two separate SQL queries. Rejected due to extra network latency and potential race conditions.

### 2. Dedicated Monthly Stored Procedure (`sp_ReporteIngresosMensualesAdmin`)
- **Decision:** Create `sp_ReporteIngresosMensualesAdmin` returning `PeriodoMes` (`VARCHAR(7)` e.g. `'2026-06'`), `FechaMes` (`DATE`), `CantidadFacturas` (`INT`), and `IngresosTotales` (`DECIMAL(10,2)`).
- **Rationale:** Keeps stored procedures focused and optimized. Grouping by `FORMAT(Fecha, 'yyyy-MM')` / `YEAR` and `MONTH` is clean and fast.
- **Alternative considered:** Grouping daily records in C# LINQ. Rejected because server-side grouping in SQL is significantly more efficient and allows direct reporting regardless of pagination.

### 3. Repository Contract & Pagination Tuple
- **Decision:** Expose:
  - `Task<(IEnumerable<ReporteIngresoDto> Items, int TotalRegistros)> ObtenerIngresosPorFechaPaginadoAsync(DateTime? fechaDesde, DateTime? fechaHasta, int pagina, int tamanoPagina);`
  - `Task<IEnumerable<ReporteIngresoMensualDto>> ObtenerIngresosMensualesAsync(DateTime? fechaDesde, DateTime? fechaHasta);`
- **Rationale:** Value tuples in C# avoid unnecessary wrapper classes while maintaining clear type signatures. Existing `ObtenerIngresosPorFechaAsync` is preserved for backward compatibility (e.g. Dashboard tile).

### 4. WPF Pagination UI in `CtlReporteGerencial.xaml`
- **Decision:** Add a pagination toolbar below the `DataGrid` in Tab 1 ("Ingresos Totales"):
  - Navigation buttons: `|◀` (First), `◀` (Previous), `▶` (Next), `▶|` (Last).
  - Status text: `Página {PaginaActual} de {TotalPaginas} ({TotalRegistros} registros)`.
  - Page size ComboBox: options `[5, 10, 20, 50]`, default `10`.
  - Monthly summary toggle/expander or card displaying the monthly grouped breakdown.

### 5. QuestPDF Export Enhancement
- **Decision:** Update `ComposeIngresosContent` in `ReporteGerencialViewModel` to render:
  1. Header with Period Revenue Total.
  2. Monthly Grouped Breakdown table (Year-Month, Invoices, Monthly Revenue).
  3. Bar Chart (Progression).
  4. Detailed daily records.
- **Rationale:** Gives management executive high-level insight (monthly) as well as operational detail (daily).

## Risks / Trade-offs

- **[Risk]** User sets `TamanoPagina` or `Pagina` out of bounds.
  - **Mitigation:** Clamp `Pagina` between `1` and `Math.Max(1, TotalPaginas)` in ViewModel, and provide minimum check `@Pagina = CASE WHEN @Pagina < 1 THEN 1 ELSE @Pagina END` in stored procedure.
- **[Risk]** Empty result set returns 0 rows and NULL `TotalRegistros`.
  - **Mitigation:** Stored procedure returns `0` as `TotalRegistros` when `Base` is empty, and ViewModel handles 0 records gracefully.
