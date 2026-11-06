# Plan: Corrección de Etiquetas y Eliminación de Motor en FrmClienteVehiculo

## Objetivo
Corregir el formulario modal `FrmClienteVehiculo.xaml` para que:
1. Cada campo de entrada tenga su etiqueta (Label/TextBlock) arriba individualmente:
   - Cliente: DNI, Apellido, Nombre, Teléfono.
   - Vehículo: Patente, Marca, Modelo, Año.
2. Eliminar el campo y la etiqueta de `Motor`, ya que no se persiste ni forma parte de la entidad.
3. Limpiar la propiedad `Motor` en `ClienteVehiculoViewModel.cs`.
