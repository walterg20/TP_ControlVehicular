# 006 · Refactorización — Tareas

_Checklist accionable derivada del `plan.md`. Tareas pequeñas y concretas; marca `[x]` al completarlas._

- [x] Refactorizar variables de ViewModels en `Presentacion/Pantalla/Cliente/` (`CtlCliente.xaml.cs`, `FrmCliente.xaml.cs`).
- [x] Refactorizar variables de ViewModels en `Presentacion/Pantalla/Marca/` (`CtlMarca.xaml.cs`, `FrmMarca.xaml.cs`).
- [x] Refactorizar variables de ViewModels en `Presentacion/Pantalla/Modelo/` (`CtlModelo.xaml.cs`, `FrmModelo.xaml.cs`).
- [x] Refactorizar variables de ViewModels en `Presentacion/Pantalla/Vehiculo/` (`CtlVehiculo.xaml.cs`, `FrmVehiculo.xaml.cs`).
- [x] Refactorizar variables de ViewModels en `Presentacion/Pantalla/Taller/` (`CtlTaller.xaml.cs`, `FrmTaller.xaml.cs`).
- [x] Refactorizar campos y parámetros en `Presentacion/ViewModels/` (`ClienteViewModel`, `MarcaViewModel`, `ModeloViewModel`, `VehiculoViewModel`, `TallerViewModel`).
- [x] Refactorizar campos y parámetros en `Negocio/Services/` (`RegistrarClienteHandler`, `RegistrarVehiculoHandler`, `ListarVehiculosPorClienteHandler`, `ListarTallerHandler`, `RegistrarTallerHandler`, `ModificarTallerHandler`).
- [x] Refactorizar campo `_context` en `Datos/Repositories/Repository.cs` y repositorios derivados.
- [x] Validar compilación limpia con `dotnet build "TP_ControlVehicular.slnx"`.
- [x] Validar criterios de aceptación de `spec.md`.

## Mantenimiento (checklist recurrente)

- [ ] Verificar que futuras integraciones de ViewModels o Handlers mantengan la convención de nombres explícitos de dominio.
