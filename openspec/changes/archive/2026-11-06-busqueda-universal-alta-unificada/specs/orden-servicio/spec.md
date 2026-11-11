# Spec Delta

## ADDED Requirements

### Requirement: Universal Client/Vehicle Search in Work Order Form
The work order form (`CtlOrdenServicioForm`) SHALL provide a single editable search control that matches the entered term against the client's `Dni`, `Apellido`, `Nombre` **and** the vehicle's `Patente` simultaneously.

1. **Activation Threshold**: The predictive filter SHALL activate only when the entered term contains **3 or more characters**. Below the threshold the system SHALL NOT filter and SHALL keep the dropdown closed.
2. **Matching Criteria**: The term SHALL be matched case-insensitively as a substring against `Dni`, `Apellido`, `Nombre` of the owner and `Patente` of the vehicle.
3. **Row Format**: Every result row SHALL be displayed as `[Patente] - Apellido, Nombre (DNI) - Vehiculo`, where `Vehiculo` is the brand and model description.
4. **Dropdown Display**: Upon reaching the 3-character threshold, the control SHALL open its dropdown automatically to display the matching candidates.

#### Scenario: Operator searches by license plate
- **GIVEN** `CtlOrdenServicioForm` is open
- **WHEN** the operator types 3 or more characters matching a vehicle plate (e.g. "AA")
- **THEN** the dropdown SHALL open and list every vehicle whose plate contains that text
- **AND** each row SHALL be formatted as `[Patente] - Apellido, Nombre (DNI) - Vehiculo`.

#### Scenario: Operator searches by surname
- **GIVEN** `CtlOrdenServicioForm` is open
- **WHEN** the operator types 3 or more characters matching a client surname (e.g. "Perez")
- **THEN** the dropdown SHALL open and list the vehicles owned by the matching clients.

#### Scenario: Operator types fewer than 3 characters
- **GIVEN** `CtlOrdenServicioForm` is open
- **WHEN** the operator types 1 or 2 characters into the universal search control
- **THEN** the system SHALL NOT trigger predictive filtering
- **AND** the dropdown SHALL remain closed.

---

### Requirement: Selection Auto-Fill and Multi-Vehicle Grouping in Work Order Form
Selecting a row of the universal search SHALL populate the work order client and vehicle selection according to the number of vehicles the client owns.

1. **Single Vehicle Client**: When the selected row identifies exactly one vehicle, the system SHALL set both `ClienteId` and `VehiculoId` of the work order.
2. **Multi Vehicle Client**: When the selected client owns more than one vehicle, the row SHALL display `[Ver N vehiculos]` and selecting it SHALL set only `ClienteId`, leaving the vehicle selection pending for the operator.
3. **Vehicle Dropdown Expansion**: After selecting a multi-vehicle row, the vehicle selector SHALL display only the vehicles of that client and SHALL be opened so the operator can pick one in a single click.

#### Scenario: Selecting a result with a single vehicle
- **GIVEN** a matching client owns exactly one vehicle
- **WHEN** the operator selects that row
- **THEN** the work order's `ClienteId` SHALL be set to the client's ID
- **AND** the work order's `VehiculoId` SHALL be set to that vehicle's ID.

#### Scenario: Selecting a result for a client owning several vehicles
- **GIVEN** a matching client owns 2 or more vehicles
- **WHEN** the operator selects that row
- **THEN** the row SHALL be displayed as `Apellido, Nombre (DNI: nnn) - [Ver N vehiculos]`
- **AND** the work order's `ClienteId` SHALL be set to the client's ID
- **AND** the vehicle selector SHALL be filtered to that client's vehicles and opened.

---

### Requirement: Unified Client and Vehicle Registration from the Universal Search
When the universal search yields no match, the work order form SHALL offer a single unified registration flow that creates the client, the vehicle and their ownership link together.

1. **No-Results Action Row**: When the search yields no match, the dropdown SHALL show a single actionable row with the text `🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo`.
2. **Single Modal**: Choosing that row, or pressing the `➕` button next to the search control, SHALL open **one** modal window containing both the client fields and the vehicle fields.
3. **Modal Layout**: The modal SHALL present two visual columns — client data (DNI, surname, name, phone) on the left and vehicle data (plate, brand, model, optional year and engine) on the right — persisted together by a single `Guardar y Vincular` button.
4. **Plate Pre-Fill**: When the search term looks like a license plate, the modal's plate field SHALL be pre-filled with that term.
5. **Atomic Creation**: Pressing `Guardar y Vincular` SHALL insert the client, insert the vehicle and create the ownership link between them, and SHALL NOT leave a client without a vehicle or vice versa.
6. **Post-Save UX**: After a successful save the modal SHALL close, the work order form SHALL show the newly created client and vehicle as selected, and the keyboard focus SHALL move to the `Kilometraje` field.

#### Scenario: No match found, operator registers from the dropdown
- **GIVEN** the operator typed a term with no matches (e.g. "AG 987 ZK")
- **WHEN** the predictive search completes
- **THEN** the dropdown SHALL show the row `🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehiculo`
- **AND** the plate field of the modal SHALL be pre-filled with `AG 987 ZK` when it was selected.

#### Scenario: Unified registration succeeds
- **GIVEN** the unified modal is open with client and vehicle data filled in
- **WHEN** the operator presses `Guardar y Vincular`
- **THEN** the client, the vehicle and the ownership link SHALL be persisted
- **AND** the modal SHALL close
- **AND** the work order form SHALL show the new client and vehicle selected
- **AND** the `Kilometraje` field SHALL receive the keyboard focus.

#### Scenario: Unified registration is cancelled
- **GIVEN** the unified modal is open
- **WHEN** the operator cancels it
- **THEN** no client, vehicle or ownership record SHALL be created
- **AND** the work order form SHALL keep its previous client and vehicle selection.
