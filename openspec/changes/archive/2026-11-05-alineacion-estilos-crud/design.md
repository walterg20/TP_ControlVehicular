# Design: Alignment of CRUD Screen Styling with CtlModelo

## Architecture & Layers
1. **Repositories (`Datos/Repositories/`) & Interfaces (`Negocio/Interfaces/`)**:
   - `IClienteRepository`, `IUsuarioRepository`, `IRolRepository`, `IServicioRepository`, `ITallerRepository`:
     Change `GetPaginadoAsync` to take `string? filtroEstado` instead of `bool incluirInactivos` (or parameterless).
   - Filter query via `EF.Functions.Like` for search and expressions for `filtroEstado` ("Activos" -> `x.Activo`, "Inactivos" -> `!x.Activo`, "Todos" -> true).

2. **Handlers (`Negocio/Services/`)**:
   - `ListarUsuariosHandler`, `ListarRolHandler`, `ListarServiciosHandler`, `ListarTallerHandler`:
     Update `HandlePaginadoAsync` signature to receive `string? filtroEstado`.

3. **ViewModels (`Presentacion/ViewModels/`)**:
   - Expose `FiltroEstado`, `EstadosDisponibles` (`["Todos", "Activos", "Inactivos"]`), `PuedeVerFiltroEstado => PuedeEliminar`.
   - On permission notification, notify `PuedeVerFiltroEstado`.
   - Pass `PuedeVerFiltroEstado ? FiltroEstado : "Activos"` to handlers / repositories in `CargarPaginaAsync()`.

4. **Views (`Presentacion/Pantalla/`)**:
   - Row 1: ComboBox `Estado:` bound to `FiltroEstado` with `SelectionChanged="CmbFiltroEstado_SelectionChanged"`.
   - Row 2: Remove `DataGridCheckBoxColumn Header="Activo"`, add `DataGrid.RowStyle` DataTrigger for `Activo == False`.
   - Row 4: Standardize action buttons order & styling: Edit (`#F39C12`), Delete (`#C0392B`), Reactivate (`#27AE60`).
