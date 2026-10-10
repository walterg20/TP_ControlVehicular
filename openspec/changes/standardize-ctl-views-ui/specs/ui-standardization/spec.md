# Delta Spec: UI Standardization for UserControls

## Modified Requirements

### Requirement: Layout Standardization for Ctl Views
- `CtlServicio.xaml`: Move `BtnEditar` and `BtnBorrar` to footer (Row 3). Keep `BtnNuevo` in Row 1.
- `CtlCliente.xaml`, `CtlVehiculo.xaml`, `CtlModelo.xaml`, `CtlMarca.xaml`, `CtlTaller.xaml`, `CtlUsuario.xaml`, `CtlRol.xaml`:
  - Enclose search bar in a white card container with `CornerRadius="12"` and `DropShadowEffect`.
  - Enclose DataGrid in a white card container with `CornerRadius="12"`, `DropShadowEffect`, and styled column headers (`Background="#2C3E50"`, `Foreground="White"`).
  - Wrap footer action buttons in styled buttons (`CornerRadius="6"`, bold text, distinct colors: Green for New, Orange for Edit, Red for Delete/State toggle).
