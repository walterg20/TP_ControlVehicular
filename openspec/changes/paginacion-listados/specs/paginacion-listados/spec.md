# Spec Delta

## ADDED Requirements

### Requirement: Reusable Pagination Toolbar
The system SHALL provide a single reusable pagination control (`CtlPaginacion`) used by every listing screen. The control SHALL expose navigation actions (First, Previous, Next, Last), a page-size selector, a status text showing the current page, the total page count and the total record count. The control SHALL receive its state through a single bound model implementing the `IPaginacion` contract, and SHALL be visually consistent with the project conventions (light card, dark navigation buttons, rounded corners).

#### Scenario: Toolbar rendered on a listing screen
- **WHEN** a listing screen is rendered
- **THEN** the pagination toolbar is displayed below the grid
- **AND** it shows the page-size selector, the page status text and the four navigation buttons.

#### Scenario: Navigation controls disabled at the boundaries
- **WHEN** the user is on the first page
- **THEN** the First and Previous navigation controls are disabled
- **AND** when the user is on the last page, the Next and Last navigation controls are disabled.

#### Scenario: Changing the page size
- **WHEN** the user changes the page size selection
- **THEN** the current page index is reset to 1
- **AND** the listing is reloaded with the new page size.

### Requirement: Shared Pagination ViewModel Base
The system SHALL provide a shared ViewModel base (`BasePaginableViewModel`) that owns the pagination state (current page, total pages, total records, page size, page-size options, status text, navigation command availability) and the navigation commands. Implementations SHALL be able to supply the records for the requested page through a single hook, and the base SHALL clamp any requested page index into the valid range and normalise the total page count to at least one.

#### Scenario: Page index is clamped
- **WHEN** a page index below 1 or above the total page count is requested
- **THEN** the listing is loaded with the nearest valid page index.

#### Scenario: Total page count with no records
- **WHEN** the query returns no records
- **THEN** the total page count is 1 and the total record count is 0.

#### Scenario: Filter change resets the page
- **WHEN** the user changes a filter (search text, status, date range or visibility of inactive records)
- **THEN** the current page index is reset to 1 before the listing is reloaded.

### Requirement: Database-Level Pagination for Listings
Every main listing SHALL retrieve its records from the database using server-side pagination (SQL Server `OFFSET ... FETCH NEXT ... ROWS ONLY` or an equivalent database paging mechanism), returning only the records of the requested page together with the total count of matching records. The system SHALL NOT load the complete result set into memory to serve a paginated grid.

#### Scenario: Only the requested page is retrieved
- **WHEN** a listing is loaded with a page index and a page size
- **THEN** the database returns only the records of that page
- **AND** the grid is populated with those records.

#### Scenario: Total record count reflects the filters
- **WHEN** a listing is loaded with active filters
- **THEN** the reported total record count is the number of records matching those filters.

### Requirement: Server-Side Search
The search of a listing SHALL be executed by the database against the underlying columns, and SHALL be case-insensitive. The search SHALL be triggered by an explicit `Buscar` action and by the `Enter` key, and SHALL additionally be triggered automatically after the user stops typing for 500 ms.

#### Scenario: Searching through the explicit action
- **WHEN** the user types a search term and triggers the `Buscar` action
- **THEN** the database is queried with that term applied as a filter
- **AND** the grid shows the matching records of the current page.

#### Scenario: Searching by pressing Enter
- **WHEN** the user types a search term and presses `Enter` in the search box
- **THEN** the listing is reloaded applying that term.

#### Scenario: Searching while typing
- **WHEN** the user types a search term and stops typing
- **THEN** the listing is reloaded once, 500 ms after the last keystroke.

#### Scenario: Search with no results
- **WHEN** the search term matches no records
- **THEN** the grid is empty and the toolbar reports zero records.

### Requirement: Report PDF Exports the Complete Filtered Set
Report PDFs SHALL export the complete set of records matching the active filters, regardless of the page currently visible on screen.

#### Scenario: Exporting a paginated report
- **WHEN** the user exports a report to PDF while a page other than the first is displayed
- **THEN** the PDF contains every record matching the active filters.
