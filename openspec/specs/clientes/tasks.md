# 001 · Clientes — Tareas

_Checklist accionable derivada del `plan.md`. Tareas pequeñas y concretas; marca `[x]` al completarlas._

- [x] Verificar entidad `Cliente.cs` (convenciones: string.Empty, List<Vehiculo>, null!).
- [x] Verificar DTO `ClienteDto.cs` y mapeo en `MappingProfile.cs`.
- [x] Verificar `IClienteRepository` + `ClienteRepository` (CRUD completo).
- [x] Verificar `RegistrarClienteHandler` (inyecta repo + mapper, HandleAsync retorna DTO).
- [x] Verificar `ClienteViewModel` (hereda BaseViewModel, ObservableCollection<ClienteDto>, RelayCommand async, OnPropertyChanged).
- [x] Verificar `CtlCliente.xaml.cs` (Owner = Window.GetWindow(this), DataContext inyectado).
- [x] Verificar registro DI en `App.xaml.cs` (AddScoped repo, handler, VM, UserControl).
- [x] Verificar migración `20260827020907_InitialCreate.cs` incluye tabla Clientes.
- [x] Probar alta de cliente válido (persistencia + DTO retornado).
- [x] Probar validación DNI/email duplicado (error controlado).
- [x] Probar listado, edición y eliminación (sin vehículos asociados).
- [x] Probar eliminación con vehículos asociados (error referencial).
- [x] Validar contra los criterios de aceptación de `spec.md`.
- [x] Mover la feature a "Hecho" en `../../constitution/roadmap.md`.

## Mantenimiento (checklist recurrente)

_Opcional. Pasos a repetir cada vez que se toque esta feature en el futuro (revisar datos, regenerar algo, etc.). Borra esta sección si no aplica._

- [ ] Revisar índices de BD en tabla Clientes tras cambios de volumen.
- [ ] Validar que AutoMapper no rompe al agregar propiedades a Cliente/ClienteDto.
- [ ] Confirmar que `ClienteViewModel` sigue heredando `BaseViewModel` y usa `RelayCommand` async.