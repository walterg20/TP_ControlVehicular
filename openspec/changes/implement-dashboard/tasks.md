# Implementation Tasks: Executive Dashboard

- [ ] 1. **Negocio Layer Implementation**
  - [ ] 1.1 Create `DashboardMetricsDto.cs` in `Negocio/DTOs/`
  - [ ] 1.2 Implement `ObtenerDashboardHandler.cs` in `Negocio/Services/` using `IVehiculoRepository` and `IMapper`

- [ ] 2. **Presentacion Layer Implementation**
  - [ ] 2.1 Create `DashboardViewModel.cs` in `Presentacion/ViewModels/`
  - [ ] 2.2 Create `CtlDashboard.xaml` and `CtlDashboard.xaml.cs` in `Presentacion/Pantalla/Dashboard/`
  - [ ] 2.3 Style metric cards with `CornerRadius="12"` (`rounded-xl`), `shadow-md`, and custom status colors
  - [ ] 2.4 Bind metric cards and workshop vehicles DataGrid to `DashboardViewModel`

- [ ] 3. **MainWindow & Navigation Integration**
  - [ ] 3.1 Modify `MainWindow.xaml` to add `"📊 Dashboard"` button above the OPERACIONES section
  - [ ] 3.2 Update `MainWindow.xaml.cs` click handler to inject `CtlDashboard` into `grdContenido`
  - [ ] 3.3 Set `CtlDashboard` as the default view loaded right after successful login

- [ ] 4. **Dependency Injection & App Startup**
  - [ ] 4.1 Register `ObtenerDashboardHandler`, `DashboardViewModel`, and `CtlDashboard` in `App.xaml.cs`
  - [ ] 4.2 Build solution (`dotnet build "TP_ControlVehicular.slnx"`) and verify execution
