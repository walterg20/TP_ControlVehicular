# Tasks: Refactor Billing to Facturacion

- [x] **Task 1: Rename Interface and Implementation**
  - Rename `Negocio/Interfaces/IBillingService.cs` -> `Negocio/Interfaces/IFacturacionService.cs`
  - Rename `Negocio/Services/BillingService.cs` -> `Negocio/Services/FacturacionService.cs`
- [x] **Task 2: Update DI and UI Consumers**
  - Update DI registration in `App.xaml.cs`
  - Update `CtlOrdenServicio.xaml.cs` and `FrmPago.xaml.cs`
- [x] **Task 3: Update Test Suite**
  - Update `SimulacionOrdenesServicioTests.cs` to instantiate `FacturacionService`
- [x] **Task 4: Update Documentation and Memory**
  - Update `AGENTS.md` and `MEMORY.md`
- [x] **Task 5: Verify Build and Tests**
  - Execute `dotnet build` and `dotnet test`
