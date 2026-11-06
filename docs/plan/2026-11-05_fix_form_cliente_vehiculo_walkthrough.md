# Walkthrough: Corrección de Etiquetas y Eliminación de Motor en FrmClienteVehiculo

## Resumen de Cambios
Se rediseñó el formulario modal `FrmClienteVehiculo.xaml` para asegurar que cada campo de entrada cuente con su correspondiente etiqueta (Label / TextBlock) ubicada directamente arriba, y se eliminó el campo y etiqueta de `Motor` que no se persisten en la base de datos:

1. **Columna Izquierda (Datos del Cliente)**:
   - **DNI / CUIT**: Etiqueta `DNI / CUIT:` sobre `txtDni`.
   - **Apellido**: Etiqueta individual `Apellido:` sobre `txtApellido`.
   - **Nombre**: Etiqueta individual `Nombre:` sobre `txtNombre`.
   - **Teléfono**: Etiqueta individual `Teléfono:` sobre `txtTelefono`.
2. **Columna Derecha (Datos del Vehículo)**:
   - **Patente**: Etiqueta `Patente / Dominio:` sobre `txtPatente`.
   - **Marca**: Etiqueta individual `Marca:` sobre `cmbMarca`.
   - **Modelo**: Etiqueta individual `Modelo:` sobre `cmbModelo`.
   - **Año**: Etiqueta individual `Año (opcional):` sobre `txtAnio`.
   - **Motor**: Se removió la etiqueta `Motor` y el `TextBox` `txtMotor`.
3. **ViewModel**:
   - Se removió la propiedad no persistida `Motor` de `ClienteVehiculoViewModel.cs`.
4. **Dimensiones**:
   - Altura de la ventana ajustada a `Height="580"` con `ScrollViewer` vertical automático para una visualización holgada y cómoda.

---

## Verificación
- `dotnet build "TP_ControlVehicular.slnx"`: **0 Errores**.
- `dotnet test --filter "FullyQualifiedName~ClienteVehiculoViewModelTests"`: **9 pruebas superadas**.
- `dotnet test --filter "FullyQualifiedName!~EndToEnd"`: **147 pruebas superadas**.
