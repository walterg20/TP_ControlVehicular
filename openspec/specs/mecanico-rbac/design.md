# Design: RBAC para Mecánico

## Arquitectura y Componentes Afectados

Para llevar a cabo el filtrado dinámico de información, el acceso a los datos necesita conocer el rol y el ID del usuario actualmente autenticado.

### 1. `Presentacion/MainWindow.xaml.cs`
* Modificar `AplicarRestriccionesPorRol(string rolNombre)`:
  - Añadir la lógica para colapsar `secAdministracion` cuando el rol contiene "mecánic" o "mecanic".
  - (Opcional) Proveer el usuario a una clase global de sesión. Una solución rápida y limpia para WPF sin sobreingeniería es utilizar una propiedad global estática, o simplemente que los ViewModels lean de `MainWindow.UsuarioSesionActual`.

### 2. Contexto Global (Opcional pero Recomendado)
* **`Negocio/Context/UserSession.cs`**:
  Se recomienda crear una clase estática pequeña `UserSession` con una propiedad `public static UsuarioDto? CurrentUser { get; set; }`. 
  - Al iniciar sesión en `MainWindow`, se hace `UserSession.CurrentUser = usuario`.
  - Al cerrar sesión, `UserSession.CurrentUser = null`.

### 3. `Negocio/Services/DashboardHandler.cs` (o similar)
* Obtener el `UserSession.CurrentUser`.
* Si `rol.Contains("mecanic")`, ajustar las consultas a Entity Framework:
  - Total Generado: Sumar `Precio * Cantidad` de `DetalleServicio` donde `UsuarioId == currentUser.Id`.
  - Total Servicios: Contar `DetalleServicio` donde `UsuarioId == currentUser.Id`.

### 4. `Negocio/Services/ReporteOrdenesHandler.cs` (o Repository)
* En el método que trae las órdenes para el reporte, inyectar el chequeo del usuario:
  - `query = query.Where(rs => rs.Detalles.Any(ds => ds.UsuarioId == currentUser.Id))` si el rol es Mecánico.

### 5. `Presentacion/ViewModels/DashboardViewModel.cs` y `ReporteOrdenesViewModel.cs`
* Se comunicarán con sus respectivos handlers (que accederán a `UserSession`) o enviarán el ID y Rol como parámetros en las llamadas al Backend.

## Riesgos y Consideraciones
* **Asincronía y Estado Global**: Usar una propiedad estática para el usuario actual es completamente seguro en esta arquitectura porque es una aplicación de escritorio (WPF) de hilo principal, por lo que no hay peticiones concurrentes de múltiples usuarios (no es una app web).
* **Seguridad de Nombres de Rol**: En la BD, el rol de mecánico podría estar escrito con o sin tilde. `rol.Contains("mecánic") || rol.Contains("mecanic")` en minúsculas es lo recomendado para atrapar ambas variantes.
