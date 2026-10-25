# 003 · Marcas — Plan

_Cómo se implementa lo descrito en `spec.md`. Debe respetar la `constitution/`._

## Enfoque

Gestión de marcas de vehículos (p. ej. "Toyota", "Ford", "Peugeot"). Entidad simple: solo `MarcaId` (PK) y `Nombre` (`string.Empty`, único). Mismo patrón: `IMarcaRepository` + `MarcaRepository`, `MarcaViewModel` (hereda `BaseViewModel`), DTO `MarcaDto` mapeado por AutoMapper. Sin handler dedicado; VM usa repo directamente. UI: modal alta/edición, grilla principal.

## Implementación

_Pasos técnicos concretos, en orden. Indica los archivos/módulos que se tocan._

1. Verificar entidad `Entidad/Marca.cs`: `MarcaId` (PK), `Nombre` (`string.Empty`), `Modelos` nav `new List<Modelo>()`.
2. Verificar DTO `Negocio/DTOs/MarcaDto.cs` y `MappingProfile.cs` → `CreateMap<Marca, MarcaDto>()`.
3. Verificar `Datos/Repositories/IMarcaRepository.cs` + `MarcaRepository.cs` (hereda `Repository<Marca>`) con CRUD + `GetByNombreAsync(string nombre)`.
4. Verificar `Presentacion/ViewModels/MarcaViewModel.cs`: hereda `BaseViewModel`, `ObservableCollection<MarcaDto>`, `RelayCommand` async, `private readonly IMarcaRepository _repo`, `private readonly IMapper _mapper`.
5. Verificar UserControl modal para marcas (patrón `Owner = Window.GetWindow(this)`, `DataContext` inyectado).
6. Confirmar registro DI en `App.xaml.cs`: `AddScoped<IMarcaRepository, MarcaRepository>()`, `AddScoped<MarcaViewModel>()`, `AddScoped<CtlMarca>()` (si existe).
7. Validar migración aplicada: tabla `Marcas` con PK `MarcaId`, columna `Nombre` unique not null.

## Decisiones

_Elecciones de diseño relevantes y su justificación. Alternativas descartadas y por qué._

- **Entidad mínima (solo Nombre)** — Cumple YAGNI; atributos extra (país origen, logo, web) se añadirán si el negocio lo pide.
- **Índice único en `Nombre`** — Evita duplicados ("Toyota" vs "TOYOTA"); validación en BD + chequeo en VM (`GetByNombreAsync`).
- **Sin handler de escritura** — CRUD simple sin reglas complejas; `MarcaViewModel` llama a repo (patrón aceptado, ver `VehiculoViewModel`).
- **AutoMapper Entity→DTO** — `MarcaDto` plano para UI; no expone `Modelos` (carga perezosa si hace falta en otra feature).

## Riesgos

_Qué puede salir mal o requerir cuidado, y cómo se mitiga._

- **Borrar marca con modelos asociados** — Mitigación: FK `Modelos.MarcaId` con `ON DELETE RESTRICT`; repo/VM valida `Modelos.Any()` antes de `DeleteAsync`.
- **Case-insensitive en unicidad** — Mitigación: colación BD case-insensitive (default SQL Server) + `ToLower()` en chequeo VM.
- **Referencias circulares AutoMapper** — Mitigación: perfil unidireccional; `MarcaDto` no tiene `List<ModeloDto>`.