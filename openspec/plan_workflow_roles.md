# Propuesta de Feature: Workflow de Órdenes y UX Roles (Actualizado)

## [Goal Description]
Refinar la experiencia de usuario y lógica de negocios del Workflow de Órdenes de Servicio según los requerimientos actualizados: ocultar filas ajenas al mecánico, agregar controles de asignación, formatear el nombre del mecánico y automatizar el estado general de la Orden.

## Requerimientos Finales y Soluciones

### 1. Formato en Combo de Mecánicos
**Problema:** Se requiere mostrar "Nombre Apellido [DNI]".
**Solución:** Modificar `UsuarioDto` añadiendo una propiedad computada `NombreCompletoYDni` (ej: `Pedro Perez [12345678]`) y actualizar el XAML de la grilla (`DisplayMemberPath`) para consumir esta propiedad en el ComboBox de la columna Mecánico.

### 2. Checkbox "Revisado / OK" Interactivo para Recepcionistas
**Solución:** La recepcionista podrá tildar/destildar la casilla libremente. Al estar conectada con rol de Admin/Recepción, no sufrirá las restricciones del mecánico.

### 3. Filtro de Grilla (Ocultar filas ajenas)
**Requerimiento:** El listado debe ocultar de la vista las filas que no le corresponden al mecánico.
**Solución:** Activar un filtro estricto usando `CollectionViewSource.GetDefaultView` sobre la lista de Detalles. Si la Sesión es "Mecánico", solo se renderizan las filas donde `Detalle.UsuarioId == Session.IdUsuario`.

### 4. Alerta de Servicio Duplicado (Oculto)
**Requerimiento:** Si agrega otro servicio de otro mecánico debe dar mensaje de que ya está asignado.
**Solución:** Al momento de presionar `BtnAgregarItem_Click`, el chequeo lógico evalúa la lista subyacente (completa), no la vista filtrada. Si el mecánico Pedro intenta agregar "Alineación", y eso ya fue agregado por Sergio (oculto en la UI de Pedro), saltará un `FrmConfirmacion.MostrarAviso` informando: *"El servicio ya se encuentra asignado a otro mecánico"*.

### 5. Mecánico por Defecto y Combobox Deshabilitado
**Requerimiento:** Al agregar un servicio, asignar el mecánico conectado por defecto y deshabilitar el combo de Mecánico para él.
**Solución:**
* En `BtnAgregarItem_Click`, se detecta la sesión. Si es mecánico, `UsuarioId` se setea automáticamente con su ID (ya hay una base de esto en código, la validaremos).
* En el `DataGrid`, se enlazará la propiedad `IsEnabled` del `ComboBox` a una nueva propiedad del ViewModel `EsRecepcionista`. Así, si entra un mecánico, el combo es de solo-lectura para todas sus filas.

### 6. Transición Automática de Estados
**Requerimiento:** Al tildar "Revisado / OK", el estado de la Orden pasa a "En Proceso" o "Completo".
**Solución:** Agregar lógica en el ViewModel que se dispare cuando cambia un Detalle. 
* Todos completados = "Finalizada".
* Algunos completados = "En Proceso".
* Ninguno completado = "Pendiente".

## User Review Required
> [!IMPORTANT]
> **¿Debería el cambio de estado (Pendiente -> Proceso -> Completo) guardarse en la Base de Datos al instante (automáticamente por detrás), o solo es visual en pantalla y se guarda recién cuando el usuario presiona el botón verde de "Guardar" abajo de todo?** Lo estándar es guardarlo al presionar "Guardar". Confirmar esto.

## Proposed Changes
* `Negocio/DTOs/UsuarioDto.cs`: [MODIFY] Agregar `NombreCompletoYDni`.
* `Presentacion/ViewModels/OrdenServicioViewModel.cs`: [MODIFY] Agregar propiedad `EsRecepcionista`, lógica `EvaluarEstadoOrden()`.
* `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml`: [MODIFY] Cambiar `DisplayMemberPath` y binding de `IsEnabled` en la columna Mecánico.
* `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml.cs`: [MODIFY] Pulir la lógica del `CollectionView` y el mensaje de servicio ya asignado.
