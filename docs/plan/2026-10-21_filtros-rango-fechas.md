# Plan de Implementación: Filtros por Rango de Fechas en Listados y Reportes

## Relevamiento del Estado Actual

Tras realizar una auditoría integral de todos los listados (`Presentacion/Pantalla/`) y reportes (`Presentacion/Pantalla/Reporte/`, `Negocio/Reportes/`, `QuestPDF`) del sistema, se identificó la siguiente situación:

### 1. Estado de los Reportes Existentes
- **`Reporte Gerencial` (`CtlReporteGerencial`)**: Ya cuenta con filtro global `FechaDesde` y `FechaHasta` en UI para sus 4 pestañas (Ingresos Totales, Top Clientes, Modelos Reparados, Servicios por Rango). Los PDFs generados respetan dicho rango y muestran el período en el encabezado.
- **`Reporte Operativo` (`CtlReporteOperativo`)**: Ya cuenta con filtros `FechaDesde` y `FechaHasta` en UI y los transmite al SP de productividad y al PDF de exportación.
- **`Reporte de Órdenes` (`CtlReporteOrdenes`)**: Ya cuenta con selectores de fecha `FechaDesde` y `FechaHasta` tanto en su grilla como en la emisión del documento PDF.
- **Reportes del Mecánico (`CtlMisTrabajos`)**:
  - `Tareas del Día` (`GenerarHojaTrabajoPdfAsync`): Actualmente está fijo en `DateTime.Today`. No permite elegir una fecha o rango de trabajo.
  - `Historial Clínico del Vehículo` (`GenerarHistorialPdfAsync`): Imprime todas las intervenciones del vehículo históricas sin opción de acotar por período.

### 2. Estado de los Listados Operativos (DataGrids)
- **`Órdenes de Servicio` (`CtlOrdenServicio`)**: **NO TIENE FILTRO DE FECHAS**. Siendo la pantalla central del taller donde se gestionan las órdenes día a día, solo dispone de un cuadro de búsqueda de texto general (`TextoBusqueda`). No permite visualizar órdenes de una fecha puntual, de una semana o de un mes, ni tampoco emitir/imprimir un reporte directo del listado filtrado.
- **`Mis Trabajos` (`CtlMisTrabajos`)**: **NO TIENE FILTRO DE FECHAS**. El maestro de órdenes solo filtra por texto y estado (Pendientes/Finalizados/Todos), pero no por fecha de ingreso o asignación.
- **Otros listados de entidad (`Cliente`, `Vehiculo`, `Usuario`, `Servicio`, `Taller`, `Marca`, `Modelo`, `Rol`)**: Son catálogos maestros descriptivos no transaccionales (los vehículos solo guardan `Año` entero, los usuarios y clientes guardan fecha de nacimiento fija). Se mantienen con búsqueda por texto libre.
- **`Respaldos` (`CtlBackup`)**: Lista archivos físicos `.bak` locales; la lista es muy acotada.

---

## Decisiones Confirmadas por el Usuario

1. **Botón de Impresión de Listado**: Se agregará el botón `📄 Imprimir Listado` en `CtlOrdenServicio` para generar directamente un documento PDF con las órdenes que están visibles bajo los filtros aplicados (usando el formato oficial con logo, resumen y totales).
2. **Período por Defecto**: Las pantallas con filtros de fecha iniciarán con el **mes actual** seleccionado por defecto:
   - `FechaDesde` = Primer día del mes actual (`new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)`).
   - `FechaHasta` = Día de la fecha (`DateTime.Today`).

---

## Cambios Propuestos por Componente

### Componente 1: Listado Principal de Órdenes de Servicio (`Presentacion/Pantalla/OrdenServicio/`)

#### [MODIFY] [CtlOrdenServicio.xaml](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/Presentacion/Pantalla/OrdenServicio/CtlOrdenServicio.xaml)
- Rediseñar la barra superior de filtros (`Row 1`):
  - Selectores `DatePicker` para `Desde:` y `Hasta:`.
  - Selector `Estado:` (`ComboBox` con "Todos", "Abierta", "En Proceso", "Completada", "Pagada", "Cancelada").
  - Campo de texto `🔍 Buscar:` (`TextBox Text="{Binding TextoBusqueda}"`).
  - Botón `🔄 Limpiar` para restablecer al mes actual o todo.
  - Botón `📄 Imprimir Listado` con estilo morado (`#8E44AD`) o verde para emitir el PDF de la grilla filtrada.

#### [MODIFY] [OrdenServicioViewModel.cs](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/Presentacion/ViewModels/OrdenServicioViewModel.cs)
- Propiedades observables:
  - `DateTime? _fechaDesde` (inicializado al primer día del mes actual).
  - `DateTime? _fechaHasta` (inicializado a `DateTime.Today`).
  - `string _estadoFiltro = "Todos"`.
  - `string _mensajeValidacionFechas`.
- Lógica de `ListadoOrdenesFiltered`:
  - Evalúa `o.Fecha.Date >= FechaDesde.Value.Date` si tiene valor.
  - Evalúa `o.Fecha.Date <= FechaHasta.Value.Date` si tiene valor.
  - Evalúa `o.Estado == EstadoFiltro` si no es "Todos".
  - Búsqueda por texto (patente, cliente, vehículo, recepción, taller, id).
- Comando `LimpiarFiltrosCommand`:
  - Restablece al primer día del mes y fecha actual.
- Comando `ImprimirListadoCommand`:
  - Genera documento QuestPDF en formato horizontal A4 con `ComposeEncabezadoTaller("Listado de Órdenes de Trabajo")`, subtítulo con filtros aplicados, tabla con columnas (`N°`, `Fecha`, `Cliente`, `Patente`, `Vehículo`, `Recepción`, `Km`, `Estado`) y `ComposePieDePagina()`.

---

### Componente 2: Mis Trabajos del Mecánico (`Presentacion/Pantalla/MisTrabajos/`)

#### [MODIFY] [CtlMisTrabajos.xaml](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml)
- En la barra de filtros superior (`Row 1`):
  - Añadir `DatePicker` para `Desde:` y `Hasta:`.
  - Botón `Limpiar`.

#### [MODIFY] [MisTrabajosViewModel.cs](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/Presentacion/ViewModels/MisTrabajosViewModel.cs)
- Propiedades `FechaDesde` (1° del mes) y `FechaHasta` (hoy).
- Modificar `OrdenesFilter`:
  - Filtra órdenes según `orden.Fecha.Date >= FechaDesde.Value.Date` y `orden.Fecha.Date <= FechaHasta.Value.Date`.
- Modificar `GenerarHojaTrabajoPdfAsync`:
  - Utilizar el rango o fecha seleccionada en vez de asumir exclusivamente `DateTime.Today`.
- Modificar `GenerarHistorialPdfAsync`:
  - Si hay rango de fechas seleccionado, filtrar las intervenciones del vehículo entre esas fechas.

---

### Componente 3: Reportes Existentes (`Presentacion/Pantalla/Reporte/`)

#### [MODIFY] [CtlReporteOrdenes.xaml](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/Presentacion/Pantalla/Reporte/CtlReporteOrdenes.xaml) y [ReporteOrdenesViewModel.cs](file:///e:/UNNE/Taller%20de%20Programaci%C3%B3n%20II/Proyecto/TP_ControlVehicular/Presentacion/ViewModels/ReporteOrdenesViewModel.cs)
- Inicializar `FechaDesde` al 1° del mes actual y `FechaHasta` a `DateTime.Today`.
- Agregar botón `🔄 Limpiar Filtros`.
- Agregar mensaje de advertencia visual cuando `FechaDesde > FechaHasta`.

---

## Plan de Verificación

### Pruebas Automatizadas
1. Compilación limpia:
   ```powershell
   dotnet build "TP_ControlVehicular.slnx"
   ```
2. Ejecución de suite de tests:
   ```powershell
   dotnet test "TP_ControlVehicular.slnx"
   ```
3. Pruebas unitarias de filtrado por fechas para `OrdenServicioViewModel` y `MisTrabajosViewModel`.

### Pruebas Manuales
1. **Listado de Órdenes (`CtlOrdenServicio`)**:
   - Abrir la pantalla; comprobar que inicie filtrando por el mes actual.
   - Cambiar fechas y verificar que la grilla filtre en tiempo real.
   - Cambiar estado y buscar texto.
   - Presionar `📄 Imprimir Listado` y verificar que el PDF generado contenga exactamente los registros filtrados con encabezado y logo.
2. **Mis Trabajos (`CtlMisTrabajos`)**:
   - Probar filtrado por fechas en las órdenes del mecánico.
   - Emitir hoja de trabajo del período.
3. **Reporte de Órdenes (`CtlReporteOrdenes`)**:
   - Probar el botón limpiar y la validación de fechas invertidas.
