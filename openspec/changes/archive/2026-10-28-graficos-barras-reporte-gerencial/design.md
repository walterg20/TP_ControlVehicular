# Design: Bar Charts in Managerial Report PDFs

## Context & Architecture
Following the architecture of `ServiciosPieChartGenerator` and the rules in `docs/referencia/reportes.md`:
- Charts are drawn only in PDF exports via QuestPDF `.Svg(...)` extension.
- No third-party UI chart library is added.
- Pure C# classes construct clean SVG strings using `StringBuilder`.
- Culture-invariant formatting (`CultureInfo.InvariantCulture`) ensures cross-locale decimal point consistency.

## Technical Design

### 1. `IngresosBarChartGenerator`
- **Location:** `TP_ControlVehicular.Negocio.Reportes.Documentos.IngresosBarChartGenerator`
- **Method:** `public static string BuildSvg(IReadOnlyList<ReporteIngresoDto> datos, double width = 500, double height = 190)`
- **Layout:**
  - Background: clean white canvas with subtle border/fill.
  - Margins: Left (55px for monetary Y-axis values), Bottom (35px for dates), Top (20px), Right (20px).
  - Y-Axis: 3 to 4 horizontal gridlines with formatted amounts.
  - Bars: Vertical bars `<rect>` with fill `#2980B9`, rounded corners (`rx="3"`), proportional to `IngresosTotales`.
  - Labels: Date labels under each column formatted as `dd/MM`. Amount labels over the bars when item count <= 15.

### 2. `ModelosBarChartGenerator`
- **Location:** `TP_ControlVehicular.Negocio.Reportes.Documentos.ModelosBarChartGenerator`
- **Method:** `public static string BuildSvg(IReadOnlyList<ReporteModeloReparadoDto> datos, double width = 500)`
- **Layout:**
  - Height: dynamically sized based on item count (`Math.Max(160, datos.Count * 26 + 35)`).
  - Left Margin: 160px for model text (`MarcaModelo`).
  - Bars: Horizontal `<rect>` with fill `#3498DB` (top item `#2980B9` or `#27AE60`), rounded corners.
  - Labels: XML-escaped `MarcaModelo` text on the Y-axis. Repair count text adjacent to the right edge of each bar.

### 3. QuestPDF Integration in `ReporteGerencialViewModel`
- `ComposeIngresosContent`:
  - When `Ingresos` has valid positive data, insert `column.Item().PaddingVertical(10).Svg(IngresosBarChartGenerator.BuildSvg(Ingresos.ToList()));` prior to the table.
- `ComposeModelosContent`:
  - When `ModelosReparados` has valid positive data, insert `column.Item().PaddingVertical(10).Svg(ModelosBarChartGenerator.BuildSvg(ModelosReparados.ToList()));` prior to the table.
