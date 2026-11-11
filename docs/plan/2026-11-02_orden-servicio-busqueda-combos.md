# Plan de Ejecución — Búsqueda predictiva de Clientes y Servicios en Orden de Servicio

**Fecha:** 2026-11-02  
**Autor:** Antigravity / Walter Velazco  
**Spec relacionada:** `openspec/changes/orden-servicio-busqueda-combos/`  
**Rama:** `feature/orden-servicio-busqueda-combos`

---

## 1. Contexto y Diagnóstico
En la pantalla de registro y modificación de órdenes de servicio (`CtlOrdenServicioForm`), la selección del cliente y de los servicios a inspeccionar presentaba limitaciones de usabilidad:
- El cliente se elegía desde un `ComboBox` no editable que requería desplazarse por toda la lista de clientes.
- El selector de servicios no admitía filtrado predictivo por teclado manteniendo la capacidad de desplegar el catálogo completo.

---

## 2. Solución a Implementar
1. **Selector de Clientes predictivo (umbral 3 caracteres)**:
   - Convertir `cmbClientes` en un `ComboBox` editable.
   - En `OrdenServicioViewModel`, implementar `FiltrarClientes(string texto)`.
   - Si `texto.Length >= 3`, filtra los clientes activos comparando insensible a mayúsculas/minúsculas con `Nombre`, `Apellido` y `Dni`, poblando `ClientesFiltrados` y abriendo el dropdown.
   - Si `texto.Length < 3`, no filtra agresivamente y mantiene la lista general o vacía sugerencias.
   - Al seleccionar un cliente, se actualiza `ClienteId` y se ejecuta la cascada existente `FiltrarVehiculosPorCliente()`.

2. **Selector de Servicios dual (catálogo o predictivo con 3 caracteres)**:
   - En `OrdenServicioViewModel`, implementar `FiltrarServicios(string texto)`.
   - Si el dropdown se despliega sin texto o con menos de 3 caracteres, muestra todos los servicios activos disponibles.
   - Si el usuario escribe 3 o más caracteres, filtra los servicios por `Nombre` y abre las opciones sugeridas.
   - Al hacer clic en "➕ Agregar", se añade el servicio al checklist y se resetea el campo de búsqueda.

---

## 3. Plan de Tareas
- [x] 1.1 Métodos de filtrado en `OrdenServicioViewModel`.
- [x] 1.2 Soporte dual de servicios en `OrdenServicioViewModel`.
- [x] 1.3 Pruebas unitarias de los filtros con umbral de 3 caracteres.
- [x] 2.1 Actualización de XAML en `CtlOrdenServicioForm.xaml`.
- [x] 2.2 Control de eventos de teclado y dropdown en `CtlOrdenServicioForm.xaml.cs`.
- [x] 2.3 Integración del selector dual de servicios en `CtlOrdenServicioForm`.
- [x] 2.4 Verificación de cascada hacia `cmbVehiculos`.
- [x] 3.1 Verificación de compilación y pruebas completas (`dotnet test`).
