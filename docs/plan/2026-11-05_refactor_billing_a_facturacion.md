# Plan de Implementación: Refactorizar Billing a Facturacion

## Contexto
El usuario solicitó renombrar la palabra `Billing` por `Facturacion` en interfaces, clases, inyecciones de dependencias, formularios y tests para mantener la nomenclatura íntegramente en español.

## Tareas
1. Mover y renombrar `Negocio/Interfaces/IBillingService.cs` a `Negocio/Interfaces/IFacturacionService.cs`.
2. Mover y renombrar `Negocio/Services/BillingService.cs` a `Negocio/Services/FacturacionService.cs`.
3. Actualizar inyección en `App.xaml.cs`.
4. Actualizar llamadas en `CtlOrdenServicio.xaml.cs` y `FrmPago.xaml.cs`.
5. Actualizar tests en `SimulacionOrdenesServicioTests.cs`.
6. Actualizar `AGENTS.md` y `MEMORY.md`.
7. Verificar compilación y tests.
