# Plan: Test Automatizado de Creación de 15 Órdenes de Servicio

## Objetivo
Implementar un test automatizado integral en `TP_ControlVehicular.Tests` que simule la operativa real del sistema con 15 órdenes de servicio:
- Alta de clientes y sus vehículos vinculados.
- Asignación de servicios a 1, 2 y 3 mecánicos.
- Diferentes estados de orden (`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`).
- Facturación y pago mediante `IBillingService` para órdenes completadas/pagadas.
- Instrucciones paso a paso para ejecutar desde Visual Studio y línea de comandos.

## Fases de Ejecución
1. **Infraestructura y Preparación**:
   - Clase `SimulacionOrdenesServicioTests` bajo `TP_ControlVehicular.Tests/EndToEnd/` (o integración).
   - Utilizar `RequiereSqlServerFactAttribute` y `ContextoTestFactory`.
   - Limpieza previa/idempotente de datos de prueba identificados con el marcador `SIM-`.
2. **Generación de las 15 Órdenes**:
   - 5 órdenes con 1 solo mecánico asignado.
   - 5 órdenes con 2 mecánicos colaborando en tareas distintas.
   - 5 órdenes con 3 mecánicos atendiendo diferentes servicios.
   - Transiciones de estado canónicas y facturación/pagos para las órdenes en estado `Pagada`.
3. **Verificación y Documentación**:
   - Compilación con `dotnet build`.
   - Ejecución de prueba con `dotnet test`.
   - Guía detallada para el usuario sobre cómo ejecutarlo desde el Explorador de Pruebas de Visual Studio.
