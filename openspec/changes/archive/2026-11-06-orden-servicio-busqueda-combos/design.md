# Design: Búsqueda predictiva de Clientes y Servicios en Orden de Servicio

## Context

En `CtlOrdenServicioForm`, la creación y edición de órdenes de trabajo requiere asignar un cliente (lo que filtra los vehículos del cliente) y agregar servicios a la lista de inspección (`dgChecklist`).

Actualmente:
- El selector de cliente es un `<ComboBox>` estático enlazado a `ClientesDisponibles`. No permite escribir para buscar.
- El selector de servicios es un `<ComboBox IsEditable="True">` enlazado a `ServiciosDisponibles`, pero no filtra dinámicamente según lo que se teclea ni despliega automáticamente sugerencias al alcanzar un umbral de caracteres.

Ver motivación completa en [proposal.md](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/openspec/changes/orden-servicio-busqueda-combos/proposal.md).

## Goals / Non-Goals

**Goals:**
- Habilitar en el selector de clientes de `CtlOrdenServicioForm` la búsqueda predictiva activada con **3 o más caracteres** (coincidencia por Nombre, Apellido o DNI), abriendo automáticamente el desplegable con las sugerencias.
- Permitir la doble modalidad en el selector de servicios:
  1. Desplegar manualmente el combo para ver y elegir de la lista completa.
  2. Escribir 3 o más caracteres para filtrar en tiempo real y desplegar las opciones coincidentes.
- Mantener la sincronización con la selección en cascada de vehículos (`FiltrarVehiculosPorCliente`).
- Mantener la coherencia estética de estilos de la aplicación (`CornerRadius="6"`, altura estándar `Height="30"`).

**Non-Goals:**
- No modificar el esquema de base de datos ni procedimientos almacenados (los catálogos residen en memoria en la sesión de carga del formulario).
- No alterar el flujo de guardado ni el cálculo de precios del checklist.

## Decisions

### 1. ComboBox Editable con Colección Filtrada para Clientes
- **Decisión:** Configurar el selector de clientes como un `ComboBox` editable (o con caja de texto de búsqueda integrada) que enlace su `ItemsSource` a una colección observable filtrada (`ClientesSugeridos` o mediante `ICollectionView`).
- **Mecanismo de activación (3 caracteres):**
  - Un evento de cambio de texto (con debounce de ~300 ms vía `Debouncer`) evalúa la longitud del término.
  - Si `longitud < 3`, la lista de sugerencias se mantiene vacía o en estado inicial y el dropdown permanece cerrado.
  - Si `longitud >= 3`, se filtra la lista maestra de clientes buscando coincidencias en `Nombre`, `Apellido` o `Dni`, actualizando la colección y abriendo el desplegable (`IsDropDownOpen = true`).
- **Alternativa descartada:** Popup externo o modal de búsqueda. Resulta disruptivo para la recepcionista; un ComboBox predictivo directo es mucho más fluido.

### 2. Comportamiento Dual en Selector de Servicios
- **Decisión:** Implementar un comportamiento inteligente en el ComboBox de servicios:
  - **Click en el botón desplegable / Dropdown abierto sin texto:** Muestra la totalidad de `ServiciosDisponibles`.
  - **Texto tecleado >= 3 caracteres:** Aplica filtro sobre `ServiciosDisponibles` (por `Nombre`), actualiza la lista visual de opciones y asegura `IsDropDownOpen = true` para selección inmediata.
  - **Texto < 3 caracteres:** Si el usuario no ha escrito o borra el texto, al desplegar vuelve a mostrar la lista completa.
- **Alternativa descartada:** Dos controles separados (un botón de catálogo y un buscador). Un único ComboBox editable bien afinado cubre ambos casos de uso sin saturar visualmente el formulario.

### 3. Preservación del Estado de Selección en MVVM
- **Decisión:** Separar la propiedad de texto de búsqueda tecleado del `ClienteId` y `ServicioBusquedaSeleccionado` reales.
  - El `ClienteId` solo se actualiza cuando el usuario hace clic o confirma un elemento de la lista desplegable.
  - Al seleccionar un cliente, el texto del combo muestra `NombreCompletoConDni` y se dispara `FiltrarVehiculosPorCliente()`.
  - Al agregar un servicio ("➕ Agregar"), se limpia el texto de búsqueda del servicio y se resetea la lista para la siguiente adición.

### 4. Control de foco y cursor
- **Decisión:** Al filtrar en un ComboBox editable de WPF, evitar que la actualización de la colección desplace el cursor del texto al inicio o seleccione todo el texto mientras se escribe. Se gestionará en el code-behind de `CtlOrdenServicioForm.xaml.cs` el evento `KeyUp` o `TextChanged` preservando el `SelectionStart`.

## Risks / Trade-offs

- **[Risk]** En WPF, modificar el `ItemsSource` de un `ComboBox` editable mientras el usuario escribe puede resetear la posición del cursor o cerrar el dropdown.
  - *Mitigación:* Usar filtrado sobre `ICollectionView` (`CollectionViewSource.GetDefaultView(...)`) o manipular la colección subyacente guardando y restaurando el cursor (`SelectionStart = caretIndex`).
- **[Risk]** Selección accidental de texto no validado si el usuario escribe y pierde el foco sin seleccionar.
  - *Mitigación:* Si el texto no coincide con ningún cliente existente o no fue seleccionado de la lista, `ClienteId` permanece en 0 y la validación en `LostFocus` resalta el error en rojo según el estándar del proyecto.
