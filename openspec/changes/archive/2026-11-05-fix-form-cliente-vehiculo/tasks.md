# Tasks: Fix FrmClienteVehiculo Layout

- [x] 1. Update XAML Layout
  - [x] 1.1 Separate Apellido, Nombre, and Teléfono with individual labels above each input in `FrmClienteVehiculo.xaml`
  - [x] 1.2 Separate Marca and Modelo with individual labels, remove Motor label and input, keep Año with its own label
  - [x] 1.3 Adjust Window Height to fit comfortably
- [x] 2. Update ViewModel
  - [x] 2.1 Remove unused `Motor` property from `ClienteVehiculoViewModel.cs`
- [x] 3. Verification
  - [x] 3.1 Verify build with `dotnet build`
  - [x] 3.2 Verify tests with `dotnet test`
