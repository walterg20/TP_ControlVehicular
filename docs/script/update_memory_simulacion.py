# -*- coding: utf-8 -*-
path = 'MEMORY.md'
with open(path, 'r', encoding='utf-8') as f:
    c = f.read()

target = '## Estado actual (verificar con `git`)'
addition = '''## Estado actual (verificar con `git`)
- Rama de trabajo: `feature/test-simulacion-15-ordenes`. Spec `test-simulacion-15-ordenes` implementada (build con 0 errores; test de simulación end-to-end y 144 tests en verde).
- **Test automatizado de simulación de 15 órdenes de servicio (spec `test-simulacion-15-ordenes`)**:
  - Suite de integración completa `SimulacionOrdenesServicioTests` (`TP_ControlVehicular.Tests/EndToEnd/`).
  - Creación de 8 clientes y 8 vehículos vinculados (`PropietarioVehiculo`).
  - Generación de 15 órdenes de servicio con asignaciones de 1 mecánico (órdenes 1 a 5), 2 mecánicos colaborando en paralelo (órdenes 6 a 10) y 3 mecánicos atendiendo distintas tareas (órdenes 11 a 15).
  - Cobertura de estados canónicos (`Abierta`, `En Proceso`, `Completada`, `Pagada`, `Cancelada`).
  - Facturación y cobro real con `IBillingService` (`Factura` + `Pago` con vuelto para efectivo y pago exacto para tarjeta).
  - Limpieza automática idempotente con prefijo `SIM-` en orden FK-safe.
'''

c = c.replace(target, addition)
with open(path, 'w', encoding='utf-8') as f:
    f.write(c)
print('MEMORY.md updated successfully')
