# Proposal: Búsqueda predictiva de Clientes y Servicios en Orden de Servicio

## Why

Al registrar o modificar una orden de servicio en `CtlOrdenServicioForm`, la selección del cliente se realiza a través de un `ComboBox` tradicional que lista todos los clientes sin posibilidad de búsqueda dinámica, lo cual resulta lento e incómodo a medida que crece la cartera de clientes. Asimismo, la selección de servicios no cuenta con un comportamiento predictivo optimizado que permita tanto desplegar el catálogo completo como filtrar rápidamente escribiendo unas pocas letras.

Esta mejora agiliza la carga operativa de la recepcionista en el taller mediante la activación de búsquedas predictivas a partir de 3 caracteres tanto para clientes (por nombre, apellido o DNI) como para servicios.

## What Changes

- **Búsqueda y selección de Cliente**:
  - El selector de cliente en `CtlOrdenServicioForm` permitirá escribir texto de búsqueda.
  - Al ingresar **al menos 3 caracteres**, se activará el filtrado dinámico de clientes coincidentes (por Nombre, Apellido o DNI) y se desplegarán las sugerencias para seleccionar.
  - Al elegir un cliente, se preservará la vinculación existente actualizando `ClienteId` y filtrando en cascada sus vehículos asociados en el combo de vehículos.
  - Si el campo tiene menos de 3 caracteres, se mantendrá en espera de mayor precisión o mostrará el estado inicial sin disparar búsquedas costosas.

- **Búsqueda y selección de Servicio**:
  - El combo de servicios ofrecerá dos modalidades de interacción complementarias:
    1. **Despliegue directo**: Permite abrir el combo haciendo clic para explorar y seleccionar manualmente de la lista completa de servicios disponibles.
    2. **Filtro predictivo por 3 caracteres**: Si el usuario escribe al menos 3 caracteres en el control, se desplegarán automáticamente las opciones que coincidan con el término para agilizar la adición al checklist de inspección.

## Capabilities

### New Capabilities
None.

### Modified Capabilities
- `orden-servicio`: Se extiende la especificación de `CtlOrdenServicioForm` para incorporar la búsqueda predictiva activada a partir de 3 caracteres en el selector de Clientes y el comportamiento dual (despliegue completo o filtrado por 3 caracteres) en el selector de Servicios.

## Impact

- **Presentación**:
  - `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml`: Modificación de los controles ComboBox de cliente y servicio para soportar edición, filtrado y apertura predictiva.
  - `Presentacion/Pantalla/OrdenServicio/CtlOrdenServicioForm.xaml.cs`: Manejo de eventos de texto y apertura controlada del desplegable.
  - `Presentacion/ViewModels/OrdenServicioViewModel.cs`: Propiedades observables y métodos de filtrado con umbral de 3 caracteres para clientes y servicios (`ClientesFiltrados`, `ServiciosFiltrados`).
- **Dependencias**:
  - Reutilización del helper `Debouncer` existente (`Presentacion/Helpers/Debouncer.cs`) para evitar sobrecarga de eventos en la UI.
- **Tests**:
  - Pruebas unitarias en `TP_ControlVehicular.Tests` validando que las colecciones filtradas solo se actualicen con >= 3 caracteres y coincidan con Nombre/Apellido/DNI en clientes y Nombre en servicios.
