# Spec Delta

## MODIFIED Requirements

### Requirement: Database Pagination for Managerial Total Income Grid
The system SHALL provide database-level pagination for the Managerial Total Income grid. Queries for managerial income SHALL support server-side paging (using SQL Server `OFFSET ... FETCH NEXT ... ROWS ONLY` or equivalent database paging mechanism) to retrieve only the records corresponding to the requested page index and page size, along with the total count of matching records. The user interface SHALL provide navigation controls (First, Previous, Next, Last) and display the current page, total pages, and total record count. The navigation controls SHALL be provided by the shared pagination toolbar (`CtlPaginacion`), and the ViewModel SHALL derive its pagination state from the shared pagination base class (`BasePaginableViewModel`), preserving the existing public member names and behaviour.

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

### Requirement: Report Listings Paginados en Base de Datos
Las grillas de reportes que listan registros (Reporte de Órdenes, Reportes de Recepción) SHALL recuperar sus registros mediante paginación del lado del servidor. Los reportes que exponen proyecciones aplanadas sobre datos relacionados (Reporte de Órdenes, historial de vehículo, vehículos por cliente) SHALL ejecutar la consulta paginada mediante procedimientos almacenados en `bd/03_procedimientos.sql`. La exportación a PDF de un reporte SHALL incluir el conjunto completo de registros que cumple los filtros activos, con independencia de la página visible en pantalla.

#### Scenario: Reporte de órdenes paginado
- **WHEN** el usuario consulta el Reporte de Órdenes
- **THEN** el sistema recupera únicamente los registros de la página actual desde la base de datos
- **AND** la barra de paginación indica la página actual, el total de páginas y el total de registros filtrados.

#### Scenario: Reportes de recepción paginados
- **WHEN** el usuario genera el historial de un vehículo o los vehículos de un cliente
- **THEN** el sistema recupera únicamente los registros de la página actual desde la base de datos.

#### Scenario: Exportación PDF con conjunto completo
- **WHEN** el usuario exporta a PDF un reporte con filtros aplicados y una página distinta de la primera visible
- **THEN** el PDF contiene todos los registros que cumplen los filtros activos.

### Requirement: Filtros de búsqueda en el servidor para reportes
El filtrado textual de los listados de reportes SHALL ejecutarse en la base de datos, sobre las columnas de la consulta paginada, sin distinguir mayúsculas de minúsculas. El cambio de cualquier filtro SHALL reiniciar la página actual a 1.

#### Scenario: Búsqueda textual en reporte de órdenes
- **WHEN** el usuario escribe un término en el buscador del Reporte de Órdenes y ejecuta la consulta
- **THEN** la base de datos filtra por patente, marca/modelo, cliente, mecánico, recepcionista o servicios aplicados, sin distinguir mayúsculas de minúsculas.

#### Scenario: Cambio de rango de fechas
- **WHEN** el usuario modifica el rango de fechas del Reporte de Órdenes
- **THEN** la página actual vuelve a 1 y la consulta se ejecuta con el nuevo rango.
