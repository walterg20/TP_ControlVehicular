# 003 · Marcas — Tareas

_Checklist accionable derivada del `plan.md`. Tareas pequeñas y concretas; marca `[x]` al completarlas._

- [x] Verificar entidad `Marca.cs` (convenciones: string.Empty, Modelos List).
- [x] Verificar DTO `MarcaDto.cs` y mapeo en `MappingProfile.cs`.
- [x] Verificar `IMarcaRepository` + `MarcaRepository` (CRUD + GetByNombreAsync).
- [x] Verificar `MarcaViewModel` (hereda BaseViewModel, ObservableCollection<MarcaDto>, IMapper).
- [x] Verificar UserControl modal marcas (Owner = Window.GetWindow(this), DataContext inyectado).
- [x] Verificar registro DI en `App.xaml.cs` (AddScoped IMarcaRepository, MarcaViewModel, CtlMarca).
- [x] Verificar migración: tabla Marcas con PK, Nombre unique not null.
- [x] Probar alta marca válida (persistencia + DTO).
- [x] Probar validación nombre duplicado case-insensitive (error controlado).
- [x] Probar validación nombre vacío.
- [x] Probar listado, edición, eliminación (sin modelos).
- [x] Probar eliminación con modelos asociados (error referencial).
- [x] Validar contra los criterios de aceptación de `spec.md`.
- [x] Mover la feature a "Hecho" en `../../constitution/roadmap.md`.

## Mantenimiento (checklist recurrente)

_Opcional. Pasos a repetir cada vez que se toque esta feature en el futuro (revisar datos, regenerar algo, etc.). Borra esta sección si no aplica._

- [ ] Revisar índice único en Nombre tras cambios de volumen.
- [ ] Confirmar que `MarcaViewModel` no expone `Modelos` en DTO.
- [ ] Validar que colación BD mantiene case-insensitive en unicidad.