# Proposal: Automated 15 Service Orders Simulation Test

## Intent
Provide an automated integration/end-to-end test that simulates real-world execution of the automotive workshop system:
1. Creating clients with their respective vehicles.
2. Creating 15 service orders distributed with 1, 2, and 3 assigned mechanics.
3. Exercising various canonical states (`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`).
4. Generating invoices and processing payments through `IBillingService` for paid orders.
5. Providing full transparency and execution guidance within Visual Studio Test Explorer and `dotnet test` CLI.

## Scope
- New test class in `TP_ControlVehicular.Tests/EndToEnd/SimulacionOrdenesServicioTests.cs`.
- Uses EF Core database context or handlers/repositories from the project.
- Idempotent execution using identifiable markers (`SIM-`).
