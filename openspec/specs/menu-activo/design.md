# Design: Resaltado Dinámico del Menú Activo

## Arquitectura y Componentes Afectados

El control de la navegación y el estilo de la barra lateral recae sobre la vista principal (`MainWindow.xaml` y `MainWindow.xaml.cs`).

### 1. `MainWindow.xaml`
- **Quitar Hardcodeo**: Se debe quitar la propiedad `Background="#2980B9"` del `btnDashboard` en el XAML y dejarlo como `Background="Transparent"` para que todos los botones partan del mismo estado base.

### 2. `MainWindow.xaml.cs` (Code-Behind)
- **Método Helper `ResaltarBotonActivo`**: Se creará una función privada que reciba el botón presionado. Esta función realizará dos pasos:
  1. *Resetear todos los botones*: Asignará `Background = Brushes.Transparent` a `btnDashboard`, `btnOrdenesTrabajo`, `btnServicio`, etc.
  2. *Resaltar el seleccionado*: Al botón que recibió como parámetro, le asignará un pincel azul (`#2980B9`) y lo pondrá en negrita.

```csharp
private void ResaltarBotonActivo(Button botonActivo)
{
    // 1. Resetear todos
    var transparente = System.Windows.Media.Brushes.Transparent;
    btnDashboard.Background = transparente;
    btnOrdenesTrabajo.Background = transparente;
    // ... repetir para los demás

    // 2. Pintar el activo
    if (botonActivo != null)
    {
        var colorAzul = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2980B9");
        botonActivo.Background = new System.Windows.Media.SolidColorBrush(colorAzul);
    }
}
```

- **Interceptar los Clics**: Modificar todos los métodos manejadores de eventos (ej: `MenuItem_Click_Servicio`) para que incluyan la llamada:
  `ResaltarBotonActivo(sender as Button);`

- **Estado Inicial**: En el método `OnLoginExitoso` (o donde se lance el Dashboard por primera vez tras iniciar sesión), inyectar `ResaltarBotonActivo(btnDashboard);`.
