# Tasks: Baja Lógica Consistente — Cliente, Taller y Servicio

- [x] 1. Preparación
  - [x] 1.1 Change OpenSpec (`proposal.md`, `specs/baja-logica/spec.md`, `design.md`, `tasks.md`).
  - [x] 1.2 Bitácora `docs/plan/` (opcional).

- [x] 2. Entidades y BD
  - [x] 2.1 `Entidad/Cliente.cs`: `Activo = true` por defecto.
  - [x] 2.2 `Entidad/Taller.cs`: `Activo = true` por defecto.
  - [x] 2.3 `bd/01_estructura.sql`: `[Activo] bit NOT NULL DEFAULT (1)` en `Cliente`, `Taller`, `Servicio`.

- [x] 3. Repositorio base
  - [x] 3.1 `Negocio/Interfaces/IRepository.cs`: agregar `GetTodosAsync()`.
  - [x] 3.2 `Datos/Repositories/Repository.cs`: implementar `GetTodosAsync()`.

- [x] 4. Handlers
  - [x] 4.1 `ListarServiciosHandler.HandleAsync(bool incluirInactivos = false)`.
  - [x] 4.2 `ListarTallerHandler.HandleAsync(bool incluirInactivos = false)`.
  - [x] 4.3 Nuevo `ReactivarServicioHandler`.
  - [x] 4.4 Nuevo `ReactivarTallerHandler`.
  - [x] 4.5 `App.xaml.cs`: registrar los nuevos handlers.

- [x] 5. ViewModels
  - [x] 5.1 `ClienteViewModel`: `MostrarInactivos`, `LoadAsync` ramificado, `ReactivarClienteAsync`.
  - [x] 5.2 `TallerViewModel`: `MostrarInactivos`, `LoadAsync` ramificado, `ReactivarAsync`, inyectar `ReactivarTallerHandler`.
  - [x] 5.3 `ServicioViewModel`: `MostrarInactivos`, `LoadAsync` ramificado, `ReactivarAsync` (vía handler), inyectar `ReactivarServicioHandler`.

- [x] 6. UI
  - [x] 6.1 `CtlCliente.xaml`/`.xaml.cs`: checkbox "Mostrar inactivos" + botón "Reactivar".
  - [x] 6.2 `CtlTaller.xaml`/`.xaml.cs`: checkbox + botón "Reactivar".
  - [x] 6.3 `CtlServicio.xaml`/`.xaml.cs`: checkbox + botón "Reactivar".

- [x] 7. Cierre
  - [x] 7.1 `dotnet build "TP_ControlVehicular.slnx"` sin errores.
  - [x] 7.2 `dotnet test "TP_ControlVehicular.slnx"` sin regresiones.
  - [x] 7.3 Actualizar `MEMORY.md` y commitear en la rama feature.
  - [x] 7.4 Merge `--no-ff` a `develop` y archivar el change.
