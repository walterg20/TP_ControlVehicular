# 004 · Vehículos — Plan

_Cómo se implementa lo descrito en `spec.md`. Debe respetar la `constitution/`._

## Enfoque

Gestión completa de vehículos: registro (patente, año, color, cliente, modelo), listado general, listado por cliente, edición y eliminación. Usa dos handlers: `RegistrarVehiculoHandler` (escritura, valida patente única, cliente y modelo existen) y `ListarVehiculosPorClienteHandler` (lectura, filtra por `ClienteId`). `VehiculoViewModel` es la excepción del proyecto: **no hereda `BaseViewModel`**, construye `Entidad.Vehiculo` directo y usa `IVehiculoRepository` + `IClienteRepository` + `IModeloRepository` inyectados. DTOs: `VehiculoDto` (con `ClienteNombre`, `ModeloNombre`, `MarcaNombre` compuestos), `VehiculoPorClienteDto`. AutoMapper perfil unidireccional. UI: `CtlVehiculo` modal para alta/edición, grilla principal con filtros.

## Implementación

_Pasos técnicos concretos, en orden. Indica los archivos/módulos que se tocan._

1. Verificar entidad `Entidad/Vehiculo.cs`: `VehiculoId` (PK), `Patente` (`string.Empty`), `Anio` (int), `Color` (`string.Empty`), `ClienteId` (FK), `Cliente` nav `null!`, `ModeloId` (FK), `Modelo` nav `null!`, sin colecciones de navegación propia.
2. Verificar DTOs `Negocio/DTOs/VehiculoDto.cs`, `VehiculoPorClienteDto.cs` y `MappingProfile.cs` → `CreateMap<Vehiculo, VehiculoDto>()` (incluye `ClienteNombre`, `ModeloNombre`, `MarcaNombre` via `Modelo.Marca.Nombre`), `CreateMap<Vehiculo, VehiculoPorClienteDto>()`.
3. Verificar `Datos/Repositories/IVehiculoRepository.cs` + `VehiculoRepository.cs` (hereda `Repository<Vehiculo>`) con CRUD + `GetByPatenteAsync(string patente)`, `GetByClienteAsync(int clienteId)`, `GetWithDetailsAsync(int id)` (Include Cliente + Modelo + Marca).
4. Verificar `Negocio/Services/RegistrarVehiculoHandler.cs`: inyecta `IVehiculoRepository` + `IClienteRepository` + `IModeloRepository` + `IMapper`; `HandleAsync` valida patente única, cliente existe, modelo existe; crea entidad, `AddAsync`, retorna `_mapper.Map<VehiculoDto>(entidad)`.
5. Verificar `Negocio/Services/ListarVehiculosPorClienteHandler.cs`: inyecta `IVehiculoRepository` + `IMapper`; `HandleAsync(int clienteId)` llama `GetByClienteAsync` y mapea a `List<VehiculoPorClienteDto>`.
6. Verificar `Presentacion/ViewModels/VehiculoViewModel.cs`: **no hereda `BaseViewModel`**; campos `private readonly IVehiculoRepository _vehiculoRepo`, `IClienteRepository _clienteRepo`, `IModeloRepository _modeloRepo`; propiedades planas para binding (Patente, Anio, Color, ClienteSeleccionado, ModeloSeleccionado); `RelayCommand` async manual; `ObservableCollection<VehiculoDto>` para grilla; combos Cliente/Modelo poblados desde repos.
7. Verificar `Presentacion/Pantalla/Vehiculo/CtlVehiculo.xaml.cs`: modal `Owner = Window.GetWindow(this)`, `DataContext` inyectado, combos bindeados.
8. Confirmar registro DI en `App.xaml.cs`: `AddScoped<IVehiculoRepository, VehiculoRepository>()`, `AddScoped<RegistrarVehiculoHandler>()`, `AddScoped<ListarVehiculosPorClienteHandler>()`, `AddScoped<VehiculoViewModel>()`, `AddScoped<CtlVehiculo>()`.
9. Validar migración aplicada: tabla `Vehiculos` con PK `VehiculoId`, FK `ClienteId` (RESTRICT), FK `ModeloId` (RESTRICT), índice único en `Patente`, `Anio` not null.

## Decisiones

_Elecciones de diseño relevantes y su justificación. Alternativas descartadas y por qué._

- **Dos handlers (escritura + lectura por cliente)** — Separación CQRS ligera: `RegistrarVehiculoHandler` valida reglas de negocio (patente única, FKs existen); `ListarVehiculosPorClienteHandler` optimiza consulta filtrada sin cargar grafo completo.
- **`VehiculoViewModel` no hereda `BaseViewModel`** — Excepción documentada en AGENTS.md; construye entidad directo, usa `RelayCommand` manual, propiedades planas sin `SetProperty`. Mantiene simplicidad para VM con mucha lógica de combos/cruce de repos.
- **Patente como índice único** — Requisito legal/operativo; validación en handler + BD.
- **DTOs con nombres compuestos (`ClienteNombre`, `ModeloNombre`, `MarcaNombre`)** — Evita joins en UI; AutoMapper resuelve navegación `Modelo.Marca.Nombre` en perfil.
- **Combos Cliente/Modelo en VM** — Inyecta `IClienteRepository` + `IModeloRepository` para `ObservableCollection<ClienteDto>` / `ModeloDto`; `DisplayMemberPath` = "NombreCompleto" / "NombreConMarca".

## Riesgos

_Qué puede salir mal o requerir cuidado, y cómo se mitiga._

- **Patente duplicada (case-insensitive, formato variable)** — Mitigación: normalizar en handler (`patente.Replace(" ", "").ToUpper()`) antes de `GetByPatenteAsync` y `AddAsync`; índice único BD.
- **Eliminar cliente/modelo con vehículos** — Mitigación: FK `ON DELETE RESTRICT`; handlers/repo validan existencia antes de borrar; UI deshabilita borrado si hay vehículos.
- **Carga de `Marca` vía `Modelo.Marca` en `GetWithDetailsAsync`** — Mitigación: `Include(v => v.Modelo).ThenInclude(m => m.Marca)` selectivo; solo en handler de detalle/edición.
- **`VehiculoViewModel` sin `INotifyPropertyChanged`** — Mitigación: propiedades son campos planos + `OnPropertyChanged` manual en setters; binding WPF funciona. Documentar excepción para futuros mantenedores.