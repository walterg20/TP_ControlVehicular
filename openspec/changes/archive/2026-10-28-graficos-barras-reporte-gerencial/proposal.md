# Proposal: Bar Charts for Total Income and Top Repaired Models in Managerial Report PDFs

## Problem Statement
The Managerial Report (`ReporteGerencial`) currently includes a pie chart only in the "Servicios por Rango" tab PDF export. The PDF exports for "Ingresos Totales" and "Modelos Más Reparados" only display tabular data, making it difficult for management to quickly visualize income trends and identify the most frequently repaired vehicle models at a glance.

## Proposed Solution
Introduce clean SVG-based bar chart generators in `TP_ControlVehicular.Negocio.Reportes.Documentos`:
1. `IngresosBarChartGenerator`: Vertical bar chart (columns) showing chronological daily income progression.
2. `ModelosBarChartGenerator`: Horizontal bar chart ranking top repaired vehicle models (Brand + Model) by service count.

Both charts will be rendered directly inside the corresponding QuestPDF documents via `.Svg(...)` before the detailed data tables, without requiring external chart dependencies and preserving fast rendering performance.

## Scope
- In Scope:
  - SVG generation for vertical income bar chart and horizontal top models bar chart.
  - Integration with QuestPDF in `ReporteGerencialViewModel.cs`.
  - Comprehensive unit testing for both chart generators.
- Out of Scope:
  - On-screen WPF interactive charting (maintaining consistency with the existing pie chart pattern).
  - Schema or stored procedure changes.
