# Implementation Tasks: ABM Servicio

- [ ] 1. **Datos & Negocio Layer Implementation**
  - [ ] 1.1 Create `IServicioRepository.cs` in `Negocio/Interfaces/` and `ServicioRepository.cs` in `Datos/Repositories/`
  - [ ] 1.2 Create `ServicioDto.cs` in `Negocio/DTOs/` and update `MappingProfile.cs`
  - [ ] 1.3 Create handlers: `ListarServiciosHandler.cs`, `RegistrarServicioHandler.cs`, `ModificarServicioHandler.cs`, `EliminarServicioHandler.cs` in `Negocio/Services/`

- [ ] 2. **Presentacion Layer Implementation**
  - [ ] 2.1 Create `ServicioViewModel.cs` in `Presentacion/ViewModels/`
  - [ ] 2.2 Create `CtlServicio.xaml` & `CtlServicio.xaml.cs` in `Presentacion/Pantalla/Servicio/`
  - [ ] 2.3 Create `FrmServicio.xaml` & `FrmServicio.xaml.cs` in `Presentacion/Pantalla/Servicio/`

- [ ] 3. **MainWindow & Navigation Integration**
  - [ ] 3.1 Modify `MainWindow.xaml` to remove placeholder from OPERACIONES and add `btnServicio` (`"📦 Servicios"`) under ADMINISTRACIÓN
  - [ ] 3.2 Update `MainWindow.xaml.cs` click handlers and RBAC rules

- [ ] 4. **Dependency Injection & Verification**
  - [ ] 4.1 Register `IServicioRepository`, `ServicioRepository`, handlers, `ServicioViewModel`, `CtlServicio`, and `FrmServicio` in `App.xaml.cs`
  - [ ] 4.2 Compile solution (`dotnet build "TP_ControlVehicular.slnx"`) and verify CRUD operations
