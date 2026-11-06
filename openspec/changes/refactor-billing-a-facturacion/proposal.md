# Proposal: Refactor Billing to Facturacion

## Motivation
The codebase predominantly uses Spanish naming conventions for business domain entities, repositories, and handlers (`Factura`, `MetodoPago`, `Pago`, `FacturaRepository`, etc.). However, the billing service and its contract were named `IBillingService` and `BillingService`. To achieve consistency with domain language and facilitate academic defense, this change refactors `Billing` to `Facturacion` across all layers.

## Proposed Changes
- Rename `IBillingService` to `IFacturacionService` in `Negocio/Interfaces/IFacturacionService.cs`.
- Rename `BillingService` to `FacturacionService` in `Negocio/Services/FacturacionService.cs`.
- Update dependency injection registration in `App.xaml.cs`.
- Update references in presentation (`CtlOrdenServicio.xaml.cs`, `FrmPago.xaml.cs`).
- Update test references in `TP_ControlVehicular.Tests/EndToEnd/SimulacionOrdenesServicioTests.cs`.
- Update guidelines and references in `AGENTS.md` and `MEMORY.md`.

## Capabilities
- `facturacion-service-naming`: Standardized Spanish interface and class for invoicing and payment registration.
