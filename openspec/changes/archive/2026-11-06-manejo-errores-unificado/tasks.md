# Tasks: Manejo unificado de errores silenciosos

- [ ] 1. Crear `IErrorNotifier` y `ErrorNotifierService` (registro + evento + dedup 2s + log a archivo).
- [ ] 2. Registrar `IErrorNotifier` en `App.xaml.cs` y global handlers en `App.OnStartup`.
- [ ] 3. Agregar `MainWindow.MostrarError(mensaje, modal)` y suscripción al evento del notifier.
- [ ] 4. Fase A: reemplazar catch silenciosos en ViewModels (OrdenServicio, Cliente, Vehículo, Marca, Modelo, Servicio, Taller, Usuario, Rol, MisTrabajos, MisTareas, Backup, BackupConfiguracion, EmpresaConfiguracion, Reportes*).
- [ ] 5. Fase B: reemplazar catch silenciosos en code-behind de pantallas (Ctl*/Frm*).
- [ ] 6. Fase C: reemplazar catch silenciosos en servicios (AyudaService, DriveBackupService, etc.).
- [ ] 7. Tests: `ErrorNotifierServiceTests` + ajuste de VMs tests (notifier opcional).
- [ ] 8. Build + tests + commit.