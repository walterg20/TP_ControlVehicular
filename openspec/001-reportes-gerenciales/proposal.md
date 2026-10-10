# Proposal: Management and Historical Reports

## The Problem
Currently, Administrators do not have a centralized, high-level view of the organization's performance. There is a lack of aggregated data regarding revenue and the average resolution times for vehicle controls across different workshops, making it difficult to make informed, data-driven decisions.

## The Solution
Introduce a Dashboard for Administrators that features two key reports:
1. **Revenue Report**: Displays income generated over a specific period, filterable by Date and Workshop.
2. **Resolution Time Report**: Shows the average time taken to complete vehicle controls, also filterable by Date and Workshop.

By leveraging existing Stored Procedures (`sp_ReporteIngresos` and `sp_ReporteTiemposResolucion`), we can efficiently present this historical data. Role-based access control will ensure that non-administrator users can only view data relevant to their assigned workshop.
