# Walkthrough: Test Automatizado de Creación de 15 Órdenes de Servicio

## Resumen
Se implementó la suite automatizada de pruebas end-to-end `SimulacionOrdenesServicioTests` (`TP_ControlVehicular.Tests/EndToEnd/SimulacionOrdenesServicioTests.cs`) que recrea el ciclo completo de vida del taller para 15 órdenes de servicio con las siguientes características:

1. **Creación de Clientes y Vehículos**:
   - 8 clientes con DNI, nombres y domicilios verosímiles.
   - 8 vehículos asociados a través de `PropietarioVehiculo` (`EsActual = true`).
2. **Distribución de Mecánicos**:
   - **Órdenes 1 a 5**: Servicios asignados a **1 solo mecánico** por orden.
   - **Órdenes 6 a 10**: Servicios asignados a **2 mecánicos distintos** colaborando en paralelo dentro de la misma orden.
   - **Órdenes 11 a 15**: Servicios asignados a **3 mecánicos distintos** atendiendo diferentes ítems de la orden.
3. **Diversidad de Estados**:
   - `Abierta` (servicios con estado `Pendiente`).
   - `En Proceso` (servicios con estado `En Curso`).
   - `Completada` (servicios con estado `Finalizada`).
   - `Pagada` (con emisión de `Factura` y registro de `Pago` en Efectivo o Tarjeta vía `IBillingService`).
   - `Cancelada` (orden dada de baja).
4. **Facturación y Pagos**:
   - Emisión de comprobante fiscal mediante `billingService.GenerarFactura(orden.Id)`.
   - Pago con cálculo automático de vuelto para Efectivo y validación de saldo exacto para Tarjeta mediante `billingService.RegistrarPago(...)`.
5. **Idempotencia y Limpieza**:
   - Limpieza automática previa y posterior de registros de simulación (`SIM-`) en orden FK-safe (`Pago` → `Factura` → `DetalleServicio` → `RegistroServicio` → `PropietarioVehiculo` → `Vehiculo` → `Cliente`), garantizando que la prueba pueda ejecutarse repetidas veces sin dejar datos residuales.

---

## Cómo Ejecutar el Test en Visual Studio

### Opción 1: Desde el Explorador de Pruebas (Test Explorer) en Visual Studio
1. Abre la solución `TP_ControlVehicular.slnx` en Visual Studio.
2. Abre el **Explorador de pruebas**:
   - Menú superior: **Prueba** (Test) ➔ **Explorador de pruebas** (Test Explorer) (o el atajo `Ctrl + E, T`).
3. En la barra de búsqueda del Explorador de Pruebas escribe:
   `SimularCreacionDe15OrdenesConMecanicosYFacturacion_EjecutaFlujoCompleto`
4. Haz clic derecho sobre el test y selecciona **Ejecutar** (o pulsa el botón verde **▶**).

### Opción 2: Desde la Terminal / Consola de PowerShell
```powershell
dotnet test --filter "FullyQualifiedName~SimulacionOrdenesServicioTests"
```
