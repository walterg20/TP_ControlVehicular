# Specification: Taller ABM Fix

## Requirements
- Requirement: The user MUST be able to register a new Taller by providing a valid Name (min 5 chars) and Address (min 5 chars), optionally providing a Phone number.
- Requirement: The user MUST be able to edit an existing Taller selected from the `CtlTaller` DataGrid.
- Requirement: The user MUST be able to toggle the active status (baja lógica / reactivación) of a selected Taller after confirming via `FrmConfirmacion`.

## Scenarios
### Scenario: Register a new Taller successfully
Given the user opens `FrmTaller` via the "➕ Nuevo Taller" button
When the user fills valid Name and Address and clicks "Guardar"
Then `GuardarTallerAsync()` persists the new Taller and adds it to the list.

### Scenario: Edit an existing Taller successfully
Given the user selects a Taller from `CtlTaller` and clicks "✏️ Editar Seleccionado"
When `FrmTaller` displays the current details and the user modifies them and clicks "Guardar"
Then `GuardarTallerAsync()` updates the existing Taller record.

### Scenario: Logical deletion of a Taller
Given the user selects a Taller and clicks "🗑️ Eliminar Seleccionado"
When the user confirms the action on `FrmConfirmacion`
Then `ToggleActivoAsync()` flips the `Activo` flag and refreshes the list.
