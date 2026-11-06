# Design: FrmClienteVehiculo Layout Alignment

## Layout Structure
- Two balanced columns (`Border` with `#F8F9F9`, `CornerRadius="8"`, `Padding="15"`):
  - **Left column (Cliente)**:
    - Title: "Datos del Cliente"
    - 4 rows: DNI, Apellido, Nombre, Teléfono.
    - Each field uses `<TextBlock Text="..." FontWeight="Bold" Margin="0,0,0,2"/>` followed by `<StackPanel Margin="0,0,0,12">` containing the TextBox and validation message.
  - **Right column (Vehículo)**:
    - Title: "Datos del Vehículo"
    - 4 rows: Patente, Marca, Modelo, Año.
    - Each field uses the same label-above-input rhythm.
- Removal of `Motor` and `txtMotor` controls and property.
