# 005 · Taller

**Estado:** pendiente

## Qué hace

Permite gestionar talleres del sistema: registrar nuevo taller (nombre, dirección, teléfono), listar talleres existentes y modificar sus datos. La UI presenta un UserControl de listado (`CtlTaller`) y un formulario modal (`FrmTaller`) para alta/edición, alineado con el patrón DI + `RegistrationCompleted` + `DialogResult`/`Dispatcher.Invoke` usado en Cliente. La baja es lógica: setea `Activo = false` para preservar integridad referencial.

## Por qué

Complementa las entidades base del dominio vehicular: los talleres son el destino de alta/edición del catálogo junto a Marca/Modelo y soportan órdenes y asociaciones con vehículos. Centraliza la identidad del taller y permite trazabilidad de operaciones. La feature corrige además deuda técnica en `FrmTaller`/`CtlTaller`, que aún usaban code-behind stale (escritura directa `_vm.Nombre = txtNombre.Text`) y atributos `Owner="{x:Null}"`/`CenterOwner`, para alinearlos con el patrón real de Cliente y permitir la modificación de un taller existente.

## Criterios de aceptación

_Condiciones verificables que deben cumplirse para dar la feature por terminada. Redacta cada una de forma que se pueda comprobar con un sí/no. Marca `[x]` al cumplirse._

- [ ] Registrar taller válido persiste en SQL Server y devuelve `TallerDto` con `IdTaller` generado.
- [ ] `ModificarTallerHandler` actualiza un taller existente (`UpdateAsync` en repo) y devuelve el `TallerDto` modificado.
- [ ] `RegistrarTallerHandler` queda solo con `AddAsync` (altas nuevas); no contiene lógica condicional de edición.
- [ ] El botón "Nuevo" usa `RegistrarTallerHandler` y el botón "✏️ Modificar" usa `ModificarTallerHandler`.
- [ ] Listar talleres carga `ObservableCollection<TallerDto>` en `TallerViewModel` y muestra en UI.
- [ ] El modal `FrmTaller` incluye el campo `Teléfono` con validación y mapeo al DTO.
- [ ] `FrmTaller` se abre centrado, con `Owner = Window.GetWindow(this)`, y cierra tras guardar/cancelar.
- [ ] `FrmTaller` no contiene `Owner="{x:Null}"` ni `WindowStartupLocation="CenterOwner"`.
- [ ] `FrmTaller.xaml.cs` no escribe propiedades del VM por code-behind (`_vm.Nombre = txtNombre.Text`); delega en el VM vía `RegistrationCompleted`.
- [ ] `TallerSeleccionado` en `TallerViewModel` es `TallerDto` y usa `IdTaller` para construir la entidad en edición.
- [ ] `ModificarTallerHandler` está registrado en la DI de `App.xaml.cs` (`AddScoped`).
- [ ] Baja lógica: `BtnBaja_Click` confirma y setea `Activo = false` (no borrado físico).
- [ ] AutoMapper mapea `Taller` → `TallerDto` (`Id → IdTaller`, `Telefono`) sin ciclos ni referencias nulas.
- [ ] La solución compila sin errores (`dotnet build "TP_ControlVehicular.slnx"`).

## Fuera de alcance

_Lo que esta feature NO incluye, para evitar que crezca. Si algo se difiere, enlaza a dónde (roadmap/backlog)._

- Búsqueda/filtrado avanzado de talleres (backlog en listados/reportes).
- Historial de cambios/auditoría de talleres (backlog).
- Asociación de órdenes/vehículos a taller dentro de esta feature (feature de órdenes, posterior).
- Papelera / recuperación de talleres desactivados (backlog).