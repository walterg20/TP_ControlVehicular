# Specification: Date Range Filtering for Listings and Reports

## Capability: `date-range-filtering`

### Scenario 1: Default date range initialization on Work Orders listing
```gherkin
Given the user navigates to the Work Orders listing ("CtlOrdenServicio")
When the screen is loaded
Then "FechaDesde" should be initialized to the first day of the current month
And "FechaHasta" should be initialized to today's date
And the DataGrid should display only work orders with "Fecha" within this default period
```

### Scenario 2: Filtering work orders by a custom date range
```gherkin
Given the user is on the Work Orders listing
When the user sets "FechaDesde" to a specific start date
And the user sets "FechaHasta" to a specific end date
Then the DataGrid should update in real time to show only orders within [FechaDesde 00:00:00, FechaHasta 23:59:59]
And orders with dates outside this range should be excluded
```

### Scenario 3: Combining date range filter with text search and status filter
```gherkin
Given the user has selected a date range in the Work Orders listing
When the user selects a status "Completada"
And types a license plate or client name in the search box
Then the DataGrid should display only records matching all criteria simultaneously
```

### Scenario 4: Clearing filters in Work Orders listing
```gherkin
Given the user has applied custom date filters, status filter, and search text
When the user clicks the "Limpiar Filtros" button
Then "FechaDesde" should reset to the first day of the current month
And "FechaHasta" should reset to today's date
And "EstadoFiltro" should reset to "Todos"
And "TextoBusqueda" should be cleared
And the DataGrid should refresh to show all orders of the current month
```

### Scenario 5: Exporting / Printing the filtered list of Work Orders to PDF
```gherkin
Given the user has filtered work orders by date range or status
When the user clicks "📄 Imprimir Listado"
Then a PDF document should be generated using QuestPDF
And the document should include the official workshop header and company logo
And the document should display the applied filters (date range, status, search term) in the subtitle
And the document table should contain exactly the filtered rows from the DataGrid
```

### Scenario 6: Date range filtering in My Assigned Jobs ("CtlMisTrabajos")
```gherkin
Given a mechanic is logged in and navigates to "Mis Trabajos"
When the screen is displayed
Then "FechaDesde" and "FechaHasta" date pickers are available
And changing the date range dynamically filters the master list of assigned work orders
```

### Scenario 7: Date validation when Start Date is greater than End Date
```gherkin
Given the user enters a "FechaDesde" that is strictly greater than "FechaHasta"
When the filter evaluates
Then a validation message should be presented indicating "La fecha 'Desde' no puede ser posterior a 'Hasta'"
And the list should not produce invalid query states
```
