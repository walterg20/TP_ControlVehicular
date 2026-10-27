# Implementation Tasks: Comprobantes con Stored Procedures

- [x] **Task 1: SQL Scripts**
  - Create `docs/sp_GenerarComprobanteOrden.sql`.
  - Create `docs/sp_GenerarComprobantePago.sql`.

- [x] **Task 2: DTOs**
  - Create `Negocio/DTOs/Reportes/ComprobanteOrdenDto.cs`.
  - Create `Negocio/DTOs/Reportes/ComprobantePagoDto.cs`.

- [x] **Task 3: Handlers**
  - Create `Negocio/Handlers/Reportes/GenerarComprobanteOrdenHandler.cs`.
  - Create `Negocio/Handlers/Reportes/GenerarComprobantePagoHandler.cs`.
  - Register Handlers in `App.xaml.cs` DI container.

- [x] **Task 4: UI Integration**
  - Update `OrdenServicioViewModel` to inject the new Handlers.
  - Add `RelayCommand`s to trigger the reports.
  - Add buttons in the UI (`CtlOrdenServicio.xaml` / `CtlOrdenServicioForm.xaml`) to invoke these commands.
  - Create a simple UI modal (e.g., `FrmReporteViewer.xaml`) to present the retrieved DTO data to the user.
