# Proposal: Management Report Separation & Work Orders Enhancements

## 1. Objective
Refactor the application's reporting capabilities by moving the "Reporte Gerencial" (Management Report) from the Dashboard to a **new independent menu option** under the "REPORTES" section. Concurrently, enhance the existing "Reporte Órdenes de Trabajo" (`CtlReporteOrdenes`) by adding Date-range filters (Desde/Hasta) and a PDF export functionality (A4 Landscape) using QuestPDF. 

## 2. Motivation
Currently, the Management Report clutters the Dashboard's high-level overview. Moving it to its own dedicated section will improve the application's organization and user experience. 
Furthermore, the existing "Reporte Órdenes de Trabajo" lacks date filtering and export capabilities. Users need the ability to constrain work orders to a specific time period (in addition to existing text and status filters) and generate physical or shareable PDF documents natively within the WPF application.

## 3. Proposed Changes
1. **Refactor Dashboard (`DashboardView` / `DashboardViewModel`)**: 
   * Remove the UI elements (XAML) and backend logic associated with the management report.
2. **New Menu Item - "Reporte Gerencial"**: 
   * Create a new WPF `UserControl` (e.g., `ReporteGerencialView.xaml`) and a corresponding ViewModel inheriting from `BaseViewModel`.
   * Add this new view to the lateral menu under the REPORTS section.
   * Transfer the management report logic and data binding here.
3. **Enhance `CtlReporteOrdenes`**:
   * Add two WPF `DatePicker` controls for "Desde" (From) and "Hasta" (To) into the existing XAML filter panel.
   * Update the ViewModel filtering logic to evaluate the new date ranges alongside the existing text and status filters.
   * Add a "Print PDF" (Imprimir PDF) button bound to an `ICommand`.
4. **PDF Generation (QuestPDF)**:
   * Implement PDF export using the existing `QuestPDF` dependency.
   * Configure the document layout to A4 format with Landscape orientation.
   * Include a document header, applied filter summary, and the filtered data grid.
