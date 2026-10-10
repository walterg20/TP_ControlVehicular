# Diseño: Gestión de Trabajos y Reportes de Taller

## Arquitectura y Patrones
La implementación seguirá el patrón MVVM (Model-View-ViewModel) puro en WPF, haciendo uso de `BaseViewModel` y `RelayCommand`. 
El acceso a datos para el reporte se realizará a través de Entity Framework Core utilizando `SqlQueryRaw` para ejecutar el Stored Procedure, y este acceso estará encapsulado en un repositorio específico.

## Cambios a Realizar

### 1. Base de Datos
- **Stored Procedure:** Asegurar la existencia o creación de `sp_ServiciosPorMecanico(FechaDesde, FechaHasta, MecanicoId)`.

### 2. Capa de Datos (Data Access)
- **Repositorio:** Crear `IReporteRepository` y su implementación `ReporteRepository`.
- **Método:** `GetServiciosPorMecanicoAsync(DateTime desde, DateTime hasta, int mecanicoId)` que ejecute el SP usando `_context.Database.SqlQueryRaw<ReporteServicioDto>(...)`.

### 3. Capa de Lógica / Handlers
- **Handlers:** Crear un manejador o servicio de aplicación (ej. `ReporteTallerHandler`) que valide el rol del usuario.
- **Inyección de Dependencias:** Utilizar `UserSession.CurrentUser` (o equivalente) para obtener el `Id` del usuario y su `Rol`.
- **Regla de Negocio:** En el handler, si `UserSession.CurrentUser.Rol == 3` (Mecánico), sobrescribir el parámetro `mecanicoId` con `UserSession.CurrentUser.Id` antes de llamar al repositorio.

### 4. Capa de Presentación (UI WPF)
- **Notación Húngara:** Se debe usar notación húngara para los nombres de los controles XAML (ej. `dtpFechaDesde`, `cbxMecanicos`, `dgReporte`).
- **Vista `CtlMisTrabajos`:** UserControl para listar trabajos asignados y permitir la carga de `DetalleServicio`.
- **Vista de Reporte:** UserControl (ej. `CtlReporteServicios`) con los filtros de fechas y el combobox de mecánicos.
- **ViewModels:** 
  - `MisTrabajosViewModel`: Hereda de `BaseViewModel`, usa `RelayCommand` para las acciones de guardar detalles.
  - `ReporteServiciosViewModel`: Maneja la lógica de visibilidad del combobox `cbxMecanicos` dependiendo del rol. Si es mecánico, `IsMecanicoSelectorVisible = false`.
