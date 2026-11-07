# ayuda-contextual Specification (delta)

## ADDED Requirements

### Requirement: Contextual Help Panel in Orden de Servicio
The Orden de Servicio form SHALL display a collapsible help panel on the right side showing help sections for the control that currently has focus. The panel SHALL be toggleable (show/hide) by the user.

#### Scenario: Panel toggles visibility
- **WHEN** the user clicks the "Ayuda" toggle button
- **THEN** the help panel shows or hides accordingly

#### Scenario: Help loads on focus change
- **WHEN** a control in the form receives focus
- **THEN** the help panel displays the help sections for that control and role

#### Scenario: No help available
- **WHEN** no help exists for the focused control and role
- **THEN** the panel shows "No hay ayuda disponible para este elemento"

### Requirement: Role-Filtered Help Content
Help content SHALL be filtered by the current user's role. Each role (Administrador, Mecánico, Cliente) SHALL see only the help entries defined for it.

#### Scenario: Mechanic sees mechanic help
- **WHEN** a Mecánico focuses a control with help defined for role 2
- **THEN** the mechanic-specific help sections are displayed

### Requirement: Persisted Section Expansion State
The expanded/collapsed state of each help section SHALL be persisted per role and help entry in `%AppData%/TP_ControlVehicular/Ayudas`.

#### Scenario: Expanded state persists
- **WHEN** a user expands a help section and reopens the form
- **THEN** that section remains expanded

### Requirement: Help Data Source
Help content SHALL be loaded from `Datos/Configuracion/ayudas_ordenes_servicio.json`. The file SHALL be copied to the output directory at build time.

#### Scenario: Missing JSON file
- **WHEN** the JSON file is not present
- **THEN** the service returns no help (no exception)