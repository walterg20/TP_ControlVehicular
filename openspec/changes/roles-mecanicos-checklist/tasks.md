# Tasks: Implementación de Roles y Mecánicos en Checklist

- [ ] 1. Actualizar `DetalleServicioDto.cs` agregando la propiedad `UsuarioId`.
- [ ] 2. Modificar `OrdenServicioViewModel.cs`:
  - Agregar `MecanicosDisponibles`.
  - Inyectar o usar repositorio de usuarios para cargar la lista en `LoadCombosAsync()`.
  - Actualizar el mapeo de `DetallesOrdenActual` a Entidad en `GuardarOrdenAsync` para incluir el `UsuarioId`.
- [ ] 3. Actualizar `CtlOrdenServicioForm.xaml`:
  - Agregar la columna "Mecánico" a la grilla con un ComboBox bindeado a `MecanicosDisponibles`.
- [ ] 4. Actualizar `CtlOrdenServicioForm.xaml.cs`:
  - Implementar lógica para deshabilitar cabeceras (`cmbVehiculos`, `cmbTalleres`, `txtKm`) si el usuario activo es Mecánico.
  - Opcional: Implementar chequeo para que el mecánico solo edite sus líneas.
- [ ] 5. Compilar (`dotnet build`) y asegurar que todo funciona limpiamente.
