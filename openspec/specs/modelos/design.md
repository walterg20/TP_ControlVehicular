# 002 · Modelos — Plan

_Cómo se implementa lo descrito en `spec.md`. Debe respetar la `constitution/`._

## Enfoque

Gestión de modelos de vehículos (p. ej. "Corolla", "Hilux", "208") vinculados a una `Marca`. Usa el mismo patrón handler-repositorio-MVVM: `IModeloRepository` + `ModeloRepository`, `ModeloViewModel` (hereda `BaseViewModel`), DTO `ModeloDto` mapeado por AutoMapper. No hay handler dedicado de escritura (registro simple); la lógica reside en `ModeloViewModel` que invoca repo directamente. UI: UserControl modal para alta/edición, grilla en vista principal.

## Implementación

_Pasos técnicos concretos, en orden. Indica los archivos/módulos que se tocan._

1. Verificar entidad `Entidad/Modelo.cs`: `ModeloId` (PK), `Nombre` (`string.Empty`), `MarcaId` (FK), `Marca` nav `null!`, `Vehiculos` nav `new List<Vehiculo>()`.
2. Verificar DTO `Negocio/DTOs/ModeloDto.cs` y `MappingProfile.cs` → `CreateMap<Modelo, ModeloDto>()`.
3. Verificar `Datos/Repositories/IModeloRepository.cs` + `ModeloRepository.cs` (hereda `Repository<Modelo>`) con CRUD + `GetByMarcaAsync(int marcaId)`.
3. Verificar `Presentacion/ViewModels/ModeloViewModel.cs`: hereda `BaseViewModel`, `ObservableCollection<ModeloDto>`, `RelayCommand` async, `private readonly IModeloRepository _repo`, `private readonly IMarcaRepository _marcaRepo` (para combo marcas), `private readonly IMapper _mapper`.
4. Verificar UserControl modal para modelos (patrón `Owner = Window.GetWindow(this)`, `DataContext` inyectado).
5. Confirmar registro DI en `App.xaml.cs`: `AddScoped<IModeloRepository, ModeloRepository>()`, `AddScoped<ModeloViewModel>()`, `AddScoped<CtlModelo>()` (si existe).
6. Validar migración aplicada: tabla `Modelos` con PK `ModeloId`, FK `MarcaId` a `Marcas`, índice en `Nombre`.

## Decisiones

_Elecciones de diseño relevantes y su justificación. Alternativas descartadas y por qué._

- **Sin handler de escritura dedicado** — `ModeloViewModel` llama a repo directamente (patrón aceptado en código actual, como `VehiculoViewModel`). Simplifica para CRUD simple sin reglas de negocio complejas.
- **Combo de marcas en VM** — Inyecta `IMarcaRepository` para cargar `ObservableCollection<MarcaDto>` y bindear en modal; evita hardcodeo.
- **AutoMapper Entity→DTO** — Consistente con resto del proyecto; `ModeloDto` expone `MarcaNombre` (string compuesto) para UI.
- **FK `MarcaId` requerida** — Un modelo no existe sin marca; validación en VM y en BD (NOT NULL).

## Riesgos

_Qué puede salir mal o requerir cuidado, y cómo se mitiga._

- **Borrar marca con modelos asociados** — Mitigación: FK con `ON DELETE RESTRICT` en migración; repo/VM valida antes de eliminar marca.
- **Nombres de modelo duplicados dentro de una marca** — Mitigación: índice único compuesto `(MarcaId, Nombre)` en BD + chequeo en VM antes de guardar.
- **Carga de `Vehiculos` innecesaria** — Mitigación: repo no hace `Include` por defecto; DTO no expone lista.