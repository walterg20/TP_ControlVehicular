# Spec: Alignment of CRUD Screen Styling with CtlModelo

## Scenarios

### Scenario 1: Status filtering with three states
Given an authenticated user with permissions to view and delete entities in Cliente, Usuario, Rol, Servicio, or Taller
When the user views the search bar
Then an "Estado:" combo box is displayed with options "Todos", "Activos", "Inactivos"
And changing the combo box selection triggers a search and reloads page 1 with the filtered data.

### Scenario 2: Inactive entity visual indication
Given a list containing inactive records in Cliente, Usuario, Rol, Servicio, or Taller
When the records are rendered in the DataGrid
Then the DataGrid does not display an "Activo" checkbox column
And inactive records are highlighted with foreground `#C0392B` and background `#FDECEA`.

### Scenario 3: Bottom action buttons layout
Given a CRUD screen (Cliente, Usuario, Rol, Servicio, Taller)
When the user views the bottom action footer
Then the buttons are aligned to the right inside a card with rounded corners (`CornerRadius="12"`)
And the order is: Edit (`#F39C12`), Delete (`#C0392B`), and Reactivate (`#27AE60`)
And Reactivate is visible only when the user has permission to see the status filter (`PuedeVerFiltroEstado`).
