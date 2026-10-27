# Design Document: Reportes de Recepción

## Arquitectura
La solución sigue una arquitectura limpia orientada a casos de uso (Handlers), utilizando MVVM para la capa de presentación WPF. No se utilizará MediatR; los Handlers se instanciarán o inyectarán directamente de acuerdo a las convenciones del proyecto.

## Capa de Acceso a Datos
- Se creará una interfaz `IReporteRepository`.
- La implementación en EF Core invocará los Stored Procedures usando `SqlQueryRaw`.
- Las llamadas SQL a mapear serán:
  - `EXEC sp_HistorialVehiculo @VehiculoId, @Patente`
  - `EXEC sp_VehiculosPorCliente @ClienteId, @DNI`

## Capa de Aplicación (Handlers)
- `ObtenerHistorialVehiculoHandler`: Recibirá los parámetros desde la UI y llamará al repositorio.
- `ObtenerVehiculosPorClienteHandler`: Recibirá los parámetros desde la UI y llamará al repositorio.
- Los DTOs de respuesta estarán en español (`HistorialVehiculoDto`, `VehiculoClienteDto`).

## Capa de Presentación (UI / MVVM)
- Se implementará un ViewModel (`ReportesViewModel`) que expondrá propiedades reactivas para los criterios de búsqueda de la recepcionista.
- Se utilizará la interfaz `ICommand` para enlazar los botones de la vista con la ejecución de los Handlers.
- Se respetará estrictamente la Notación Húngara para los controles de la vista WPF (ej. `txtPatente`, `txtDNI`, `btnGenerarHistorial`, `dgResultados`).
