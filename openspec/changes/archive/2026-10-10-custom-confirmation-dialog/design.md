# Technical Design: Custom Confirmation Dialog (FrmConfirmacion)

## Architecture
`FrmConfirmacion` is a WPF `Window` with `WindowStyle="None"`, `AllowsTransparency="True"`, and `WindowStartupLocation="CenterOwner"`.

### Visual Components
- **Container**: `Border` with `CornerRadius="12"`, `Background="White"`, `BorderBrush="#BDC3C7"`, `BorderThickness="1"`, `DropShadowEffect`.
- **Header**: `Border` with `Background="#2C3E50"`, `CornerRadius="12,12,0,0"`, containing `lblTitulo` (`Foreground="White"`, `FontSize="16"`, `FontWeight="Bold"`).
- **Body**: `Grid` containing warning icon `⚠️` and `lblMensaje` (`FontSize="14"`, `Foreground="#2C3E50"`, `TextWrapping="Wrap"`).
- **Footer**: `StackPanel` aligned right with:
  - `btnAceptar`: `#27AE60` Green, `CornerRadius="6"`, `Foreground="White"`, `FontWeight="Bold"`.
  - `btnCancelar`: `#C0392B` Red, `CornerRadius="6"`, `Foreground="White"`, `FontWeight="Bold"`.

### Static Helper Method
```csharp
public static bool Mostrar(string mensaje, string titulo = "Confirmar eliminación", Window? owner = null)
{
    var dialog = new FrmConfirmacion(mensaje, titulo);
    if (owner != null) dialog.Owner = owner;
    return dialog.ShowDialog() == true;
}
```

### Integration Sites
Replace `MessageBox.Show(..., MessageBoxButton.YesNo)` in:
- `CtlCliente.xaml.cs`
- `CtlVehiculo.xaml.cs`
- `CtlModelo.xaml.cs`
- `CtlMarca.xaml.cs`
- `CtlServicio.xaml.cs`
- `CtlTaller.xaml.cs`
- `CtlUsuario.xaml.cs`
- `CtlRol.xaml.cs`
