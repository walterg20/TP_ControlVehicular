# Implementation Tasks: Role Restrictions & Work Orders Report

- [ ] 1. **Domain & Negocio Layer Implementation**
  - [ ] 1.1 Create `OrdenTrabajoReporteDto.cs` in `Negocio/DTOs/`
  - [ ] 1.2 Implement `ObtenerReporteOrdenesHandler.cs` in `Negocio/Services/` returning mock/DB work order reports

- [ ] 2. **Presentacion Layer Implementation**
  - [ ] 2.1 Create `ReporteOrdenesViewModel.cs` in `Presentacion/ViewModels/`
  - [ ] 2.2 Create `CtlReporteOrdenes.xaml` and `CtlReporteOrdenes.xaml.cs` in `Presentacion/Pantalla/Reporte/`
  - [ ] 2.3 Style report control with `CornerRadius="12"` (`rounded-xl`), `shadow-md`, filter bar, and DataGrid

- [ ] 3. **Role-Based Access Control in MainWindow**
  - [ ] 3.1 Update `MainWindow.xaml` to add `"REPORTES"` section and `"📋 Órdenes de Trabajo"` button (`btnReporteOrdenes`)
  - [ ] 3.2 Implement `AplicarRestriccionesPorRol(string rolNombre)` in `MainWindow.xaml.cs`
  - [ ] 3.3 Connect `btnReporteOrdenes` click event to display `CtlReporteOrdenes`

- [ ] 4. **Dependency Injection & Verification**
  - [ ] 4.1 Register `ObtenerReporteOrdenesHandler`, `ReporteOrdenesViewModel`, and `CtlReporteOrdenes` in `App.xaml.cs`
  - [ ] 4.2 Compile solution (`dotnet build "TP_ControlVehicular.slnx"`) and verify role restrictions and report rendering
