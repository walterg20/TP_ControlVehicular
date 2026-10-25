# Specification: Reception Operational Reports

## Overview
This specification details the requirements for two operational reports: Vehicle History and Client Vehicles. These reports will be powered by Stored Procedures (`sp_HistorialVehiculo` and `sp_VehiculosPorCliente`).

## Acceptance Criteria
1. The system MUST allow the receptionist to generate a "Vehicle History Report" by providing either a `VehiculoId` or a `Patente` (license plate).
2. The system MUST allow the receptionist to generate a "Client Vehicles Report" by providing either a `ClienteId` or a `DNI`.
3. Data MUST be retrieved using the specific Stored Procedures.
4. The UI MUST display the results in a tabular format (data grid).

## BDD Scenarios

### Feature: Vehicle History Report
```gherkin
Scenario: Generate Vehicle History by Patente
  Given the receptionist is on the Reports view
  When they enter the Patente "AB123CD" into the Vehicle History search field
  And they click "Generate Report"
  Then the system should execute "sp_HistorialVehiculo"
  And display a list of historical records for the vehicle with Patente "AB123CD"
```

### Feature: Client Vehicles Report
```gherkin
Scenario: Generate Client Vehicles by DNI
  Given the receptionist is on the Reports view
  When they enter the DNI "12345678" into the Client Vehicles search field
  And they click "Generate Report"
  Then the system should execute "sp_VehiculosPorCliente"
  And display a list of all vehicles owned by the client with DNI "12345678"
```
