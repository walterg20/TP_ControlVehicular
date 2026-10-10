# 005 · Taller — Tareas

_Checklist accionable derivada del `plan.md`. Tareas pequeñas y concretas; marca `[x]` al completarlas._

- [ ] Verificar entidad `Taller.cs` (convenciones: `string.Empty`; campos `Id`, `Nombre`, `Direccion`, `Telefono`, `Activo`).
- [x] Verificar DTO `TallerDto.cs` (`IdTaller`, `Nombre`, `Direccion`, `Telefono`, `Activo`) y mapeo `Id → IdTaller` en `MappingProfile.cs`.
- [x] Verificar `ITallerRepository` + `TallerRepository` (hereda `Repository<Taller>`, `UpdateAsync` heredado).
- [x] Verificar `ListarTallerHandler` (inyecta repo + mapper, retorna `IEnumerable<TallerDto>`).
- [x] Crear `Negocio/Services/ModificarTallerHandler.cs` (inyecta repo + mapper; `HandleAsync` hace `UpdateAsync` y retorna `TallerDto`).
- [x] Simplificar `RegistrarTallerHandler.cs` (quitar condición `Id > 0`/`UpdateAsync`; solo `AddAsync`).
- [x] Ampliar `TallerViewModel.cs`: inyectar `ModificarTallerHandler`; `TallerSeleccionado` tipo `TallerDto`; conectar comando "✏️ Modificar" a `UpdateAsync`.
- [x] `FrmTaller.xaml`: agregar campo `Telefono`; quitar `Owner="{x:Null}"` y `WindowStartupLocation="CenterOwner"`.
- [x] Refactorizar `FrmTaller.xaml.cs` a patrón Cliente (DI + try/catch + `RegistrationCompleted` + `Dispatcher.Invoke` a `DialogResult`; constructores por `TallerDto`; sin code-behind escribiendo VM).
- [x] Alinear `CtlTaller.xaml(.cs)` con `CtlCliente` (try/catch DI; `Loaded += async → vm.LoadAsync()`; apertura del form por modo Nuevo/Modificar; `BtnBaja_Click` baja lógica `Activo=false`).
- [x] Registrar `ModificarTallerHandler` en `App.xaml.cs` (DI `AddScoped`).
- [x] Compilar: `dotnet build "TP_ControlVehicular.slnx"` (PowerShell: `;` no `&&`).

## Mantenimiento (checklist recurrente)

_Opcional. Pasos a repetir cada vez que se toque esta feature en el futuro (revisar datos, regenerar algo, etc.). Borra esta sección si no aplica._

- [ ] Revisar índices de BD en tabla Talleres tras cambios de volumen.
- [ ] Validar que AutoMapper no rompe al agregar propiedades a `Taller`/`TallerDto`.
- [ ] Confirmar que `TallerViewModel` sigue usando handlers separados para "Nuevo" (`RegistrarTallerHandler`) y "✏️ Modificar" (`ModificarTallerHandler`).