# Facturacion Service Specification

## Overview
The facturacion service provides invoicing (`GenerarFactura`) and payment registration (`RegistrarPago`) operations under the domain name `IFacturacionService`.

## Scenarios

### Scenario 1: Invoice Generation via IFacturacionService
- **GIVEN** a completed service order (`RegistroServicio`) with pending invoice
- **WHEN** `IFacturacionService.GenerarFactura(registroServicioId)` is called
- **THEN** it generates and persists a `Factura` with calculated totals and returns the created `Factura`.

### Scenario 2: Payment Registration via IFacturacionService
- **GIVEN** an existing `Factura` that is not yet paid
- **WHEN** `IFacturacionService.RegistrarPago(facturaId, metodoPagoId, monto)` is called
- **THEN** it validates payment criteria, records the `Pago`, updates order state to `Pagada`, and returns the payment entity with any calculated change.
