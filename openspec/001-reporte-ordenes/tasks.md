# Task Breakdown: Reporting Enhancements

## Phase 2: Implementation

### Task 1: Scaffolding the New Management Report
- [ ] **1.1** Create `ReporteGerencialViewModel.cs` inheriting from `BaseViewModel`.
- [ ] **1.2** Create `ReporteGerencialView.xaml` (UserControl).
- [ ] **1.3** Register both classes in the Dependency Injection container (e.g., `App.xaml.cs`).
- [ ] **1.4** Update the Main Window/Sidebar XAML to include a "Reporte Gerencial" navigation button.
- [ ] **1.5** Wire the navigation button to the navigation service/command to route to `ReporteGerencialView`.

### Task 2: Dashboard Refactoring
- [ ] **2.1** Identify and extract XAML elements belonging to the Management Report from `DashboardView.xaml` and paste them into `ReporteGerencialView.xaml`.
- [ ] **2.2** Identify and extract properties, data fetching, and bindings from `DashboardViewModel.cs` into `ReporteGerencialViewModel.cs`.
- [ ] **2.3** Verify that `DashboardView` continues to function and compile without the removed components.

### Task 3: Work Orders Report (UI & Filtering)
- [ ] **3.1** Open `CtlReporteOrdenes.xaml` and add two `DatePicker` controls (`Desde` and `Hasta`).
- [ ] **3.2** Open `ReporteOrdenesViewModel.cs` and add `FechaDesde` and `FechaHasta` properties (nullable DateTime).
- [ ] **3.3** Update the filtering logic inside the ViewModel to evaluate the newly added date bounds alongside the existing text and status filters.
- [ ] **3.4** Verify the UI updates reactively when date bounds are changed.

### Task 4: Work Orders Report (PDF Export)
- [ ] **4.1** In `CtlReporteOrdenes.xaml`, add an "Imprimir PDF" `Button`.
- [ ] **4.2** In `ReporteOrdenesViewModel.cs`, add an `ImprimirPdfCommand` bound to the new button.
- [ ] **4.3** Implement the PDF generation using QuestPDF inside the command's execute action:
  - Set PageSize to `PageSizes.A4.Landscape()`.
  - Compose a Header showing applied filters (Desde, Hasta, Estado).
  - Compose a Content Table iterating over the *currently filtered* list of Work Orders.
- [ ] **4.4** Save the generated PDF to a temporary system file.
- [ ] **4.5** Automatically open the generated PDF using the default system PDF viewer (`Process.Start`).

### Task 5: QA & Review
- [ ] **5.1** Test navigating between Dashboard, Reporte Gerencial, and Reporte Órdenes without state corruption.
- [ ] **5.2** Verify Date filtering boundaries (inclusive vs exclusive) on the work orders.
- [ ] **5.3** Verify PDF output formats correctly on an A4 Landscape layout without cutting off data.
