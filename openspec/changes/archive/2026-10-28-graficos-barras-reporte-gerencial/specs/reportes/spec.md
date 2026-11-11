# Specification: Managerial Report Bar Charts

## Feature Description
Generate executive bar charts in the Managerial Report PDF exports:
- Vertical bar chart for "Ingresos Totales por Fecha".
- Horizontal bar chart for "Modelos de Vehículos Más Reparados".

## Acceptance Criteria (BDD)

### Scenario 1: Income PDF displays vertical bar chart with daily revenues
- **Given** the user requests the "Ingresos Totales" PDF export with valid data in the filtered date range
- **When** the document is composed
- **Then** a vertical bar chart (SVG) is rendered above the data table
- **And** the chart displays chronological date labels on the X-axis and proportional bar heights representing daily income amounts.

### Scenario 2: Models PDF displays horizontal bar chart with top models
- **Given** the user requests the "Modelos Más Reparados" PDF export with valid repair data
- **When** the document is composed
- **Then** a horizontal bar chart (SVG) is rendered above the data table
- **And** the chart ranks models in descending order by repair frequency with brand/model labels on the Y-axis and repair count values next to each bar.

### Scenario 3: Empty data handling in PDF generation
- **Given** no income or repair records exist for the chosen period
- **When** the user generates either PDF report
- **Then** the system completes generation without error
- **And** displays an appropriate empty state notice or clean placeholder rather than failing or crashing.

### Scenario 4: XML / SVG character safety
- **Given** a vehicle brand or model name contains special characters (such as `&`, `<`, `>`, `"`)
- **When** the SVG is rendered
- **Then** all text labels are properly XML-escaped to ensure the SVG markup is valid.
