# Proposal: Fix FrmClienteVehiculo Layout and Remove Motor Input

## Intent
Improve the usability and alignment of `FrmClienteVehiculo.xaml` by giving each individual input its own dedicated label above it (DNI, Apellido, Nombre, Teléfono on the left; Patente, Marca, Modelo, Año on the right), and removing the non-persisted `Motor` input.

## Scope
- `Presentacion/Pantalla/OrdenServicio/FrmClienteVehiculo.xaml`
- `Presentacion/ViewModels/ClienteVehiculoViewModel.cs` (removal of unused `Motor` property)
