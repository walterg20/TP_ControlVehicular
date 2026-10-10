# Technical Design: Roles y Asignación de Mecánicos

## Design Details

### 1. Actualización del DTO
En `DetalleServicioDto.cs`, mapear la propiedad existente en la base de datos:
```csharp
public int UsuarioId { get; set; } // Representa al mecánico asignado
```
Validar que `MappingProfile.cs` asigne automáticamente este campo de ida y de vuelta (al llamarse igual, AutoMapper lo hará automáticamente).

### 2. ViewModel (`OrdenServicioViewModel.cs`)
- Agregar propiedad `public ObservableCollection<UsuarioDto> MecanicosDisponibles { get; set; } = new();`.
- En `LoadCombosAsync()`, usar `IUsuarioRepository` para cargar aquellos usuarios que tengan un rol correspondiente a "Mecánico" (o cargar todos y filtrar en el front, según disponibilidad).
- Al guardar (`GuardarOrdenAsync`), asegurar que se traslada el `UsuarioId` del DTO a la Entidad `DetalleServicio`.

### 3. Modificaciones en la UI (`CtlOrdenServicioForm.xaml` y `.cs`)
- **Restricciones:** 
  - En el constructor o en un método de inicialización como `Loaded`, verificar `MainWindow.UsuarioSesionActual.IdRol`.
  - Si es mecánico, establecer `cmbVehiculos.IsEnabled = false;`, `cmbTalleres.IsEnabled = false;`, `txtKm.IsEnabled = false;`.
- **Asignación en Grilla:**
  - Agregar un `DataGridTemplateColumn` en `dgChecklist` que contenga un `ComboBox`.
  - El `ItemsSource` del combo será un `Binding` estático (usando `RelativeSource` para llegar al `DataContext` del UserControl y obtener `MecanicosDisponibles`).
  - El `SelectedValue` será `Binding UsuarioId` del propio `DetalleServicioDto`.
- **Restricciones de Fila:** 
  - Si el usuario logueado es mecánico, deshabilitar (o volver *Read-Only*) la edición de las celdas de "Revisado / OK" y "Observaciones" cuando `UsuarioId` de la línea no coincida con su propio ID. (Puede lograrse evaluándolo desde código en un evento de `BeginningEdit` del DataGrid o a través de un trigger en el estilo).
