# Walkthrough: Refactorizar Billing a Facturacion

## Resumen de Cambios
- Creado `Negocio/Interfaces/IFacturacionService.cs` y eliminado `Negocio/Interfaces/IBillingService.cs`.
- Creado `Negocio/Services/FacturacionService.cs` y eliminado `Negocio/Services/BillingService.cs`.
- Registrado `IFacturacionService` / `FacturacionService` en el contenedor DI de `App.xaml.cs`.
- Actualizado `CtlOrdenServicio.xaml.cs` y `FrmPago.xaml.cs` para inyectar y consumir `IFacturacionService`.
- Actualizado `SimulacionOrdenesServicioTests.cs` en `TP_ControlVehicular.Tests`.
- Actualizados `AGENTS.md` y `MEMORY.md`.

## Verificación
- `dotnet build "TP_ControlVehicular.slnx"`: 0 errores.
- `dotnet test --filter "FullyQualifiedName!~EndToEnd"`: 147 tests unitarios pasando exitosamente.
- `dotnet test --filter "FullyQualifiedName~SimulacionOrdenesServicioTests"`: 1 test pasando exitosamente.
