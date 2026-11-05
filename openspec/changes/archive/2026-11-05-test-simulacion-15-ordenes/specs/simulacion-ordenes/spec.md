# Spec: Automated Service Orders Simulation Test

## Scenario 1: Creating clients and vehicles for simulation
Given a running SQL Server instance and valid connection
When the simulation suite initializes
Then it ensures at least 3 active mechanics exist in the database
And creates simulated clients and vehicles with prefix `SIM-` if they do not exist.

## Scenario 2: Creating 15 service orders with 1, 2, and 3 mechanics
Given the prepared clients, vehicles, services, and 3 mechanics
When the automated test executes the generation of 15 service orders:
  - Orders 1 to 5 are created with 1 mechanic assigned to their service items.
  - Orders 6 to 10 are created with 2 different mechanics assigned across distinct service items.
  - Orders 11 to 15 are created with 3 different mechanics assigned across distinct service items.
Then all 15 orders are persisted in the database with their respective `DetalleServicio` rows
And no detail row has an unassigned or invalid mechanic (`UsuarioId <= 0`).

## Scenario 3: Order state transitions and billing
Given the 15 generated service orders
When the orders are configured across canonical domain states:
  - Some orders remain `Abierta` or `En Proceso`.
  - Some orders are marked as `Cancelada`.
  - Completed orders have their services marked as `Finalizada` and order as `Completada` or `Pagada`.
  - Paid orders generate a `Factura` and record a `Pago` through `IBillingService`.
Then the resulting database state contains valid invoices and payments matching the order service totals
And all domain constraints and foreign keys are satisfied.
