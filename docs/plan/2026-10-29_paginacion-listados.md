# Plan de ejecución — `paginacion-listados` (Fase 1)

**Spec:** `openspec/changes/paginacion-listados/`
**Rama:** `feature/paginacion-listados` (base: `develop` @ `b86b6bf`)
**Fecha:** 2026-10-29

---

## Objetivo

Eliminar el filtrado en memoria de los listados: la paginación y la búsqueda pasan a ejecutarse en la base de datos. La spec se organiza en tres fases por riesgo:

| Fase | Contenido | Riesgo |
|---|---|---|
| 1 | Infraestructura compartida + piloto en Clientes | Bajo |
| 2 | CRUD restantes (Marca, Modelo, Vehículo, Rol, Usuario, Servicio, Taller) | Bajo |
| 3 | Transaccionales y reportes (OrdenServicio, Mis Trabajos, Reporte Órdenes, Reportes Recepción) | Alto (SQL nuevo) |

---

## Fase 1 — Infraestructura y piloto (COMPLETADA)

### 1. Qué se construyó

**Infraestructura compartida**

- `Presentacion/ViewModels/IPaginacion.cs` — contrato de paginación (`INotifyPropertyChanged`) que consume el control.
- `Presentacion/ViewModels/BasePaginableViewModel.cs` — `BaseViewModel` + `IPaginacion`. Centraliza estado de página, clamp del índice, cálculo de `TotalPaginas`, normalización de `TamanoPagina` (5..200), comandos de navegación y el hook abstracto `CargarPaginaAsync()`.
- `Presentacion/Pantalla/Compartido/CtlPaginacion.xaml(.cs)` — control reutilizable. Réplica exacta de la barra que tenía `CtlReporteGerencial` embebida (`#F8F9F9`, borde `#E5E7E9`, `CornerRadius=8`, `Padding=12,8`, botones `#2C3E50`). Recibe el estado por la DP `Model` de tipo `IPaginacion` y enlaza el contenido contra ese modelo (no contra el `DataContext` de la pantalla).
- `Presentacion/Helpers/Debouncer.cs` — coalesce de llamadas con retardo de 500 ms. Usa `Task.Delay` con `CancellationTokenSource`, que captura el `SynchronizationContext` de WPF: la acción diferida vuelve al hilo de interfaz.

**Migración del Reporte Gerencial**

- `ReporteGerencialViewModel` ahora hereda `BasePaginableViewModel`; se eliminó el estado de paginación duplicado. Se conservaron los nombres y tipos públicos (`PaginaActual`, `TotalPaginas`, `TotalRegistros`, `TamanoPagina`, `OpcionesTamanoPagina`, `TextoPaginacion`, `PuedeIrAnterior`, `PuedeIrSiguiente` y los cuatro comandos `RelayCommand`), por lo que `ReporteGerencialPaginacionTests` sigue pasando sin cambios.
- `CtlReporteGerencial.xaml` reemplaza el `Border` de la barra por `<compartido:CtlPaginacion Grid.Row="3" Model="{Binding}"/>`.

**Capa de datos**

- `Datos/Repositories/Repository.cs` — nuevo `GetPaginadoAsync(pagina, tamanoPagina, filtro, configurador, orden)`: `Skip`/`Take` (EF Core 10 los traduce a `OFFSET ... FETCH NEXT ... ROWS ONLY`) + `CountAsync()` sobre la misma consulta filtrada. Se agrega el helper `CrearPatronLike` que escapa `%`, `_` y `[` para que la búsqueda sea de subcadena literal y no de comodines.
- `Negocio/Interfaces/IClienteRepository.cs` + `Datos/Repositories/ClienteRepository.cs` — `GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` con `EF.Functions.Like` sobre `Nombre`, `Apellido` y `Dni`, y filtro de `Activo` según `incluirInactivos`.

**Piloto en Clientes**

- `ClienteViewModel` hereda `BasePaginableViewModel`. Se eliminó `ListadoClientesFiltered` (el filtrado en memoria). `CargarPaginaAsync()` consulta el repositorio paginado y llama a `ActualizarTotales(total)`.
- Búsqueda server-side: el setter de `TextoBusqueda` sólo actualiza estado y reinicia la página a 1; la consulta se dispara desde `BuscarAsync()` (botón `Buscar` y tecla `Enter`) y desde `ProgramarBusqueda()` (debounce de 500 ms disparado por `TextChanged`).
- `CtlCliente.xaml` — la grilla sigue enlazando a `Clientes`; se agrega la fila con `<compartido:CtlPaginacion Model="{Binding}"/>`; el `TextBox` de búsqueda gana `TextChanged` y `KeyDown`; el check `Mostrar inactivos` gana `Click` para recargar.
- `CtlCliente.xaml.cs` — `BtnBuscar_Click`, `TxtBusqueda_TextChanged`, `TxtBusqueda_KeyDown` y `ChkMostrarInactivos_Click` delegan en el ViewModel.

### 2. Decisiones tomadas durante la implementación

| Decisión | Motivo |
|---|---|
| `IPaginacion` declara los comandos como `ICommand`, pero la clase base los expone como `RelayCommand` (implementación explícita de la interfaz) | El `RelayCommand` concreto es necesario para `RaiseCanExecuteChanged()`; se preserva el tipo público que ya usaba `ReporteGerencialViewModel`. |
| `BaseViewModel.IsLoading` pasa a `virtual` y se sobreescribe en la clase base paginada | `PuedeIrAnterior`/`PuedeIrSiguiente` dependen de `IsLoading`; sin el override los comandos no se refrescaban al terminar la carga. |
| El debounce vive en `Presentacion/Helpers/` y se dispara desde el code-behind (`TextChanged`), no desde el setter de la propiedad | Mantiene el setter del ViewModel libre de efectos sobre la base de datos y hace el comportamiento determinista y testeable. |
| `TextoBusqueda` se recorta en el ViewModel antes de enviarse al repositorio | Deja explícito el contrato ViewModel → repositorio; el repositorio también recorta. |
| El filtro de búsqueda se aplica dentro de la misma consulta paginada | El `TotalRegistros` corresponde al conjunto filtrado. |
| `ClienteViewModel.CargarPaginaAsync` captura las excepciones y deja el listado vacío | Mantiene el comportamiento previo de `LoadAsync`, que no propagaba fallos del repositorio. |

### 3. Verificación

- `dotnet build "TP_ControlVehicular.slnx"` → compilación correcta, 0 errores.
- `dotnet test "TP_ControlVehicular.slnx"` → **57 / 57 tests en verde** (39 previos + 18 nuevos):
  - `TP_ControlVehicular.Tests/ViewModels/PaginacionTests.cs` (10) — clamp del índice de página, `TotalPaginas` mínimo 1, normalización de `TamanoPagina`, texto de estado, comandos deshabilitados en los bordes.
  - `TP_ControlVehicular.Tests/ViewModels/ClientePaginacionTests.cs` (7) — carga de la primera página, propagación del total, envío del término de búsqueda, índice de página solicitado, reinicio a página 1 al cambiar filtro o tamaño de página, listado vacío sin resultados.

### 4. Pendiente de validación manual

- [ ] Abrir *Control de Clientes*: la grilla trae solo la primera página y la barra muestra `Página 1 de N (M registros)`.
- [ ] Escribir en el buscador: la grilla se actualiza solo a los ~500 ms; el botón `Buscar` y `Enter` la fuerzan de inmediato.
- [ ] Navegar con ⏮ ◀ ▶ ⏭; los botones de borde quedan deshabilitados.
- [ ] Cambiar el tamaño de página: vuelve a la página 1 y recarga.
- [ ] Marcar *Mostrar inactivos*: recarga el listado.
- [ ] Abrir *Reporte Gerencial* → Tab *Ingresos Totales*: la barra migrada se ve igual que antes y navega correctamente.

---

## Fase 2 — CRUD restantes (COMPLETADA)

Mismo patrón de la Fase 1, un commit por pantalla: Marca, Modelo, Vehículo, Rol, Usuario, Servicio y Taller.

### 1. Qué se construyó

**Capa de datos** — cada repositorio expone un método paginado con búsqueda server-side, y su `Listar*Handler` agrega `HandlePaginadoAsync(...)` que mapea la página a DTOs:

| Listado | Método del repositorio | Columnas de la búsqueda |
|---|---|---|
| Marca | `IMarcaRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `NombreMarca` |
| Modelo | `IModeloRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `NombreModelo`, `Marca.NombreMarca` |
| Vehículo | `IVehiculoRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `Patente`, `Modelo.NombreModelo`, `Modelo.Marca.NombreMarca`, nombre/apellido/DNI del propietario actual |
| Rol | `IRolRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` | `Nombre` |
| Usuario | `IUsuarioRepository.GetPaginadoAsync(busqueda, pagina, tamanoPagina)` | `Nombre`, `Apellido`, `Dni`, `Rol.Nombre` |
| Servicio | `IServicioRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` | `Nombre` |
| Taller | `ITallerRepository.GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` | `Nombre` |

**ViewModels** — los siete pasan a `BasePaginableViewModel` y eliminan su `ListadoXFiltered`. La búsqueda se ejecuta en la base de datos y se dispara con el botón **Buscar**, con **Enter** y con el debounce de 500 ms. Los setters de filtro (`TextoBusqueda`, `MostrarInactivos`) sólo actualizan estado y reinician la página a 1.

**Vistas** — cada `Ctl*.xaml` gana la fila de la barra `<compartido:CtlPaginacion Model="{Binding}"/>` entre la grilla y el pie de acciones, y la grilla se enlaza a la colección paginada en lugar del listado filtrado.

**Code-behind** — se agregan `BtnBuscar_Click`, `TxtBusqueda_TextChanged`, `TxtBusqueda_KeyDown` y `ChkMostrarInactivos_Click` (este último solo en las pantallas con check de inactivos).

### 2. Decisiones tomadas durante la implementación

| Decisión | Motivo |
|---|---|
| `RolRepository.GetPaginadoAsync` incluye `Permisos` | `FrmRolPermiso` (botón *Administrar Permisos*) lee `RolDto.Permisos` del rol seleccionado. Antes, con *Mostrar inactivos* activado el listado usaba `GetTodosAsync()` y los permisos venían vacíos. |
| `AsSplitQuery()` en Vehículo, Rol y Usuario | Los `Include` de colecciones (propietarios, permisos) generaban un JOIN 1:N que puede alterar el `Skip`/`Take`. Con split query se pagina primero y las colecciones se cargan después para los registros de la página. |
| El filtro de `Activo` va en el `configurador` de la consulta base, no en el `filtro` | La clase base aplica `configurador` → `filtro` → `CountAsync` → `orden`. El filtro de estado tiene que quedar dentro de la misma consulta que el conteo para que `TotalRegistros` sea consistente. |
| `ServicioViewModel` elimina su `IsLoading` propio | Ocultaba (`new`) el `IsLoading` virtual de `BaseViewModel`, con lo que `PuedeIrAnterior`/`PuedeIrSiguiente` nunca se refrescaban. |
| Se agrega el botón **Buscar** a las pantallas que no lo tenían | El requisito de la spec es que la búsqueda se dispare por una acción explícita, con Enter y con debounce. |
| Las altas/ediciones recargan la página en lugar de manipular la colección | Con paginación en BD, agregar el registro a la colección en memoria lo mostraría aunque no corresponda a la página ni al filtro activo. |

### 3. Verificación

- `dotnet build "TP_ControlVehicular.slnx"` → compilación correcta, 0 errores.
- `dotnet test "TP_ControlVehicular.slnx"` → **67 / 67 tests en verde** (57 previos + 10 nuevos):
  - `TP_ControlVehicular.Tests/ViewModels/ListadosCrudPaginacionTests.cs` — carga de la primera página y propagación del total en los siete listados, envío del término de búsqueda recortado, reinicio a página 1 al cambiar el filtro y propagación del flag de inactivos.

### 4. Pendiente de validación manual

- [ ] Abrir cada una de las 7 pantallas: la grilla trae una sola página y la barra informa `Página 1 de N (M registros)`.
- [ ] Buscar con el botón, con Enter y escribiendo (debounce).
- [ ] Cambiar el tamaño de página y navegar con ⏮ ◀ ▶ ⏭.
- [ ] Marcar *Mostrar inactivos* en Roles, Servicios y Talleres.
- [ ] En Roles: abrir *Administrar Permisos* sobre un rol y verificar que los permisos figuran marcados.
- [ ] En Vehículos: verificar que la columna de cliente, marca y modelo se sigue mostrando (el `Include` con `AsSplitQuery` cambia la forma de la consulta).

---

## Fase 3 — Transaccionales y reportes (COMPLETADA)

Fase de mayor riesgo: los listados transaccionales}y los reportes exponen proyecciones aplanadas que EF Core no puede traducir.

### 1. Qué se construyó

**SQL (`bd/03_procedimientos.sql`)** — tres procedimientos nuevos, validados contra la base real (`WALTERG20\SQLEXPRESS2019\ControlVehicular`):

| SP | Qué hace |
|---|---|
| `sp_ReporteOrdenesPaginado` | Reporte de órdenes aplanado. Usa `STRING_AGG` para *Servicios Aplicados* (el `string.Join` sobre colección del código C# no es traducible a SQL) y `OUTER APPLY` para el propietario actual, el mecánico y los servicios. |
| `sp_HistorialVehiculoPaginado` | Historial de un vehículo paginado. |
| `sp_VehiculosPorClientePaginado` | Vehículos de un cliente con conteo de servicios, paginado. |

Los tres usan **variables de tabla** en lugar de CTE: el conjunto filtrado se materializa una vez, el total se cuenta sobre esa tabla y la paginación se aplica con `OFFSET ... FETCH NEXT`. Con `@TamanoPagina = 0` devuelven el conjunto completo, que es lo que usa la exportación a PDF.

**Capa de datos**
- `IReporteRepository` / `ReporteRepository`: `ObtenerHistorialVehiculoPaginadoAsync`, `ObtenerVehiculosPorClientePaginadoAsync` y `ObtenerReporteOrdenesPaginadoAsync` (consumidos con `SqlQueryRaw("EXEC sp_...")`).
- `IRegistroServicioRepository` / `RegistroServicioRepository`: `GetPaginadoAsync` (para *Órdenes de Trabajo*) y `GetPaginadoPorMecanicoAsync` (para *Mis Trabajos*), ambas con EF Core y `AsSplitQuery`. Además `GetFiltradoAsync`, que devuelve el conjunto filtrado sin paginar para el PDF.
- `ListarRegistroServiciosHandler`: `HandlePaginadoAsync`, `HandlePaginadoPorMecanicoAsync` y `HandleCompletoAsync` (para el PDF).
- `ObtenerReporteOrdenesHandler`: `HandlePaginadoAsync` y `HandleCompletoAsync`; la proyección pasa a hacerse en SQL.

**ViewModels**
- `ReporteOrdenesViewModel`: paginado, la grilla se enlaza a `Ordenes` y el PDF exporta el conjunto completo filtrado.
- `ReportesViewModel`: **dos grillas con paginación independiente**. La del historial usa el estado de la propia clase; la de vehículos por cliente usa `PaginacionVehiculos`, una instancia de la nueva `PaginacionViewModel` (clase concreta que reutiliza `BasePaginableViewModel` con un delegado de carga).
- `OrdenServicioViewModel`: paginado con filtros de texto, estado y rango de fechas aplicados en SQL.
- `MisTrabajosViewModel`: el maestro pasa a la base de datos. Se elimina el `ICollectionView` con su filtro en memoria (`OrdenesFilter`); los filtros de estado, fechas y búsqueda se aplican en la consulta.

**Vistas** — cada pantalla suma la barra `CtlPaginacion`, el botón **Buscar** y los eventos de búsqueda (texto, `Enter`, `DatePicker`, `ComboBox`).

### 2. Bugs reales encontrados por las pruebas E2E

Las pruebas end-to-end contra la base real|elevationaron dos defectos **preexistentes** que impedían que el reporte de recepción funcionara:

1. **`sp_VehiculosPorCliente` devolvía `CantidadServicio` y el DTO espera `CantidadServicios`.** EF Core lanza `The required column 'CantidadServicios' was not present in the results of a 'FromSql' operation`. La pantalla estaba rota desde antes y no se notó porque nunca se abrió desde el menú.
2. **`sp_HistorialVehiculo` no devolvía la columna `Diagnostico`** que el DTO declara: mismo error de EF Core.

Ambos se corrigieron en los SP originales y en los nuevos paginados. La columna `Diagnostico` ahora devuelve las observaciones de la primera tarea de la orden.

**Tercer defecto (infraestructura de tests):** `DbProvisioner.Provision()` no recreaba la base `ControlVehicular_Test`. En la segunda corrida el seed (`02_datos.sql`) se re-aplicaba sobre una base que ya tenía los triggers activos y `TR_RegistroServicio_SincronizarKm` abortaba la siembra con *"El kilometraje de ingreso (69000) no puede ser menor al actual del vehículo (75000)"*. Ahora la base de pruebas se tira y se recrea en cada corrida. La infraestructura E2E existía pero nunca se había ejercido: no había ninguna clase de test E2E en el repositorio.

### 3. Verificación

- `dotnet build "TP_ControlVehicular.slnx"` → compilación correcta, 0 errores.
- `dotnet test "TP_ControlVehicular.slnx"` → **78 / 78 tests en verde** (67 previos + 11 E2E nuevos).
- `TP_ControlVehicular.Tests/EndToEnd/PaginacionBaseDeDatosTests.cs` (11) — valida contra la base real: paginación y total, páginas disjuntas, filtro por estado, búsqueda por patente, filtro por tareas del mecánico, los tres SP paginados y el modo sin paginar.
- Los tres SP se aplicaron y probaron también contra la base de desarrollo `ControlVehicular`.

### 4. Pendiente de validación manual

- [ ] *Órdenes de Trabajo*: la grilla trae una página; la columna **Recepción** ahora muestra el taller (antes salía vacía porque faltaba el `Include`); *Imprimir Listado* exporta el conjunto filtrado completo.
- [ ] *Reporte Órdenes*: el botón **Buscar** consulta con los filtros; *Imprimir PDF* exporta todo el conjunto filtrado, no solo la página.
- [ ] *Mis Trabajos*: la grilla maestra pagina; el filtro *Pendientes / Finalizados / Todos* y el rango de fechas se aplican en la base de datos; el detalle de tareas sigue funcionando y *Guardar Cambios* refresca la página actual.
- [ ] *Reportes de Recepción*: **la pantalla no está enlazada a ningún ítem del menú** (ver más abajo). Con el DataContext resuelto por DI debería funcionar si se agrega el acceso.

---

## Observaciones de la Fase 3

| Observación | Detalle |
|---|---|
| Pantallas huérfanas | `CtlReportesRecepcion` y `Pantalla/Taller/CtlMisTrabajos` (`grdTrabajos`) no están enlazadas a ningún ítem del menú de `MainWindow` ni registradas en el DI. La primera se migró igual porque estaba en el alcance de la spec; se le agregó la resolución del `DataContext` por DI para que funcione si se la expone. |
| Filtro por número de orden | `r.Id.ToString().Contains(...)` no es traducible a SQL. En *Mis Trabajos* el término numérico se compara como entero (`r.Id == n`); el resto de los términos buscan por texto en las columnas. |
| Busqueda por servicio | *Órdenes de Trabajo* filtra por patente, modelo, marca, recepcionista, taller, estado y datos del propietario. No incluye el nombre de los servicios (en *Reporte Órdenes* sí, vía `EXISTS` sobre `DetalleServicio`). |
| `TallerNombre` en *Órdenes de Trabajo* | Ahora se puebla: el `Include(r => r.Taller)` que faltaba en `GetReporteCompletoAsync` era también un defecto previo. |

---

## Complemento — Paginación de las pestañas del Reporte Gerencial

Las pestañas **Top Clientes**, **Modelos Más Reparados** y **Servicios por Rango** del Reporte Gerencial se habían quedado fuera: la barra del Gerencial solo cubría la pestaña *Ingresos Totales*. La razón era que sus procedimientosLIMITaban el resultado con `TOP (@TopN) = 10`, así que nunca había más de una página que paginar.

### Qué se hizo

- **SQL**: tres procedimientos nuevos en `bd/03_procedimientos.sql` — `sp_ReporteTopClientesPaginado`, `sp_ReporteModelosReparadosPaginado` y `sp_ReporteServiciosPorRangoPaginado`. A diferencia de los originales, **no aplican el tope `TOP`**: devuelven el ranking completo y lo paginan con `OFFSET ... FETCH NEXT`. Con `@TamanoPagina = 0` devuelven el conjunto completo (lo usan los PDF).
  Los procedimientos originales (`sp_ReporteTopClientes`, `sp_ReporteModelosReparados`) se mantienen sin cambios porque `ReporteOperativoViewModel` los sigue usando con `@TopN = 10` para el ranking de vehículos frecuentes.
- **ViewModel**: `ReporteGerencialViewModel` expone tres estados de paginación independientes — `PaginacionTopClientes`, `PaginacionModelos` y `PaginacionServicios` (instancias de `PaginacionViewModel`), cada uno con su delegado de carga. `LoadAsync` carga la primera página de cada pestaña.
- **Vista**: las tres pestañas ganan una tercera fila con `<compartido:CtlPaginacion Model="{Binding Paginacion...}"/>`.
- **PDF**: las tres exportaciones ahora piden el conjunto completo al procedimiento (`@TamanoPagina = 0`), de modo que el documento no se limita a la página visible.

### Consecuencia a tener en cuenta

Las pestañas antes mostraban como máximo 10 filas; ahora muestran el **ranking completo paginado** (con los datos de ejemplo: 55 clientes, 43 modelos y 13 servicios). El PDF correspondiente también pasó de 10 filas al ranking completo, en línea con la regla de que los PDF exportan el conjunto filtrado completo.

### Verificación

- `dotnet build` → 0 errores. `dotnet test` → **87 / 87** (81 previos + 6 E2E nuevos: `ReporteGerencialPaginacionTests`).
- Los tres procedimientos se probaron además con `sqlcmd` contra la base de desarrollo: Top Clientes 55 registros, Modelos 43, Servicios 13.

---

## Fases 2 y 3 (pendientes)

### Fase 2 — CRUD restantes

Mismo patrón, un commit por pantalla: Marca, Modelo, Vehículo, Rol, Usuario, Servicio, Taller.
Requiere en cada caso: método paginado en el repositorio + interfaz, `ViewModel` paginado, `Ctl*.xaml` con `CtlPaginacion` y eventos de búsqueda, y tests unitarios del predicado de búsqueda.

### Fase 3 — Transaccionales y reportes

Requiere SQL nuevo en `bd/03_procedimientos.sql`:

- `sp_ReporteOrdenesPaginado` — proyección aplanada de órdenes (`STRING_AGG` para *Servicios Aplicados*, que no es traducible a SQL por EF Core), filtros de texto, estado, rango de fechas y mecánico.
- `sp_HistorialVehiculoPaginado` y `sp_VehiculosPorClientePaginado` — versión paginada de los SP existentes.

Se consumen con el patrón ya establecido `SqlQueryRaw("EXEC sp_...")` de `ReporteRepository`.

Además: `OrdenServicio` y el maestro de `MisTrabajos` pasan a paginar sobre `RegistroServicio` con `Include(r => r.Taller)` (hoy `TallerNombre` sale vacío en la grilla por falta de ese `Include` — se corrige de paso), y los PDF de reporte siguen exportando el conjunto completo filtrado.

---

## Notas de proceso

- El `read`/`edit` de este entorno reescribe el namespace `http://schemas.openxmlformats.org/markup-compatibility/2006` como `http://schemas.opencode.openxmlformats.org/...`. Eso rompió la compilación de los XAML nuevos (`MC3072`). Se corrigió a nivel de bytes con `docs/script/corregir-namespace-markup-compatibility.py`. **A futuro: al escribir XAML, usar entidades XML o el namespace correcto verificado con `ReadAllLines`.**
- El repositorio trabaja con `core.autocrlf=true`; los archivos se normalizaron a CRLF antes de commitear.
