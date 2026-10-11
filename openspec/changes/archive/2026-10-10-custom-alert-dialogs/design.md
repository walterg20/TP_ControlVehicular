# Technical Design: Custom Alert Dialogs

## Design Details
1. `FrmConfirmacion.xaml`:
   - Set `x:Name="lblIcono"` on the icon `TextBlock`.
   - Keep `x:Name="btnCancelar"` and `x:Name="btnAceptar"` accessible.
2. `FrmConfirmacion.xaml.cs`:
   - Implement `MostrarAviso`:
     ```csharp
     public static void MostrarAviso(string mensaje, string titulo = "Aviso", Window? owner = null, string textoBoton = "✓ Aceptar", string icono = "⚠️")
     {
         var dialog = new FrmConfirmacion(mensaje, titulo, textoAceptar: textoBoton);
         dialog.btnCancelar.Visibility = Visibility.Collapsed;
         dialog.lblIcono.Text = icono;
         // Set owner and show modal dialog
         dialog.ShowDialog();
     }
     ```
3. Controller replacements across `Ctl*.xaml.cs`:
   - Replace all `MessageBox.Show("Por favor, selecciona...", "Aviso")` calls with `FrmConfirmacion.MostrarAviso(...)`.
