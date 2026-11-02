# Tasks

## 1. Lógica en ViewModel y Pruebas Unitarias

- [ ] 1.1 Incorporar en `OrdenServicioViewModel` el método y colección de filtrado predictivo de clientes `FiltrarClientes(string texto)` que se active únicamente con longitud >= 3 (coincidiendo por Nombre, Apellido o DNI) y verificar que no altere la lista con < 3 caracteres.
- [ ] 1.2 Incorporar en `OrdenServicioViewModel` el soporte dual para servicios `FiltrarServicios(string texto)`: catálogo completo cuando no hay filtro o tiene < 3 caracteres, y filtrado por Nombre cuando tiene >= 3 caracteres.
- [ ] 1.3 Crear pruebas unitarias en `TP_ControlVehicular.Tests` para validar el umbral de 3 caracteres en el filtrado de clientes y la doble modalidad de servicios, verificando que pasen con `dotnet test`.

## 2. Interfaz de Usuario y Comportamiento Predictivo

- [ ] 2.1 Actualizar el `ComboBox` de clientes (`cmbClientes`) en `CtlOrdenServicioForm.xaml` con `IsEditable="True"`, `StaysOpenOnEdit="True"` y estilo acorde a los estándares de UI (`CornerRadius="6"` y altura 30).
- [ ] 2.2 Implementar en `CtlOrdenServicioForm.xaml.cs` la captura de eventos de texto (`KeyUp`/`TextChanged`) con debounce y preservación de cursor, abriendo automáticamente el desplegable (`IsDropDownOpen = true`) al alcanzar 3 caracteres.
- [ ] 2.3 Ajustar el `ComboBox` de servicios en `CtlOrdenServicioForm.xaml(.cs)` para abrir el catálogo completo al hacer clic en el desplegable y filtrar dinámicamente al escribir >= 3 caracteres, limpiando el campo al pulsar "➕ Agregar".
- [ ] 2.4 Validar la integración en cascada: verificar que al seleccionar un cliente sugerido se asigne `ClienteId` y se actualicen inmediatamente los vehículos disponibles en `cmbVehiculos`.

## 3. Verificación de Integración

- [ ] 3.1 Ejecutar `dotnet build "TP_ControlVehicular.slnx"` y verificar que la solución compile limpiamente sin advertencias nuevas ni errores.
- [ ] 3.2 Ejecutar la suite completa de tests con `dotnet test "TP_ControlVehicular.slnx"` confirmando 100% de pruebas en verde.
- [ ] 3.3 Validar interactivamente en la pantalla de *Registrar Nueva Orden*: búsqueda de cliente por DNI/nombre con 3 letras, selección de vehículo asociado y selección de servicio tanto por dropdown como por búsqueda predictiva.
