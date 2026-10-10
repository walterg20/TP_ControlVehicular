# Tasks: Orden de Servicio (Flujo Cliente-Céntrico)

- [ ] **Task 1: Handler de Asignación por Patente**
  - Crear Negocio/Services/AsignarVehiculoPorPatenteHandler.cs. Debe recibir Patente y IdCliente.
  - Buscar en IVehiculoRepository por Patente (añadir método en repositorio si no existe).
  - Si existe: Obtener el propietario actual (EsActual == true). Si el propietario actual es distinto al nuevo, marcarle EsActual = false y FechaVenta = hoy. Luego, agregar el nuevo propietario con EsActual = true. Retornar el VehiculoDto.
  - Si no existe: Retornar null para que el frontend maneje la creación desde cero.

- [ ] **Task 2: ViewModel de la Orden de Servicio**
  - Modificar Presentacion/ViewModels/OrdenServicioFormViewModel.cs.
  - Implementar la lógica en la propiedad IdCliente para que al cambiar, cargue la lista VehiculosDelCliente filtrando los vehículos que le pertenecen.
  - Implementar el AgregarVehiculoRapidoCommand que interactúe con el modal (que se hará en la Task 3) y el handler de la Task 1.

- [ ] **Task 3: Modificaciones XAML en Orden de Servicio**
  - Modificar Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml.
  - Asegurar que el ComboBox de Cliente esté por encima del de Vehículo.
  - Enlazar el ComboBox de Vehículo a VehiculosDelCliente.
  - Agregar botón + para disparar la carga rápida del vehículo.
