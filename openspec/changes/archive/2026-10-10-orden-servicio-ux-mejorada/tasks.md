# Tasks: Implementación de UX en Orden de Servicio

- [x] 1. Modificar `OrdenServicioViewModel.cs`:
  - Inyectar dependencia del repositorio de clientes.
  - Agregar `ClientesDisponibles` y `ClienteId`.
  - Crear lógica de filtrado de vehículos en el setter de `ClienteId`.
- [x] 2. Modificar `CtlOrdenServicioForm.xaml`:
  - Agregar sección de selección de cliente con botón de Nuevo `[+]`.
  - Agregar botón de Nuevo `[+]` junto a Vehículo.
- [x] 3. Modificar `CtlOrdenServicioForm.xaml.cs`:
  - Implementar eventos para abrir `FrmCliente` y `FrmVehiculo`.
  - Auto-asignar el Id del mecánico en `BtnAgregarItem_Click` si el usuario es mecánico.
  - Bloquear la edición del ComboBox de Mecánicos en la grilla para los usuarios mecánicos (puede ser cancelando `BeginningEdit` para esa columna).
- [x] 4. Compilar, probar flujos y validar.

