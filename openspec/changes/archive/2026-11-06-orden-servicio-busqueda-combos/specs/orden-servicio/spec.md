# Spec Delta

## ADDED Requirements

### Requirement: Client Predictive Search and Selection in Work Order Form
The work order form (`CtlOrdenServicioForm`) SHALL provide an editable search-enabled client selection control. The control SHALL trigger real-time predictive filtering only when the user enters **3 or more characters**.

1. **Activation Threshold**: While the input contains fewer than 3 characters, the predictive filter SHALL NOT activate, and the full client catalog SHALL NOT be aggressively filtered.
2. **Matching Criteria**: When 3 or more characters are entered, the system SHALL filter clients by matching the search term (case-insensitive) against the client's `Nombre`, `Apellido`, or `Dni`.
3. **Dropdown Display**: Upon reaching the 3-character threshold, the control SHALL automatically open its dropdown to display matching client candidates, showing their full name and DNI.
4. **Cascading Vehicle Selection**: When a client is selected from the suggestions, the system SHALL set `ClienteId` and automatically filter the vehicles combo (`cmbVehiculos`) to display only the vehicles owned by the selected client.

#### Scenario: User types at least 3 characters to search a client
- **GIVEN** `CtlOrdenServicioForm` is open
- **WHEN** the user types 3 or more characters (e.g. "Per" or "321") into the client search control
- **THEN** the client list SHALL filter to show only clients whose first name, last name, or DNI contains that text
- **AND** the dropdown list SHALL open to display the matching candidates.

#### Scenario: User types fewer than 3 characters
- **GIVEN** `CtlOrdenServicioForm` is open
- **WHEN** the user types 1 or 2 characters into the client search control
- **THEN** the system SHALL NOT trigger predictive filtering
- **AND** the dropdown list SHALL remain closed until the 3-character threshold is reached.

#### Scenario: User selects a client and vehicles cascade
- **GIVEN** a list of filtered clients is displayed in the dropdown
- **WHEN** the user selects one of the suggested clients
- **THEN** `ClienteId` SHALL be updated to the selected client's ID
- **AND** the vehicle selector SHALL update its options to include only vehicles registered to that client.

---

### Requirement: Dual-Mode Service Selection in Work Order Form
The inspection item/service selection control in `CtlOrdenServicioForm` SHALL support two complementary modes of interaction: direct catalog browsing and 3-character predictive filtering.

1. **Direct Dropdown Mode**: The user SHALL be able to click or open the dropdown control without typing, displaying all available active services for manual selection.
2. **3-Character Predictive Search Mode**: The user SHALL be able to type directly into the service control. When the text reaches **3 or more characters**, the system SHALL filter the services by matching the search term (case-insensitive) against the service `Nombre`, and SHALL display the dropdown showing only the matching services.
3. **Checklist Addition**: Selecting an item from either mode and clicking "➕ Agregar" SHALL add the service to the work order checklist with its default price and a pending status.

#### Scenario: User browses full service catalog via dropdown
- **GIVEN** `CtlOrdenServicioForm` is open and the service search input is empty
- **WHEN** the user clicks the dropdown toggle button on the service selector
- **THEN** the dropdown SHALL open displaying the complete list of available active services.

#### Scenario: User filters services with 3 or more characters
- **GIVEN** `CtlOrdenServicioForm` is open
- **WHEN** the user types 3 or more characters (e.g. "Ace" or "Fre") into the service selector
- **THEN** the service list SHALL filter to show only services whose name contains that term
- **AND** the dropdown list SHALL open to present the matching service options.

#### Scenario: User adds filtered service to the checklist
- **GIVEN** a filtered service is selected in the service selector
- **WHEN** the user clicks the "➕ Agregar" button
- **THEN** the selected service SHALL be added as a new detail line in `dgChecklist` with its corresponding price
- **AND** the service input SHALL be cleared and ready for another selection.
