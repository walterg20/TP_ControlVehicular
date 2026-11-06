# Spec: FrmClienteVehiculo Layout and Fields

## Scenario 1: Dedicated label per input in Client column
Given the modal `FrmClienteVehiculo`
When rendered on screen
Then the Client section displays distinct labels positioned above each input:
  - "DNI / CUIT:" above `txtDni`
  - "Apellido:" above `txtApellido`
  - "Nombre:" above `txtNombre`
  - "Teléfono:" above `txtTelefono`
And validation error text blocks are preserved for each field.

## Scenario 2: Dedicated label per input in Vehicle column and removal of Motor
Given the modal `FrmClienteVehiculo`
When rendered on screen
Then the Vehicle section displays distinct labels positioned above each input:
  - "Patente / Dominio:" above `txtPatente`
  - "Marca:" above `cmbMarca`
  - "Modelo:" above `cmbModelo`
  - "Año (opcional):" above `txtAnio`
And the "Motor" label and `txtMotor` input are removed from the form.
