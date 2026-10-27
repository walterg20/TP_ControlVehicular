# Task Breakdown: Billing System

## Phase 1: Database Setup
- [ ] **Task 1.1**: Create `schema_billing.sql` to define the `FACTURA`, `METODO_PAGO`, and `PAGO` tables, including all primary and foreign key constraints and the `fecha_pago` field.
- [ ] **Task 1.2**: Execute `schema_billing.sql` on the development database. Insert initial seed data for `METODO_PAGO` (e.g., Efectivo, Tarjeta).

## Phase 2: Application Models & Repositories
- [ ] **Task 2.1**: Implement the C# models for `Factura`, `MetodoPago`, and `Pago`.
- [ ] **Task 2.2**: Implement `FacturaRepository` for basic CRUD operations.
- [ ] **Task 2.3**: Implement `PagoRepository` for storing and retrieving payments by invoice.
- [ ] **Task 2.4**: Implement `MetodoPagoRepository` to retrieve available payment methods.

## Phase 3: Business Logic (BillingService)
- [ ] **Task 3.1**: Implement `BillingService.GenerarFactura`. Ensure it accurately calculates the `Total` by summing the `DETALLE_SERVICIO` records linked to the provided `REGISTRO_SERVICIO`.
- [ ] **Task 3.2**: Implement `BillingService.RegistrarPago`. Ensure it correctly retrieves the current balance.
- [ ] **Task 3.3**: Add Cash/Card validation rules inside `RegistrarPago`, capping the inserted payment amount to the balance for Cash and strictly enforcing the exact amount for Card. Return the calculated change (vuelto) to the caller.

## Phase 4: Integration (Future Scope / Out of Bounds for Backend Core)
- [ ] **Task 4.1**: Integrate the `BillingService` with the presentation layer UI, ensuring the change (vuelto) is correctly displayed for cash payments.
