# Specification: Generación de Comprobantes

## Feature: Service Order Receipt (Comprobante de Recepción)

**Scenario: A receptionist confirms a new service order**
- **Given** the receptionist has successfully created a Service Order for a client's vehicle
- **When** the receptionist clicks "Generar Comprobante" (or confirms the creation)
- **Then** the system should execute `sp_GenerarComprobanteOrden`
- **And** retrieve the client's data, vehicle data, service description, and estimated delivery date.
- **And** display the receipt data for printing/confirmation.

## Feature: Payment Receipt (Comprobante de Pago)

**Scenario: A receptionist registers the payment and delivery of a vehicle**
- **Given** an existing Service Order that is ready for delivery
- **When** the receptionist registers the payment and clicks "Generar Comprobante de Pago"
- **Then** the system should execute `sp_GenerarComprobantePago`
- **And** retrieve the final costs, client data, vehicle data, and payment date.
- **And** display the payment receipt data for printing/confirmation.
