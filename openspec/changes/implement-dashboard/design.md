# Technical Design: Executive Dashboard

## Component Architecture

```
+-----------------------------------------------------------------------+
|                              PRESENTACION                             |
|                                                                       |
|  +------------------+         +---------------------+                 |
|  |   MainWindow     | ------> |    CtlDashboard     |                 |
|  +------------------+         +---------------------+                 |
|                                          |                            |
|                                          v                            |
|                               +---------------------+                 |
|                               | DashboardViewModel  |                 |
|                               +---------------------+                 |
+------------------------------------------|----------------------------+
                                           |
                                           v
+-----------------------------------------------------------------------+
|                                NEGOCIO                                |
|  +-----------------------------------------------------------------+  |
|  |                    ObtenerDashboardHandler                      |  |
|  +-----------------------------------------------------------------+  |
|                                          |                            |
|                                          v                            |
|  +-----------------------------------------------------------------+  |
|  |                      DashboardMetricsDto                        |  |
|  +-----------------------------------------------------------------+  |
+------------------------------------------|----------------------------+
                                           |
                                           v
+-----------------------------------------------------------------------+
|                                 DATOS                                 |
|  +-----------------------------------------------------------------+  |
|  |                       IVehiculoRepository                       |  |
|  +-----------------------------------------------------------------+  |
+-----------------------------------------------------------------------+
```

## Data Transfer Objects & Handlers

### `DashboardMetricsDto.cs`
- `int VehiculosActivosCount`
- `int EnProcesoCount`
- `int EntregadasHoyCount`
- `List<VehiculoDto> VehiculosEnTaller`

### `ObtenerDashboardHandler.cs`
- Injects `IVehiculoRepository` and `IMapper`.
- Method `Task<DashboardMetricsDto> HandleAsync()`:
  - Fetches all vehicles with `GetWithDetailsAsync()` / `GetAllAsync()`.
  - Maps to `List<VehiculoDto>`.
  - Calculates metrics (Vehículos Activos = total vehicles count; En Proceso = simulated / pending work orders; Entregadas Hoy = completed orders today).

## UI & Styling Guidelines
- **Container**: Border with `CornerRadius="12"`, `Background="White"`, `DropShadowEffect`.
- **Metric Cards Grid**: 3-column layout (`<UniformGrid Columns="3">` or `<Grid>` with 3 columns).
  - Card 1: Background `#E8F8F5`, Accent `#27AE60` (Vehículos Activos)
  - Card 2: Background `#FEF9E7`, Accent `#F39C12` (En Proceso)
  - Card 3: Background `#F4ECF7`, Accent `#8E44AD` (Entregadas Hoy)
- **DataGrid**: Auto-generated columns or explicit styled DataGrid (`#2C3E50` header, row hover effects).
