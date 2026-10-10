# Proposal: ABM Servicio Implementation & Menu Reorganization

## Why
Services form the core catalog applied during vehicle maintenance (`DETALLE_SERVICIO`). Currently, the menu contains a placeholder button under OPERACIONES. Reorganizing services to the ADMINISTRACIÓN section as a full CRUD catalog ensures proper administrative control over service names and pricing.

## What
1. **Menu Reorganization**: Remove `"📦 Repuestos / Servicios"` from OPERACIONES and place `"📦 Servicios"` under the ADMINISTRACIÓN section in `MainWindow.xaml`.
2. **Repositories & Handlers**: Implement `IServicioRepository`, `ServicioRepository`, `ListarServiciosHandler`, `RegistrarServicioHandler`, `ModificarServicioHandler`, and `EliminarServicioHandler`.
3. **DTO & ViewModel**: Create `ServicioDto.cs` and `ServicioViewModel.cs`.
4. **UI Views**: Create `CtlServicio.xaml` & `.cs` (list view) and `FrmServicio.xaml` & `.cs` (modal create/edit window).

## Out of Scope
- Inventory management of physical spare parts (Repuestos). Physical spare parts can be tracked in future iterations.
