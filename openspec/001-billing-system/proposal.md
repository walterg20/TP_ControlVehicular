# Proposal: Billing System Implementation

## Problem Statement
The system currently manages vehicles, clients, and service records (`REGISTRO_SERVICIO`), but lacks a formal billing system. The garage needs a way to issue invoices (`FACTURA`) for closed services and register payments (`PAGO`) against these invoices using various payment methods (`METODO_PAGO`). Additionally, we must enforce the business rule that the total amount paid cannot exceed the invoice's total amount.

## Proposed Solution
We propose implementing the billing module by introducing three new entities as described in the ERD (`docs/DER.md`):
1. **FACTURA**: Represents the invoice generated for a service record.
2. **METODO_PAGO**: A catalog of available payment methods (e.g., Cash, Credit Card, Transfer).
3. **PAGO**: Represents individual payment transactions made against an invoice.

## Scope
- Database schema creation (SQL script) for `FACTURA`, `METODO_PAGO`, and `PAGO`.
- Implementation of the business rule: `SUM(PAGO.monto) <= FACTURA.total`.
- Basic data access logic to generate invoices and register payments.

## Open Questions
- What should be the exact behavior if a user tries to create a payment that exceeds the invoice total? Should we reject the entire transaction, or accept up to the remaining balance?
- Do we need to support partial payments across different dates, and do we need to track the date of each `PAGO`? (The DER does not specify a date for `PAGO`).
- Should `FACTURA.total` be automatically calculated based on the sum of `DETALLE_SERVICIO.precio * cantidad` related to the `REGISTRO_SERVICIO`?
