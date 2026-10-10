# 004 · Vehículos — Tareas

_Checklist accionable derivada del `plan.md`. Tareas pequeñas y concretas; marca `[x]` al completarlas._

- [x] Verificar entidad `Vehiculo.cs` (convenciones: string.Empty, Cliente null!, Modelo null!, sin colecciones).
- [x] Verificar DTOs `VehiculoDto.cs`, `VehiculoPorClienteDto.cs` y mapeo en `MappingProfile.cs` (ClienteNombre, ModeloNombre, MarcaNombre).
- [x] Verificar `IVehiculoRepository` + `VehiculoRepository` (CRUD + GetByPatenteAsync, GetByClienteAsync, GetWithDetailsAsync con Includes).
- [x] Verificar `RegistrarVehiculoHandler` (inyecta 3 repos + mapper; valida patente única normalizada, cliente existe, modelo existe; retorna DTO).
- [x] Verificar `ListarVehiculosPorClienteHandler` (inyecta repo + mapper; HandleAsync(int clienteId) → List<VehiculoPorClienteDto>).
- [x] Verificar `VehiculoViewModel` (NO hereda BaseViewModel; 3 repos inyectados; propiedades planas + OnPropertyChanged manual; RelayCommand manual; combos Cliente/Modelo).
- [x] Verificar `CtlVehiculo.xaml.cs` (Owner = Window.GetWindow(this), DataContext inyectado, combos bindeados).
- [x] Verificar registro DI en `App.xaml.cs` (AddScoped IVehiculoRepository, 2 handlers, VehiculoViewModel, CtlVehiculo).
- [x] Verificar migración: tabla Vehiculos PK, FKs ClienteId/ModeloId RESTRICT, índice único Patente, Anio not null.
- [x] Probar alta vehículo válido (patente normalizada, cliente+modelo existen, DTO con nombres compuestos).
- [x] Probar validación patente duplicada case-insensitive (error controlado).
- [x] Probar validación cliente/modelo inexistente.
- [x] Probar listado general y por cliente (DTOs con nombres resueltos).
- [x] Probar edición (cambio patente/cliente/modelo) y eliminación.
- [x] Probar eliminación cliente/modelo con vehículos (error referencial).
- [x] Validar contra los criterios de aceptación de `spec.md`.
- [x] Mover la feature a "Hecho" en `../../constitution/roadmap.md`.

## Mantenimiento (checklist recurrente)

_Opcional. Pasos a repetir cada vez que se toque esta feature en el futuro (revisar datos, regenerar algo, etc.). Borra esta sección si no aplica._

- [x] Revisar índice único Patente tras cambios de volumen; considerar normalización en BD (computed column).
- [x] Confirmar que `VehiculoViewModel` sigue sin heredar `BaseViewModel` y documenta por qué.
- [x] Validar que AutoMapper resuelve `MarcaNombre` vía `Modelo.Marca` sin `Include` innecesarios en listados.
- [x] Verificar que `RegistrarVehiculoHandler` normaliza patente antes de consultar/guardar.