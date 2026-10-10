# Proposal: Roles y Asignación de Mecánicos por Detalle

## Why
El sistema actual no restringe las acciones de los usuarios en base a su rol (Administrador, Recepcionista, Mecánico). En un taller mecánico moderno, es crucial aplicar el Principio de Mínimo Privilegio (Principio de Least Privilege):
1. Los **Recepcionistas** deben poder gestionar clientes, vehículos y crear órdenes de servicio (cabeceras).
2. Los **Mecánicos** no deben tener permisos para modificar los datos de los clientes ni del vehículo.
3. Un vehículo puede ser reparado por **múltiples mecánicos** a la vez.

Aprovechando que la tabla `DetalleServicio` ya posee el campo `UsuarioId`, se propone que la asignación del responsable se haga **a nivel de línea de servicio (detalle)** y no de orden completa.

## What
1. **Roles y Permisos UI (`CtlOrdenServicioForm`):**
   - El Mecánico verá los controles de cabecera (Vehículo, Taller, Kilometraje) deshabilitados.
   - El Mecánico solo podrá interactuar con las líneas de servicio (checklist) que le fueron asignadas.
2. **Asignación en UI:**
   - La grilla de checklist (`dgChecklist`) incluirá una nueva columna "Mecánico" mediante un `ComboBox`.
   - Se modificará el `DetalleServicioDto` para soportar la asignación visual (`UsuarioId`).
   - El `OrdenServicioViewModel` cargará la lista de `MecanicosDisponibles` (usuarios con el Rol de Mecánico).

## Impact
- **Database:** Ninguno estructural (el campo `UsuarioId` ya existe).
- **Files modified:** `DetalleServicioDto.cs`, `OrdenServicioViewModel.cs`, `CtlOrdenServicioForm.xaml`, y `CtlOrdenServicioForm.xaml.cs`.
- **Beneficio Operativo:** Permite que varios mecánicos trabajen sobre una misma orden y se restringe la edición insegura de cabeceras.
