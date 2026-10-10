# Proposal: Standardize UI Design Across All Ctl UserControls

## Why
Currently, `CtlServicio.xaml` introduced a modern card-based layout with rounded corners and drop shadow effects, while other `Ctl*.xaml` controls use older default WPF controls. Furthermore, action buttons were placed inconsistently across views (e.g., `CtlServicio` had Edit/Delete in the header, while `CtlCliente` had them in the footer).

Standardizing all list controls ensures visual harmony, consistent UX across the application, clear button color conventions, and maintains project rules.

## What
1. Update `CtlServicio.xaml` to move Edit and Delete buttons to the bottom footer (Row 3).
2. Refactor all other list UserControls (`CtlCliente`, `CtlVehiculo`, `CtlModelo`, `CtlMarca`, `CtlTaller`, `CtlUsuario`, `CtlRol`) to adopt the modern 4-row card layout (`CornerRadius="12"`, shadow effects, `#2C3E50` DataGrid headers).
3. Standardize button colors (`#27AE60` for New, `#F39C12` for Edit, `#C0392B` for Delete, `#2980B9` for Search/Refresh).
4. Update `AGENTS.md` with explicit UI design guidelines for `Ctl*.xaml` controls.

## Impact
- Files affected: `AGENTS.md`, `CtlServicio.xaml`, `CtlCliente.xaml`, `CtlVehiculo.xaml`, `CtlModelo.xaml`, `CtlMarca.xaml`, `CtlTaller.xaml`, `CtlUsuario.xaml`, `CtlRol.xaml`.
- No breaking C# code changes required; all DataContext bindings and Click handler names will be strictly preserved.
