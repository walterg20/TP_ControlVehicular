# Specification: Reporting Enhancements (WPF/MVVM)

## 1. UI Changes (XAML)

### 1.1 `DashboardView.xaml`
*   **Remove**: The layout elements (Grids, Charts, DataGrids) that correspond to the Management Report.

### 1.2 Sidebar Menu
*   **Add**: A new navigation item labeled "Reporte Gerencial" nested under the "REPORTES" category.

### 1.3 New View: `ReporteGerencialView.xaml`
*   **Layout**: Recreate/migrate the Management Report elements previously located in the dashboard.
*   **Context**: Bound to a new `ReporteGerencialViewModel`.

### 1.4 Enhance Existing View: `CtlReporteOrdenes.xaml`
*   **Filters Panel**: 
    *   Add a `DatePicker` labeled "Desde" (Date From).
    *   Add a `DatePicker` labeled "Hasta" (Date To).
    *   Maintain the existing Status ComboBox and Search TextBox.
*   **Action Panel**: 
    *   Add a `Button` labeled "Imprimir PDF", binding its `Command` property to a new `ImprimirPdfCommand` in the ViewModel.

## 2. Functional Requirements

### 2.1 ViewModel Filtering (`ReporteOrdenesViewModel`)
*   Add bound properties `FechaDesde` (DateTime?) and `FechaHasta` (DateTime?).
*   The collection view source or data fetching logic must apply a composite filter:
    *   `OrderDate >= FechaDesde` (if set)
    *   `OrderDate <= FechaHasta` (if set)
    *   `Status == SelectedStatus` (if set)
    *   `Text` matches search query (as currently implemented).

### 2.2 PDF Export using QuestPDF
*   **Trigger**: Invoking `ImprimirPdfCommand`.
*   **Page Setup**:
    *   Size: A4 (`PageSizes.A4`)
    *   Orientation: Landscape (`Page().Size(PageSizes.A4.Landscape())`).
*   **Content Layout**:
    *   **Header**: Title ("Reporte de Órdenes de Trabajo") and generation timestamp.
    *   **Filter Summary**: Display applied filters (e.g., `Desde: [Date] | Hasta: [Date] | Estado: [State]`).
    *   **Data Table**: Render a QuestPDF `Table` reflecting the currently visible/filtered rows in the UI grid. Adjust column widths proportionally for Landscape.
    *   **Footer**: Page number component (`x of y`).

## 3. Technical Implementation Details
*   **Architecture**: WPF (.NET 10) leveraging the MVVM pattern.
*   **ViewModels**: Must inherit from `BaseViewModel` (as per `AGENTS.md` guidelines). Use robust `ICommand` (e.g., `RelayCommand` or equivalent from the utilized MVVM toolkit) implementations for UI actions.
*   **Dependency Injection (DI)**: Register `ReporteGerencialView` and `ReporteGerencialViewModel` in the DI container (likely within `App.xaml.cs` or a dedicated `Program.cs` / `ServiceCollection` extension).
*   **QuestPDF**:
    *   Requires `QuestPDF.Settings.License = LicenseType.Community;` (Ensure this is already present in `App.xaml.cs` or configure appropriately).
    *   Export the PDF asynchronously to avoid blocking the UI thread (`await Task.Run(() => document.GeneratePdf("..."))`).
    *   Automatically open the generated PDF document post-creation using standard OS process invocation (`Process.Start`).
