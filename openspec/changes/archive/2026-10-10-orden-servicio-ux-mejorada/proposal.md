# Proposal: Mejoras de UX en Orden de Servicio (Filtros y Auto-asignación)

## Why
El flujo actual de creación de Órdenes de Servicio requiere seleccionar directamente un vehículo. Si un cliente posee múltiples vehículos, es difícil identificar cuál es el correcto sin filtrar por cliente primero. Además, si el cliente o vehículo es nuevo, obliga al usuario a salir de la pantalla, ir al ABM, crearlo y volver.
Por otro lado, la asignación de mecánicos debe ser inteligente: si un recepcionista agrega un servicio, él elige quién lo hará; pero si un mecánico agrega un servicio durante su inspección, el sistema debe asumir automáticamente que es él quien lo está realizando y bloquear el cambio para evitar errores o inconsistencias.

## What
1. **Filtro en Cascada (Cliente -> Vehículo):**
   - Agregar un `ComboBox` de Cliente en la cabecera.
   - Al seleccionar un Cliente, el combo de Vehículos se filtrará para mostrar solo los vehículos asociados a ese cliente.
2. **Creación Rápida (Accesos Directos):**
   - Agregar botones `[+]` junto a los combos de Cliente y Vehículo para abrir `FrmCliente` y `FrmVehiculo` como ventanas modales.
   - Al cerrar el modal, se recargarán los combos y se auto-seleccionará el elemento recién creado.
3. **Auto-asignación de Mecánico:**
   - En el evento de agregar ítem al checklist, si el usuario logueado es Mecánico, se pre-asignará su `UsuarioId` a la línea.
   - El combo de "Mecánico" en el checklist se deshabilitará completamente cuando la pantalla sea abierta por un Mecánico.

## Impact
- **Files modified:** `OrdenServicioViewModel.cs`, `CtlOrdenServicioForm.xaml`, `CtlOrdenServicioForm.xaml.cs`.
- **Beneficio Operativo:** Acelera drásticamente la carga de órdenes de servicio en mostrador y hace a prueba de errores el ingreso de datos por parte de los mecánicos.
