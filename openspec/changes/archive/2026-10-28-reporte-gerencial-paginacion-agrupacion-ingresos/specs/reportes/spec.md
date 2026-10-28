# Spec Delta

## ADDED Requirements

### Requirement: Database Pagination for Managerial Total Income Grid
The system SHALL provide database-level pagination for the Managerial Total Income grid. Queries for managerial income SHALL support server-side paging (using SQL Server `OFFSET ... FETCH NEXT ... ROWS ONLY` or equivalent database paging mechanism) to retrieve only the records corresponding to the requested page index and page size, along with the total count of matching records. The user interface SHALL provide navigation controls (First, Previous, Next, Last) and display the current page, total pages, and total record count.

#### Scenario: Initial page load with database pagination
- **WHEN** the user executes the managerial report query
- **THEN** the system retrieves only the first page of income records from the database
- **AND** displays the grid populated with page 1 items and indicates the current page and total page count.

#### Scenario: Navigation between pages
- **WHEN** the user clicks the Next Page button on an active page that is not the last page
- **THEN** the system executes a paginated database query for the next page
- **AND** updates the grid with the corresponding page records and updates the page counter.

#### Scenario: Boundary page controls disabling
- **WHEN** the user is on the first page
- **THEN** the First and Previous navigation controls are disabled
- **AND** when the user is on the last page, the Next and Last navigation controls are disabled.

#### Scenario: Changing page size
- **WHEN** the user changes the page size selection (e.g., from 10 to 20)
- **THEN** the system resets the current page index to 1 and executes a new database query using the updated page size.

### Requirement: Monthly Aggregation of Total Income
The system SHALL support monthly aggregation of total income within the selected date range. Revenue data SHALL be groupable by calendar month (Year-Month), presenting the total number of invoices generated and the total revenue collected for each month. The Managerial Income PDF report SHALL incorporate this monthly grouped summary and chart to provide executive trend visibility across billing months.

#### Scenario: Querying monthly aggregated revenues
- **WHEN** the managerial report is queried over a date range spanning one or more calendar months
- **THEN** the system computes monthly grouped metrics showing Year-Month, invoice count, and total income for each month.

#### Scenario: PDF report includes monthly grouped breakdown
- **WHEN** the user exports the Managerial Income report to PDF
- **THEN** the PDF document includes the monthly aggregated summary table and monthly revenue bar chart above or alongside the detailed period metrics.
