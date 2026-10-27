# Design: Management and Historical Reports

## Architecture & Data Flow
The implementation follows a CQRS pattern using MediatR, Entity Framework Core for data access, and WPF for the UI.

### 1. Data Access (Entity Framework Core)
- EF Core does not map Stored Procedures to standard entities automatically for complex types. We will use `SqlQueryRaw` (or `FromSqlRaw` for keyless entities) to map the output of the stored procedures to Data Transfer Objects (DTOs).
- **Target Stored Procedures**: `sp_ReporteIngresos`, `sp_ReporteTiemposResolucion`.

### 2. DTOs
- `RevenueReportDto`: Represents the result set of `sp_ReporteIngresos` (e.g., Date, WorkshopName, TotalRevenue).
- `ResolutionTimeReportDto`: Represents the result set of `sp_ReporteTiemposResolucion` (e.g., WorkshopName, AverageResolutionTimeDays).

### 3. Application Layer (MediatR Handlers)
- `GetRevenueReportQuery` & `GetRevenueReportQueryHandler`: Accepts `StartDate`, `EndDate`, and optional `WorkshopId`. Executes `_dbContext.Database.SqlQueryRaw<RevenueReportDto>(...)`.
- `GetResolutionTimeReportQuery` & `GetResolutionTimeReportQueryHandler`: Accepts the same parameters and executes `_dbContext.Database.SqlQueryRaw<ResolutionTimeReportDto>(...)`.

### 4. Presentation Layer (WPF)
- **UI Component**: `CtlDashboard.xaml` (UserControl).
- **Controls**: 
  - `dtpStartDate` and `dtpEndDate` (DatePicker)
  - `cmbWorkshops` (ComboBox)
  - `dgrRevenue` and `dgrResolutionTimes` (DataGrid or Charting components depending on existing libraries).
- **Role-Based Logic**: 
  - In the code-behind (`CtlDashboard.xaml.cs`) or ViewModel, check the current user's role.
  - If `Role != "Administrator"`, set `cmbWorkshops.Visibility = Visibility.Collapsed` (or `IsEnabled = false`) and force the `WorkshopId` parameter in queries to the user's `WorkshopId`.
