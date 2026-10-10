# Tareas de Implementación

1. **Crear/Verificar SP en BD:**
   - Crear la migración o script SQL para el Stored Procedure `sp_ServiciosPorMecanico`.
   - *Hecho cuando:* El SP existe en la base de datos y retorna los campos esperados para el reporte.

2. **Implementar `IReporteRepository` y `ReporteRepository`:**
   - Encapsular la llamada al SP usando `SqlQueryRaw`.
   - *Hecho cuando:* El repositorio compila y es capaz de ejecutar el SP mapeando el resultado a un DTO.

3. **Implementar Lógica de Negocio y Seguridad (Handler):**
   - Crear el servicio/handler que consuma el repositorio.
   - Implementar la inyección/lectura de `UserSession.CurrentUser.Id` y la validación del rol.
   - *Hecho cuando:* Si el usuario es rol 3, el `mecanicoId` enviado al repo es estrictamente el del usuario actual, ignorando el parámetro de entrada.

4. **Desarrollar ViewModel `ReporteServiciosViewModel`:**
   - Heredar de `BaseViewModel`.
   - Crear propiedades bindables para fechas, lista de resultados, lista de mecánicos y visibilidad del selector.
   - *Hecho cuando:* Las propiedades notifican cambios y la visibilidad del selector se configura correctamente según el rol del usuario logueado.

5. **Crear UI `CtlReporteServicios.xaml`:**
   - Diseñar la vista usando controles con notación húngara (`dtpFechaDesde`, `dtpFechaHasta`, `cbxMecanicos`, `btnGenerar`, `dgReporte`).
   - Bindings al ViewModel.
   - *Hecho cuando:* La interfaz muestra los datos del reporte y oculta el `cbxMecanicos` para usuarios mecánicos.

6. **Desarrollar ViewModel `MisTrabajosViewModel`:**
   - Implementar la lógica para listar trabajos del mecánico actual y comandos (`RelayCommand`) para agregar `DetalleServicio`.
   - *Hecho cuando:* El ViewModel compila y permite la interacción básica de selección de servicio y preparación del detalle.

7. **Crear UI `CtlMisTrabajos.xaml`:**
   - Diseñar la vista con notación húngara para cargar `DetalleServicio`.
   - *Hecho cuando:* La interfaz está conectada al ViewModel y permite guardar detalles de un servicio.
