# Technical Design: Reporting Enhancements

## 1. Architectural Overview
This feature follows the existing WPF MVVM pattern:
- **Views** are XAML files containing UI definitions.
- **ViewModels** inherit from `BaseViewModel` and encapsulate presentation logic, state, and `ICommand` actions.
- **Dependency Injection (DI)** manages View and ViewModel lifecycles, configuring bindings.
- **QuestPDF** is used for document generation due to its fluent API and high performance.

## 2. Component Design

### 2.1 Management Report (Reporte Gerencial)
**Goal:** Extract the management report from the Dashboard into a standalone module.

*   **`ReporteGerencialViewModel.cs`**:
    *   Inherits from `BaseViewModel`.
    *   Contains properties for management metrics (e.g., `TotalRevenue`, `OrdersCompleted`, `ActiveUsers`, chart data collections).
    *   Contains the asynchronous data-fetching logic migrated from `DashboardViewModel`.
*   **`ReporteGerencialView.xaml`**:
    *   A new `UserControl`.
    *   Hosts the charting and grid components previously residing in `DashboardView.xaml`.
*   **Dependency Injection Configuration**:
    *   In the service registration (likely `App.xaml.cs` or a `ServiceExtensions.cs` class), add:
        `services.AddTransient<ReporteGerencialViewModel>();`
        `services.AddTransient<ReporteGerencialView>();`
*   **Navigation / Sidebar**:
    *   Add a new menu item bound to a navigation command directing to `ReporteGerencialView`.

### 2.2 Dashboard Refactoring
*   **`DashboardViewModel.cs`**:
    *   Remove all properties, commands, and service calls related strictly to the management report.
*   **`DashboardView.xaml`**:
    *   Remove the corresponding XAML nodes, freeing up visual space.

### 2.3 Work Orders Report (`CtlReporteOrdenes`)
**Goal:** Add Date filters and PDF export.

*   **`ReporteOrdenesViewModel.cs` Modifications**:
    *   **New Properties**:
        *   `private DateTime? _fechaDesde; public DateTime? FechaDesde { get... set... }`
        *   `private DateTime? _fechaHasta; public DateTime? FechaHasta { get... set... }`
    *   **Filter Logic Update**: Update the `ApplyFilters()` or equivalent CollectionView filtering method.
        *   *Condition:* `(!FechaDesde.HasValue || order.Date >= FechaDesde.Value) && (!FechaHasta.HasValue || order.Date <= FechaHasta.Value)` combined with existing text/status matches.
    *   **Commands**:
        *   `public ICommand ImprimirPdfCommand { get; }`
        *   Initialized as an asynchronous relay command triggering `GeneratePdfAsync()`.

*   **`CtlReporteOrdenes.xaml` Modifications**:
    *   Modify the Filters grid/panel:
        *   Add a `TextBlock` and `DatePicker` bound to `FechaDesde`.
        *   Add a `TextBlock` and `DatePicker` bound to `FechaHasta`.
    *   Modify the Actions panel:
        *   Add `<Button Content="Imprimir PDF" Command="{Binding ImprimirPdfCommand}" />`

### 2.4 PDF Generation (QuestPDF)
*   **Implementation (`GeneratePdfAsync`)**:
    *   Ensure `QuestPDF.Settings.License = LicenseType.Community;` is initialized globally.
    *   Construct the document using Fluent API:
        ```csharp
        var document = Document.Create(container => 
        {
            container.Page(page => 
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));
                
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent); // Passes the currently filtered list
                page.Footer().Element(ComposeFooter);
            });
        });
        ```
    *   **Output Handling**: Save to a temporary file path or prompt the user with a `SaveFileDialog`. Given the requirement is a direct "Print PDF" action, saving to a `Path.GetTempFileName() + ".pdf"` and calling `Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true })` will seamlessly open the PDF for the user to print or save.
