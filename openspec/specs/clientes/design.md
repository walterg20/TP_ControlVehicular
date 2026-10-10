# 001 · Clientes — Plan

_Cómo se implementa lo descrito en `spec.md`. Debe respetar la `constitution/`._

## Enfoque

La feature gestiona el CRUD completo de clientes (registrar, listar, editar, eliminar) usando el patrón handler-repositorio-MVVM ya establecido en el proyecto. Se inyecta `IClienteRepository` en `RegistrarClienteHandler` y `ClienteViewModel`, con AutoMapper Entity→DTO unidireccional. La UI usa `CtlCliente` (UserControl modal) con `Owner = Window.GetWindow(this)` y `DataContext` inyectado vía DI. La navegación y validación se centralizan en `BaseViewModel` (`INotifyPropertyChanged`, `RelayCommand` async).

## Implementación

_Pasos técnicos concretos, en orden. Indica los archivos/módulos que se tocan._

1. Verificar entidad `Entidad/Cliente.cs`: propiedades `string` = `string.Empty`, navegaciones `Vehiculos` = `new List<Vehiculo>()`, `null!` en referencias.
2. Verificar DTO `Negocio/DTOs/ClienteDto.cs` (nombres compuestos) y perfil AutoMapper `Negocio/Mappers/MappingProfile.cs` → `CreateMap<Cliente, ClienteDto>()`.
3. Verificar `Datos/Repositories/IClienteRepository.cs` + `ClienteRepository.cs` (hereda `Repository<Cliente>`) con `AddAsync`, `GetByIdAsync`, `GetAllAsync`, `UpdateAsync`, `DeleteAsync`.
4. Verificar handler `Negocio/Services/RegistrarClienteHandler.cs`: inyecta `IClienteRepository` + `IMapper`; `HandleAsync` retorna `_mapper.Map<ClienteDto>(entidad)`.
4. Verificar `Presentacion/ViewModels/ClienteViewModel.cs`: hereda `BaseViewModel`, `ObservableCollection<ClienteDto>`, `RelayCommand` async para cargar/guardar/eliminar, `OnPropertyChanged()` en setters, campos `private readonly IClienteRepository _repo` + `IMapper _mapper`.
5. Verificar `Presentacion/Pantalla/Cliente/CtlCliente.xaml.cs`: modal `Owner = Window.GetWindow(this)`, `DataContext` inyectado, eventos de cerrar/guardar.
6. Confirmar registro DI en `App.xaml.cs`: `AddScoped<IClienteRepository, ClienteRepository>()`, `AddScoped<RegistrarClienteHandler>()`, `AddScoped<ClienteViewModel>()`, `AddScoped<CtlCliente>()`.
7. Validar migración aplicada: `Migracion/20260827020907_InitialCreate.cs` incluye tabla `Clientes` con PK `ClienteId`, columnas requeridas y FK a `Vehiculos`.

## Decisiones

_Elecciones de diseño relevantes y su justificación. Alternativas descartadas y por qué._

- **Handler + Repository + AutoMapper** — Separación de responsabilidades: lógica de negocio en handler, acceso a datos en repo, mapeo en perfil; evita acoplar VM a EF Core.
- **AutoMapper unidireccional Entity→DTO** — Simplifica y evita ciclos; DTOs planos para UI. No se mapea DTO→Entity (se construye entidad en handler/VM).
- **`ClienteViewModel` hereda `BaseViewModel`** — Reutiliza `INotifyPropertyChanged`, `SetProperty<T>`, `RelayCommand` async. `VehiculoViewModel` es la excepción (no hereda).
- **Modal con `Owner = Window.GetWindow(this)`** — Garantiza ventana padre correcta en WPF; evita ventanas flotantes sin dueño.
- **DI `AddScoped` para repos/handlers/VMs** — Ciclo de vida por request/operación; `MainWindow` singleton. Coherente con `App.xaml.cs` actual.

## Riesgos

_Qué puede salir mal o requerir cuidado, y cómo se mitiga._

- **Validación de duplicados (DNI/Email)** — Mitigación: chequeo en `RegistrarClienteHandler.HandleAsync` antes de `AddAsync`; lanzar excepción de dominio clara.
- **Navegación `Vehiculos` cargada en exceso** — Mitigación: `Include` selectivo en repo; DTO no expone lista completa salvo que `ListarVehiculosPorClienteHandler` la requiera.
- **Concurrencia en edición** — Mitigación: `RowVersion` / `ConcurrencyToken` en entidad si aplica; manejar `DbUpdateConcurrencyException` en repo.
- **Rutas con espacios/caracteres no ASCII** — Mitigación: siempre usar comillas en comandos PowerShell (`dotnet build "TP_ControlVehicular.slnx"`); no usar `&&`.