# Technical Design: Billing System

## 1. Database Implementation
A SQL script (`schema_billing.sql`) will be created to define the new tables.

**Tables & Relationships:**
- `FACTURA`: Stores the invoice data. Relates to `REGISTRO_SERVICIO` via `id_registro`.
- `METODO_PAGO`: Dictionary table for payment types.
- `PAGO`: Stores payment transactions. Relates to `FACTURA` (`id_factura`) and `METODO_PAGO` (`id_metodo_pago`).

## 2. Application Architecture

### 2.1 Domain Entities (Models)
- `Factura`: 
  - Properties: `IdFactura` (int), `IdRegistro` (int), `Fecha` (DateTime), `Total` (decimal).
- `MetodoPago`:
  - Properties: `IdMetodoPago` (int), `Nombre` (string).
- `Pago`:
  - Properties: `IdPago` (int), `IdFactura` (int), `IdMetodoPago` (int), `Monto` (decimal), `FechaPago` (DateTime).

### 2.2 Data Access Layer (Repositories)
The repositories will handle raw CRUD operations against the database.
- `FacturaRepository`:
  - `Create(Factura factura)`
  - `GetById(int idFactura)`
  - `GetByRegistroServicio(int idRegistro)`
- `PagoRepository`:
  - `Create(Pago pago)`
  - `GetPagosByFactura(int idFactura)`
- `MetodoPagoRepository`:
  - `GetAll()`

### 2.3 Business Logic Layer (Services)
- `BillingService`:
  - `GenerarFactura(int idRegistro)`: Retrieves the associated `DETALLE_SERVICIO` records, sums `(precio * cantidad)`, and persists a new `Factura`.
  - `RegistrarPago(int idFactura, int idMetodoPago, decimal montoAbonado)`:
    - Calculates the current balance (`Factura.Total` - SUM(`Pago.Monto`)).
    - Validates according to the `MetodoPago` rules:
      - If Card (Tarjeta): `montoAbonado` must equal the balance.
      - If Cash (Efectivo): `montoAbonado` can be >= balance, but the inserted `Pago.Monto` is capped at the exact balance.
    - Persists the new `Pago`.
    - Returns the actual recorded `Pago` and the change (vuelto) to the presentation layer.

### 2.4 Presentation Layer Integration
- UI components will need to capture payment details and, in the case of cash payments, display the calculated change (vuelto) returned by the `BillingService`.
