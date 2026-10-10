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
The system SHALL produce a managerial report with the income generated ($) within a date range, split by service type.

#### Scenario: Income by date range
- **WHEN** a date range is selected
- **THEN** the report shows the income generated in that range split by service type

### Requirement: PDF Generation Library
The system SHALL generate the report PDFs using a PDF generation library such as iText7 or QuestPDF.

#### Scenario: PDFs produced by the library
- **WHEN** a report is exported to PDF
- **THEN** it is produced using the configured PDF generation library
