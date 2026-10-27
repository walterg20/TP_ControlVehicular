# Specification: Billing System

## 1. Data Model Updates (SQL Schema)
We will create the following tables based on the DER. The SQL script (`schema_billing.sql`) will include:

```sql
CREATE TABLE FACTURA (
    id_factura INT PRIMARY KEY AUTO_INCREMENT,
    id_registro INT NOT NULL,
    fecha DATE NOT NULL,
    total DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (id_registro) REFERENCES REGISTRO_SERVICIO(id_registro)
);

CREATE TABLE METODO_PAGO (
    id_metodo_pago INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50) NOT NULL
);

CREATE TABLE PAGO (
    id_pago INT PRIMARY KEY AUTO_INCREMENT,
    id_factura INT NOT NULL,
    id_metodo_pago INT NOT NULL,
    monto DECIMAL(10,2) NOT NULL,
    fecha_pago DATE NOT NULL,
    FOREIGN KEY (id_factura) REFERENCES FACTURA(id_factura),
    FOREIGN KEY (id_metodo_pago) REFERENCES METODO_PAGO(id_metodo_pago)
);
```

## 2. Business Logic

### 2.1 Invoice Generation (FACTURA)
When generating a new invoice for a `REGISTRO_SERVICIO`, the system will automatically calculate `FACTURA.total` by summing `precio * cantidad` for all associated `DETALLE_SERVICIO` records. The user does not input the total manually.

### 2.2 Payment Validation (PAGO)
To enforce the rule: "The sum of `PAGO.monto` linked to a `FACTURA` cannot exceed `FACTURA.total`", we will implement validation at the application layer.

**Algorithm for registering a new PAGO**:
1. Retrieve `FACTURA.total` for the target `id_factura`.
2. Calculate current sum of existing payments: `SELECT SUM(monto) FROM PAGO WHERE id_factura = ?`.
3. Calculate the remaining balance: `balance = FACTURA.total - current_sum`.
4. Validate based on `METODO_PAGO`:
   - **If method is Card (Tarjeta):** The requested `monto` must exactly match the `balance`. If it differs, reject the transaction.
   - **If method is Cash (Efectivo):** The user can provide a `monto` >= `balance`. The application layer will cap the `PAGO.monto` at the `balance` to ensure the database only records the exact amount needed to close the invoice. The presentation layer (UI) will be responsible for calculating and displaying the change (vuelto) to the user.
5. Insert into `PAGO` with the validated/adjusted `monto`.

*(Optional: A database trigger can also be used as a failsafe, but application-level validation provides better UX for error handling).*

## 3. Application Design

### 3.1 Entities/Models
- `Factura`: int IdFactura, int IdRegistro, DateTime Fecha, decimal Total
- `MetodoPago`: int IdMetodoPago, string Nombre
- `Pago`: int IdPago, int IdFactura, int IdMetodoPago, decimal Monto, DateTime FechaPago

### 3.2 Repositories / Data Access
- **FacturaRepository**:
  - `Create(Factura factura)`
  - `GetById(int idFactura)`
  - `GetByRegistroServicio(int idRegistro)`
- **PagoRepository**:
  - `Create(Pago pago)`
  - `GetByFactura(int idFactura)`
- **MetodoPagoRepository**:
  - `GetAll()`

### 3.3 Services
- **BillingService**:
  - `GenerateInvoice(int idRegistro)`: Gathers `DETALLE_SERVICIO` items, calculates the total, and creates a `FACTURA`.
  - `RegisterPayment(int idFactura, int idMetodoPago, decimal monto)`: Performs the validation logic described in section 2 and delegates to `PagoRepository`.
