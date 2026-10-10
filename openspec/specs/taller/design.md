# 005 · Taller — Plan

_Cómo se implementa lo descrito en `spec.md`. Debe respetar la `constitution/`._

## Enfoque

La feature gestiona el CRUD de talleres (nominal: registrar, listar, modificar, baja lógica) usando el patrón handler-repositorio-MVVM ya establecido en el proyecto (espejo de `001-clientes`). Se inyecta `ITallerRepository` en `ListarTallerHandler`, `RegistrarTallerHandler`, `ModificarTallerHandler` y `TallerViewModel`, con AutoMapper Entity→DTO unidireccional. La UI usa `CtlTaller` (UserControl de listado) y `FrmTaller` (ventana modal de alta/edición) alineada con el patrón real de `FrmCliente`: construcción del VM vía DI, evento `RegistrationCompleted` que decide `DialogResult`, y cierre con `Dispatcher.Invoke`. La baja es lógica (setea `Activo = false`), en línea con lo requerido.

## Implementación

_Pasos técnicos concretos, en orden. Indica los archivos/módulos que se tocan._

1. Verificar entidad `Entidad/Taller.cs`: contiene `Id`, `Nombre`, `Direccion`, `Telefono`, `Activo` (convenciones: `string.Empty`).
2. Verificar DTO `Negocio/DTOs/TallerDto.cs` (nombres compuestos): `IdTaller`, `Nombre`, `Direccion`, `Telefono`, `Activo`; perfil `MappingProfile.cs` ya mapea `Id → IdTaller`.
3. Verificar `Datos/Repositories/ITallerRepository.cs` + `TallerRepository.cs` (hereda `Repository<Taller>`); `UpdateAsync` heredado de `Repository<T>`.
4. Verificar handler `ListarTallerHandler.cs` (inyecta `ITallerRepository` + `IMapper`, retorna `IEnumerable<TallerDto>`).
5. **Crear** `Negocio/Services/ModificarTallerHandler.cs`: inyecta `ITallerRepository` + `IMapper`; `HandleAsync` hace `UpdateAsync` y retorna `_mapper.Map<TallerDto>(taller)`.
6. **Simplificar** `Negocio/Services/RegistrarTallerHandler.cs`: quitar el bloque `UpdateAsync` condicional (`Id > 0`); dejar únicamente `AddAsync` para entidades nuevas, retornando `_mapper.Map<TallerDto>(taller)`.
7. Alinear `Presentacion/ViewModels/TallerViewModel.cs`: inyectar `ModificarTallerHandler`; `TallerSeleccionado` pasa a `TallerDto`; conectar el comando "✏️ Modificar" a `UpdateAsync`.
8. `Presentacion/Pantalla/Taller/FrmTaller.xaml`: agregar el campo `Telefono`; quitar `Owner="{x:Null}"` y `WindowStartupLocation="CenterOwner"` para alinearse con `FrmCliente`.
9. `Presentacion/Pantalla/Taller/FrmTaller.xaml.cs`: refactor a patrón DTO/VM DI — constructor de alta "Nuevo" con try/catch + `DataContext = vm` + `vm.RegistrationCompleted += (s, ok) => Dispatcher.Invoke(() => this.DialogResult = ok)`; constructor de edición por `TallerDto` que precarga el VM y llama al mismo comando.
10. `Presentacion/Pantalla/Taller/CtlTaller.xaml(.cs)`: alinear con `CtlCliente` — método de apertura del form según modo (Nuevo/Modificar) y `BtnBaja_Click` que confirma y setea `Activo=false`.
11. Registrar `ModificarTallerHandler` en `App.xaml.cs` (DI `AddScoped`).

## Decisiones

_Elecciones de diseño relevantes y su justificación. Alternativas descartadas y por qué._

- **Handler + Repository + AutoMapper** — Separación de responsabilidades: lógica de negocio en handler, acceso a datos en repo, mapeo en perfil; evita acoplar VM a EF Core.
- **`ModificarTallerHandler` reemplaza la edición dentro de `RegistrarTallerHandler`** — Separar "alta" (siempre `AddAsync`) de "modificación" (siempre `UpdateAsync`) en handlers distintos, igual que hace Cliente a nivel de propósito. El botón "Nuevo" usa `RegistrarTallerHandler`; "✏️ Modificar" usa `ModificarTallerHandler`.
- **AutoMapper unidireccional Entity→DTO** — Simplifica y evita ciclos; DTOs planos para UI. No se mapea DTO→Entity (se construye/actualiza la entidad en el handler).
- **`FrmTaller` alineado con `FrmCliente`** — Patrón DI + `RegistrationCompleted` + `DialogResult`/`Dispatcher.Invoke` + try/catch, en lugar del code-behind stale actual que escribía directo `_vm.Nombre = txtNombre.Text`. El modal se abre con `Owner = Window.GetWindow(this)`.
- **Campo `Telefono` en la UI** — Ya existe en `Entidad/Taller.cs` y `TallerDto.cs`; se agrega al form con su validación y mapeo para completar el modelo.
- **Baja lógica (`Activo=false`)** — El taller no se borra físicamente; se desactiva para preservar integridad referencial con vehículos/órdenes. Coherente con `BtnBaja_Click` actual.
- **DI `AddScoped` para repos/handlers/VMs** — Ciclo de vida por request/operación; coherente con `App.xaml.cs` actual.

## Riesgos

_Qué puede salir mal o requerir cuidado, y cómo se mitiga._

- **Mismatch `Id` vs `IdTaller`** — Mitigación: AutoMapper ya mapea `Id → IdTaller`; validar que el DTO seleccionado use `IdTaller` para construir la entidad en edición.
- **Mantener código stale en `FrmTaller.xaml.cs`** — Mitigación: refactor completo del code-behind (forzar re-escritura) para que deje de inyectar direcciones por texto y use el VM; compilar para detectar referencias muertas.
- **UI con `Owner="{x:Null}"`/`CenterOwner`** — Mitigación: eliminar esos atributos para heredar el patrón `Owner = Window.GetWindow(this)` de Cliente.
- **Rutas con espacios/caracteres no ASCII** — Mitigación: usar siempre comillas y `;` (nunca `&&`) en comandos PowerShell (`dotnet build "TP_ControlVehicular.slnx"`).