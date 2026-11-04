# Tasks

## 1. Modelo de datos

- [ ] 1.1 Crear `Negocio/DTOs/BusquedaClienteVehiculoDto.cs` con los campos de la fila de búsqueda (`ClienteId`, `VehiculoId`, `Patente`, `Apellido`, `Nombre`, `Dni`, `VehiculoDescripcion`, `EsAgrupado`, `CantidadVehiculos`, `EsAccionRegistrar`, `DisplayText`) y `ToString()` que devuelve `DisplayText`.

## 2. ViewModel del buscador universal

- [ ] 2.1 Incorporar en `OrdenServicioViewModel` la colección `ResultadosBusquedaUniversal` (`ObservableCollection<BusquedaClienteVehiculoDto>`) y la propiedad `ResultadoBusquedaSeleccionado`.
- [ ] 2.2 Implementar `FiltrarBusquedaUniversal(string? texto)`: con menos de 3 caracteres limpia la colección; con 3 o más arma filas por vehículo que coincidan con `Patente`, `Dni`, `Apellido` o `Nombre`; agrupa en una fila `[Ver N vehiculos]` a los clientes con 2 o más vehículos; si no hay coincidencias agrega la fila de acción `🔍 No se encontraron resultados. + Registrar Nuevo Cliente y Vehículo`; limita el resultado a 25 filas.
- [ ] 2.3 Implementar `SeleccionarResultadoUniversal(BusquedaClienteVehiculoDto resultado)`: fila normal asigna `ClienteId` y `VehiculoId`; fila agrupada asigna solo `ClienteId`; fila de acción dispara el evento de apertura de modal.
- [ ] 2.4 Exponer los eventos `AbrirModalNuevoClienteVehiculoRequested` (con el texto tipeado), `FocoVehiculoRequested` y `FocoKilometrajeRequested`.

## 3. Modal unificado de alta

- [ ] 3.1 Crear `Presentacion/ViewModels/ClienteVehiculoViewModel.cs` con los datos del cliente (DNI, apellido, nombre, teléfono), los del vehículo (patente, marca, modelo, año, motor), el cascade de marca → modelo, la validación (`ValidateProperty` / `ValidateAll`) y `RegistrarAsync()` que inserta cliente, vehículo y vínculo de propiedad y expone `ClienteCreado` / `VehiculoCreado`.
- [ ] 3.2 Crear `Presentacion/Pantalla/OrdenServicio/FrmClienteVehiculo.xaml` con dos columnas (cliente a la izquierda, vehículo a la derecha), botón `Guardar y Vincular` y botón `Cancelar`, respetando el estándar de formularios modales del proyecto.
- [ ] 3.3 Crear `Presentacion/Pantalla/OrdenServicio/FrmClienteVehiculo.xaml.cs` con la construcción del ViewModel, el prellenado de la patente y el cierre con `DialogResult`.
- [ ] 3.4 Registrar `ClienteVehiculoViewModel` en `App.xaml.cs`.

## 4. Interfaz del formulario de orden

- [ ] 4.1 Reemplazar el combo de clientes de `CtlOrdenServicioForm.xaml` por el combo universal editable (`cmbBusquedaUniversal`) enlazado a `ResultadosBusquedaUniversal` con `DisplayMemberPath="DisplayText"`, conservando el botón `➕`.
- [ ] 4.2 Implementar en `CtlOrdenServicioForm.xaml.cs` el `TextChanged` con debounce de 250 ms y preservación del cursor, filtrando con el umbral de 3 caracteres y abriendo el desplegable.
- [ ] 4.3 Implementar la selección: al elegir una fila, invocar `SeleccionarResultadoUniversal`, actualizar el texto del combo y mover el foco según el caso (vehículo o kilometraje).
- [ ] 4.4 Conectar la fila de acción «No se encontraron resultados» y el botón `➕` con la apertura de `FrmClienteVehiculo`, recargando los combos y seleccionando cliente y vehículo creados al cerrar con éxito.
- [ ] 4.5 En modo modificación, mostrar en el combo universal el texto del cliente de la orden y limpiar la colección de resultados para evitar selecciones obsoletas.

## 5. Pruebas

- [ ] 5.1 Crear `TP_ControlVehicular.Tests/ViewModels/OrdenServicioBuscadorUniversalTests.cs` con los casos: umbral de 3 caracteres, coincidencia por patente, coincidencia por apellido, agrupación de clientes con varios vehículos, fila de acción sin resultados y selección (normal / agrupada).
- [ ] 5.2 Crear `TP_ControlVehicular.Tests/ViewModels/ClienteVehiculoViewModelTests.cs` con los casos de validación (patente obligatoria, cliente con DNI/apellido/nombre) y de registro exitoso (inserta cliente, vehículo y vínculo).
- [ ] 5.3 Verificar que los tests preexistentes de `OrdenServicioCombosBusquedaTests` sigan en verde.

## 6. Verificación

- [ ] 6.1 Ejecutar `dotnet build "TP_ControlVehicular.slnx"` sin errores.
- [ ] 6.2 Ejecutar `dotnet test "TP_ControlVehicular.slnx"` con la suite completa en verde.
- [ ] 6.3 Validar en la pantalla *Registrar Nueva Orden*: búsqueda por patente y por apellido, autocompletado de cliente y vehículo, selección de «Ver N vehículos», alta unificada desde la fila sin resultados y foco final en Kilometraje.
