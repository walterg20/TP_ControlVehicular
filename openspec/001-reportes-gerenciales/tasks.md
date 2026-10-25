# Implementation Tasks

1. **Define DTOs**
   - Create `RevenueReportDto.cs` matching the columns returned by `sp_ReporteIngresos`.
   - Create `ResolutionTimeReportDto.cs` matching the columns returned by `sp_ReporteTiemposResolucion`.
   - *Done when*: Classes exist with appropriate properties to hold the reports' row data.

2. **Implement Queries and Handlers**
   - Create `GetRevenueReportQuery.cs` and its MediatR handler.
   - Create `GetResolutionTimeReportQuery.cs` and its MediatR handler.
   - *Done when*: Both handlers successfully call their respective Stored Procedures using EF Core `SqlQueryRaw` or `FromSqlRaw` and return lists of DTOs.

3. **Develop Dashboard UI (`CtlDashboard.xaml`)**
   - Add DatePicker controls (`dtpStartDate`, `dtpEndDate`).
   - Add ComboBox control for workshops (`cmbWorkshops`).
   - Add display areas (e.g., DataGrids `dgrRevenue`, `dgrResolutionTimes`) for the reports.
   - *Done when*: The XAML compiles and visually contains the required layout with Hungarian notation.

4. **Implement UI Logic and Role-Based Access (`CtlDashboard.xaml.cs`)**
   - Wire up the filters to dispatch MediatR queries and update the DataGrids on 'Search'.
   - Add logic to check the current user's role.
   - If the user is an Administrator, bind and show `cmbWorkshops`.
   - If the user is NOT an Administrator, hide `cmbWorkshops` and hardcode the `WorkshopId` filter to the user's assigned workshop.
   - *Done when*: The dashboard loads data correctly from the SPs, and the workshop filter behaves according to the user's role.
