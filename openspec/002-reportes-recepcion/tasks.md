# Implementation Tasks

1. [ ] **Creación de DTOs**: 
   - Crear los DTOs en español (`HistorialVehiculoDto`, `VehiculoClienteDto`) para transportar la información desde el origen de datos.
   - *Hecho cuando*: Las clases existan y tengan las propiedades necesarias correspondientes a las columnas de la BD.

2. [ ] **Migración de Base de Datos**: 
   - Añadir una nueva migración en EF Core que incluya los scripts de creación de los Stored Procedures `sp_HistorialVehiculo` y `sp_VehiculosPorCliente`.
   - *Hecho cuando*: La migración se aplique con éxito en la base de datos local y los SPs sean accesibles.

3. [ ] **Interfaz de Repositorio**: 
   - Definir la interfaz `IReporteRepository` con los métodos asíncronos para ambas consultas.
   - *Hecho cuando*: La interfaz esté definida y en la capa de abstracción correcta.

4. [ ] **Implementación de Repositorio**: 
   - Implementar `IReporteRepository` en EF Core utilizando `SqlQueryRaw` para invocar a los Stored Procedures.
   - *Hecho cuando*: Se puedan ejecutar consultas enviando parámetros (Id o Patente/DNI) y retornen las listas de DTOs correspondientes sin errores de casteo.

5. [ ] **Handlers**: 
   - Implementar `ObtenerHistorialVehiculoHandler` y `ObtenerVehiculosPorClienteHandler`.
   - *Hecho cuando*: Los handlers orquesten la llamada correctamente al repositorio y apliquen validaciones de parámetros básicos (ej. no ambos nulos a la vez).

6. [ ] **ViewModels (MVVM)**: 
   - Implementar `ReportesViewModel` con propiedades que notifiquen los cambios (`INotifyPropertyChanged`) y comandos usando `ICommand`.
   - *Hecho cuando*: El ViewModel exponga los métodos de búsqueda y almacene los resultados de forma reactiva.

7. [ ] **Vistas (WPF)**: 
   - Crear o modificar la UI para incluir la sección de reportes.
   - Utilizar Notación Húngara para los controles (`txtPatente`, `txtDNI`, `btnGenerarReporte`, `dgResultados`).
   - *Hecho cuando*: La UI envíe correctamente los datos al ViewModel, ejecute el ICommand y los resultados se visualicen exitosamente en la grilla (`dgResultados`).
