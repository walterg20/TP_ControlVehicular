# Walkthrough — Búsqueda predictiva de Clientes y Servicios en Orden de Servicio

**Fecha:** 2026-11-02  
**Autor:** Antigravity / Walter Velazco  
**Spec:** `openspec/changes/orden-servicio-busqueda-combos/`  
**Rama:** `feature/orden-servicio-busqueda-combos`

---

## 1. Resumen de Cambios

Se implementó la búsqueda predictiva activada con **3 o más caracteres** en el selector de clientes y el soporte de selección dual (desplegable de catálogo completo o búsqueda por texto) en el selector de servicios dentro del formulario de registro y modificación de órdenes de servicio (`CtlOrdenServicioForm`).

### Archivos Modificados / Creados

1. `Presentacion/ViewModels/OrdenServicioViewModel.cs`:
   - Se incorporaron las listas maestras en memoria `_todosLosClientes` y `_todosLosServicios` que se pueblan en `LoadCombosAsync()`.
   - Se implementó `FiltrarClientes(string? texto)`: sólo aplica filtro predictivo si la longitud es `>= 3` (coincidiendo insensible a mayúsculas/minúsculas por `Nombre`, `Apellido`, `Dni` o `NombreCompletoConDni`). Con `< 3` caracteres, restaura la colección completa sin filtrado agresivo.
   - Se implementó `FiltrarServicios(string? texto)`: con texto vacío o `< 3` caracteres mantiene el catálogo completo de servicios; con `>= 3` caracteres filtra por `Nombre` del servicio.

2. `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml`:
   - `cmbClientes`: Se configuró como editable (`IsEditable="True"`), con `IsTextSearchEnabled="False"`, `StaysOpenOnEdit="True"` y `TextSearch.TextPath="NombreCompletoConDni"`.
   - `cmbServicios`: Se configuró como editable (`IsEditable="True"`), con `IsTextSearchEnabled="False"`, `StaysOpenOnEdit="True"`, `x:Name="cmbServicios"` y `TextSearch.TextPath="Nombre"`.

3. `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml.cs`:
   - Se crearon dos instancias de `Debouncer` con retardo de 250 ms (`_debouncerClientes`, `_debouncerServicios`).
   - Se añadieron controladores para los eventos burbujeantes `TextBoxBase.TextChangedEvent`, `SelectionChanged` y `DropDownOpened`.
   - Al alcanzar 3 caracteres, se despliega automáticamente el ComboBox (`IsDropDownOpen = true`) preservando la posición del cursor (`SelectionStart`) y evitando auto-selecciones no deseadas.
   - Al seleccionar un cliente, se actualiza `ClienteId` y se dispara en cascada la actualización de `VehiculosDisponibles` en `cmbVehiculos`.
   - Al agregar un servicio ("➕ Agregar"), se añade a la orden actual, se limpia el texto de búsqueda, se restablece el catálogo completo de servicios y se cierra el desplegable.

4. `TP_ControlVehicular.Tests/ViewModels/OrdenServicioCombosBusquedaTests.cs`:
   - 16 pruebas unitarias con xUnit y Moq validando:
     - Umbral de 3 caracteres en búsqueda de cliente (no filtrado con 0, 1 y 2 caracteres; filtrado por nombre, apellido y DNI con >= 3 caracteres; restauración al borrar).
     - Modalidad dual de servicios (catálogo completo con texto nulo, vacío o < 3 caracteres; filtrado por nombre con >= 3 caracteres; restauración al borrar o limpiar).

---

## 2. Resultados de Pruebas

- **Compilación (`dotnet build "TP_ControlVehicular.slnx"`):** 0 errores.
- **Suite de pruebas (`dotnet test "TP_ControlVehicular.slnx"`):** 125 pruebas ejecutadas, 125 superadas (100% en verde).
- **Validación OpenSpec (`openspec validate "orden-servicio-busqueda-combos"`):** Válida.
