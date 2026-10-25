# Specification: Custom Alert Dialogs

## Requirements
- Requirement: The application MUST use `FrmConfirmacion.MostrarAviso(...)` instead of native `MessageBox.Show` when notifying users that no record has been selected in a DataGrid.
- Requirement: `FrmConfirmacion.MostrarAviso(...)` MUST render a single `#27AE60` Green "Aceptar" button without a Cancel button.
- Requirement: The alert popup MUST display a dark header `#2C3E50` and an icon (`⚠️` or `ℹ️`).

## Scenarios
### Scenario: User clicks Edit or Delete without grid selection
Given the user is on any list view control (`Ctl*.xaml`)
And no row is selected in the DataGrid
When the user clicks "Editar" or "Eliminar"
Then a custom alert dialog is shown with a dark header `#2C3E50`, warning icon `⚠️`, message "Por favor, seleccione un registro de la lista", and a green "✓ Aceptar" button.
When the user clicks "✓ Aceptar"
Then the dialog closes and execution returns.
