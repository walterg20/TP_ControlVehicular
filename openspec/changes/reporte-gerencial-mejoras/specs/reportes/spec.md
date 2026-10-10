# Reportes Specification — Delta

## MODIFIED Requirements

### Requirement: Managerial Report
The system SHALL produce a managerial report with the income generated ($) within a date range, split by service type. The report SHALL be loaded only when the user explicitly requests it (a "Consultar" action); changing the start or end date alone SHALL NOT trigger a query. An invalid range (start date after end date) SHALL be rejected with a validation message and SHALL NOT query. The "most repaired models" table SHALL display each model as its brand followed by its name.

#### Scenario: Income by date range
- **WHEN** a date range is selected and the query is requested
- **THEN** the report shows the income generated in that range split by service type

#### Scenario: Explicit query only
- **WHEN** the user changes the start or end date without requesting a query
- **THEN** no query is executed and the displayed data does not change

#### Scenario: Invalid date range rejected
- **WHEN** the start date is after the end date and the user requests the query
- **THEN** no query is executed and a validation message is shown

#### Scenario: Composed model name
- **WHEN** the most repaired models table is displayed
- **THEN** each row shows the model name composed of its brand and its name

## ADDED Requirements

### Requirement: Service Quantity Breakdown
The system SHALL compute, for a selected date range, the quantity of each service performed (the sum of the quantity registered per service line), and SHALL present it both as a table on the screen and as a pie chart in the managerial report PDF.

#### Scenario: Service quantities in range
- **WHEN** a date range is selected and the query is requested
- **THEN** each service performed in that range is listed once with the total quantity performed

#### Scenario: Pie chart included in the PDF
- **WHEN** the service breakdown is exported to the managerial PDF
- **THEN** the PDF contains a pie chart of the service quantities and the company logo in the header

#### Scenario: Pie chart not shown on screen
- **WHEN** the service breakdown is displayed on the screen
- **THEN** it is shown as a table and no pie chart is rendered in the WPF view
