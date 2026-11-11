# Design: Refactor Billing to Facturacion

## Architecture Decision
- Rename interface `IBillingService` to `IFacturacionService` in `Negocio.Interfaces`.
- Rename class `BillingService` to `FacturacionService` in `Negocio.Services`.
- Both methods `GenerarFactura(int registroServicioId)` and `RegistrarPago(int facturaId, int metodoPagoId, decimal montoPagado)` maintain exact contracts and behavior.
- DI in `App.xaml.cs`: `services.AddScoped<IFacturacionService, FacturacionService>();`.
- In `FrmPago.xaml.cs`: constructor parameter and private field renamed to `_facturacionService`.
