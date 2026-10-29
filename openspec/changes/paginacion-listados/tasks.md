# Tasks

## Phase 1 — Infrastructure and pilot (low risk)

- [x] 1.1 Create `Presentacion/ViewModels/IPaginacion.cs` with the pagination contract (`PaginaActual`, `TotalPaginas`, `TotalRegistros`, `TamanoPagina`, `OpcionesTamanoPagina`, `TextoPaginacion`, `PuedeIrAnterior`, `PuedeIrSiguiente`, the four navigation commands and `CambiarPaginaAsync`), deriving from `INotifyPropertyChanged`.
- [x] 1.2 Create `Presentacion/ViewModels/BasePaginableViewModel.cs` (`BaseViewModel` + `IPaginacion`) with page clamping, total-page calculation, page-size reset to page 1, command state notification and the abstract `CargarPaginaAsync()` hook.
- [x] 1.3 Create `Presentacion/Pantalla/Compartido/CtlPaginacion.xaml` and `.xaml.cs` reusing the exact markup of the current Managerial Report toolbar, exposing a `Model` dependency property of type `IPaginacion`.
- [x] 1.4 Create `Presentacion/Helpers/Debouncer.cs` (500 ms, `DispatcherTimer`) to coalesce server-side search reloads.
- [x] 1.5 Migrate `ReporteGerencialViewModel` to inherit `BasePaginableViewModel`, removing the duplicated pagination state while keeping every public member name and signature.
- [x] 1.6 Replace the inline pagination toolbar of `CtlReporteGerencial.xaml` with `<c:CtlPaginacion Model="{Binding}" />`.
- [x] 1.7 Add `GetPaginadoAsync(...)` to `Datos/Repositories/Repository.cs` (`Skip`/`Take` + `CountAsync`).
- [x] 1.8 Add `GetPaginadoAsync(busqueda, incluirInactivos, pagina, tamanoPagina)` to `IClienteRepository` and implement it in `Datos/Repositories/ClienteRepository.cs` with server-side `LIKE` predicates over `Nombre`, `Apellido` and `Dni`.
- [x] 1.9 Convert `ClienteViewModel` to `BasePaginableViewModel`: server-side search, `CargarPaginaAsync()` implementation, `Buscar` command, `Enter` key handling, debounce, reset to page 1 on filter change, and removal of the in-memory `ListadoClientesFiltered` filter.
- [x] 1.10 Update `CtlCliente.xaml`: bind the grid to the paginated collection, add `<c:CtlPaginacion Model="{Binding}" />`, wire the `Buscar` command and the `Enter` key.
- [x] 1.11 Add unit tests in `TP_ControlVehicular.Tests/ViewModels/PaginacionTests.cs` covering page clamping, total-page calculation, page-size reset and the navigation command states.
- [x] 1.12 Add unit tests in `TP_ControlVehicular.Tests/ViewModels/ClientePaginacionTests.cs` covering the pilot: server-side search invocation, page reset on filter change and total-record propagation.
- [x] 1.13 Run `dotnet build "TP_ControlVehicular.slnx"` and `dotnet test "TP_ControlVehicular.slnx"` and ensure a clean build with all tests passing.

## Phase 2 — Remaining CRUD listings (low risk, same proven pattern)

- [ ] 2.1 Marca listing: `IMarcaRepository.GetPaginadoAsync`, `MarcaViewModel` and `CtlMarca.xaml`.
- [ ] 2.2 Modelo listing: `IModeloRepository.GetPaginadoAsync`, `ModeloViewModel` and `CtlModelo.xaml`.
- [ ] 2.3 Vehículo listing: `IVehiculoRepository.GetPaginadoAsync`, `VehiculoViewModel` and `CtlVehiculo.xaml`.
- [ ] 2.4 Rol listing: `IRolRepository.GetPaginadoAsync`, `RolViewModel` and `CtlRol.xaml`.
- [ ] 2.5 Usuario listing: `IUsuarioRepository.GetPaginadoAsync`, `UsuarioViewModel` and `CtlUsuario.xaml`.
- [ ] 2.6 Servicio listing: `IServicioRepository.GetPaginadoAsync`, `ServicioViewModel` and `CtlServicio.xaml`.
- [ ] 2.7 Taller listing: `ITallerRepository.GetPaginadoAsync`, `TallerViewModel` and `CtlTaller.xaml`.
- [ ] 2.8 Add unit tests covering the paginated search predicate and page reset for each migrated listing.
- [ ] 2.9 Run `dotnet build "TP_ControlVehicular.slnx"` and `dotnet test "TP_ControlVehicular.slnx"`.

## Phase 3 — Transactional listings and reports (high risk: new SQL)

- [ ] 3.1 Add `sp_ReporteOrdenesPaginado` to `bd/03_procedimientos.sql` (CTE + `CROSS JOIN` of `COUNT(*)`, `STRING_AGG` for `ServiciosAplicados`, filters for text, status, date range and mechanic).
- [ ] 3.2 Add `sp_HistorialVehiculoPaginado` and `sp_VehiculosPorClientePaginado` to `bd/03_procedimientos.sql`.
- [ ] 3.3 Add the paginated methods to `IReporteRepository` / `ReporteRepository` and to `IRegistroServicioRepository` / `RegistroServicioRepository`.
- [ ] 3.4 Migrate `ReporteOrdenesViewModel` and `CtlReporteOrdenes.xaml` to database pagination, keeping the PDF export on a complete un-paginated query.
- [ ] 3.5 Migrate `ReportesViewModel` and `CtlReportesRecepcion.xaml` (both grids: vehicle history and vehicles per client) to database pagination.
- [ ] 3.6 Migrate `OrdenServicioViewModel` and `CtlOrdenServicio.xaml`: paginated `RegistroServicio` query with `Include(r => r.Taller)`, server-side filters for date range, status and text.
- [ ] 3.7 Migrate the `MisTrabajos` master grid (`MisTrabajosViewModel` / `CtlMisTrabajos.xaml`) to database pagination, keeping the task detail grid scoped to the selected order.
- [ ] 3.8 Verify that the OrdenServicio "Imprimir Listado" PDF still exports the complete filtered set.
- [ ] 3.9 Add unit tests for the migrated listings and the report repository pagination.
- [ ] 3.10 Run `dotnet build "TP_ControlVehicular.slnx"` and `dotnet test "TP_ControlVehicular.slnx"`.

## Closure

- [ ] C.1 Write the execution plan/log to `docs/plan/2026-10-29_paginacion-listados.md`.
- [ ] C.2 Update `MEMORY.md` with the new pagination conventions, the new stored procedures and the new files.
- [ ] C.3 Update `AGENTS.md` with the pagination conventions and correct the stale `VehiculoViewModel` inheritance note.
- [ ] C.4 Merge `feature/paginacion-listados` into `develop` and archive the change under `openspec/changes/archive/`.
