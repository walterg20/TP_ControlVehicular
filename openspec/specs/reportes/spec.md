# Reportes Specification

## Purpose

Generate statistical information and printable/exportable reports for the workshop, separating the operational view from the managerial view.

## Requirements

### Requirement: Order Report PDF
The system SHALL generate a printable PDF report for a work order, to be delivered to the client when the vehicle is received or when the invoice is handed over (Reception Voucher / Payment Voucher).

#### Scenario: Print the reception voucher
- **WHEN** the reception voucher of a work order is printed
- **THEN** a PDF is produced with the order data

### Requirement: Operational Report
The system SHALL produce an operational report with the number of orders attended by each mechanic within a date range, and the most frequent vehicles.

#### Scenario: Export the operational report
- **WHEN** the operational report is exported
- **THEN** it is available in a visible/printable format (PDF or Excel)

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

### Requirement: PDF Generation Library
The system SHALL generate the report PDFs using a PDF generation library such as iText7 or QuestPDF.

#### Scenario: PDFs produced by the library
- **WHEN** a report is exported to PDF
- **THEN** it is produced using the configured PDF generation library

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
