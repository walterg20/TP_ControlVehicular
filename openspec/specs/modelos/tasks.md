# 002 · Modelos — Tareas

_Checklist accionable derivada del `plan.md`. Tareas pequeñas y concretas; marca `[x]` al completarlas._

- [x] Verificar entidad `Modelo.cs` (convenciones: string.Empty, Marca null!, Vehiculos List).
- [x] Verificar DTO `ModeloDto.cs` y mapeo en `MappingProfile.cs` (incluye MarcaNombre).
- [x] Verificar `IModeloRepository` + `ModeloRepository` (CRUD + GetByMarcaAsync).
- [x] Verificar `ModeloViewModel` (hereda BaseViewModel, ObservableCollection<ModeloDto>, IMarcaRepository para combo, IMapper).
- [x] Verificar UserControl modal modelos (Owner = Window.GetWindow(this), DataContext inyectado, combo marcas).
- [x] Verificar registro DI en `App.xaml.cs` (AddScoped IModeloRepository, ModeloViewModel, CtlModelo).
- [x] Verificar migración: tabla Modelos con PK, FK MarcaId, índice único (MarcaId, Nombre).
- [x] Probar alta modelo válido (persistencia + DTO con MarcaNombre).
- [x] Probar validación nombre duplicado por marca (error controlado).
- [x] Probar validación marca obligatoria.
- [x] Probar listado, edición, eliminación (sin vehículos).
- [x] Probar eliminación con vehículos asociados (error referencial).
- [x] Validar contra los criterios de aceptación de `spec.md`.
- [x] Mover la feature a "Hecho" en `../../constitution/roadmap.md`.

## Mantenimiento (checklist recurrente)

_Opcional. Pasos a repetir cada vez que se toque esta feature en el futuro (revisar datos, regenerar algo, etc.). Borra esta sección si no aplica._

- [ ] Revisar índice único (MarcaId, Nombre) tras cambios de volumen.
- [ ] Confirmar que `ModeloViewModel` inyecta `IMarcaRepository` y no usa servicio estático.
- [ ] Validar que AutoMapper resuelve `MarcaNombre` sin ciclos.