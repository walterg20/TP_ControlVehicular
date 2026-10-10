# Specification: Custom Confirmation Dialog

## Requirements
- Requirement: The application MUST use a custom confirmation modal window (`FrmConfirmacion`) instead of native Windows `MessageBox.Show` when asking users to confirm deletion or status changes.
- Requirement: `FrmConfirmacion` MUST have a dark header (`#2C3E50`) with an explicit title, a main prompt text, and action buttons.
- Requirement: The Accept button MUST be styled in Green (`#27AE60`) with white text and `CornerRadius="6"`.
- Requirement: The Cancel button MUST be styled in Red (`#C0392B`) with white text and `CornerRadius="6"`.
- Requirement: `FrmConfirmacion` MUST support a static method `FrmConfirmacion.Mostrar(mensaje, titulo, owner)` returning `bool` (`true` if accepted, `false` if cancelled or closed).

## Scenarios
### Scenario: User confirms item deletion
Given the user is on any list view control (`Ctl*.xaml`)
And an item is selected
When the user clicks the "Eliminar" or "Cambiar Estado" button
Then `FrmConfirmacion` is displayed with a dark header `#2C3E50`, green Accept button, and red Cancel button
When the user clicks "Sí, Aceptar"
Then `FrmConfirmacion.Mostrar()` returns `true` and the deletion or status update is executed.

### Scenario: User cancels item deletion
Given the confirmation modal `FrmConfirmacion` is displayed
When the user clicks "No, Cancelar" or closes the window
Then `FrmConfirmacion.Mostrar()` returns `false` and the action is aborted without modifying data.
