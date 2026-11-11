# Design: Automated 15 Service Orders Simulation Test

## Architecture & Components
- **Test Framework**: xUnit (`[Fact]` or `[RequiereSqlServerFact]`) inside `TP_ControlVehicular.Tests/EndToEnd/`.
- **Database Context**: Uses `ContextoTestFactory.Create()` to connect to the configured SQL Server instance.
- **Billing Integration**: Instantiates `BillingService` or uses repositories directly with transactions to issue `Factura` and `Pago` via `MetodoPago` (`Efectivo`/`Tarjeta`).
- **Data Model Flow**:
  1. `Cliente` + `Vehiculo` + `PropietarioVehiculo` linkage.
  2. `RegistroServicio` (header with `VehiculoId`, `TallerId`, `UsuarioId`, `KmIngreso`, `Estado`, `Fecha`).
  3. `DetalleServicio` (lines with `ServicioId`, `UsuarioId` [mechanic], `Precio`, `Cantidad`, `Estado`, `OrdenEjecucion`).
  4. `Factura` + `Pago` for `Pagada` orders.
- **Mechanic Distribution**:
  - Mechanics: Carlos Benítez, Marcos Romero, Lucas Almirón (role 3).
  - 1 Mechanic: 1-2 services handled by the same mechanic.
  - 2 Mechanics: 2-3 services split between 2 mechanics.
  - 3 Mechanics: 3-4 services split among 3 mechanics.
