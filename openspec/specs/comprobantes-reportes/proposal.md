# Proposal: Generación de Comprobantes con Stored Procedures

## Problem
Currently, the system lacks a formal mechanism to provide physical or digital receipts to the client at two critical moments:
1. When the vehicle is received by the receptionist (requires a Service Order receipt with an estimated delivery date).
2. When the vehicle is delivered back to the client and paid (requires a Payment receipt).

## Proposed Solution
Implement the generation of these reports using SQL Server Stored Procedures for optimal data aggregation. The EF Core application will consume these SPs without managing them via EF Migrations. The raw SQL scripts will be stored in the `docs/` folder for manual execution on the database engine.

## Scope
- Creation of raw SQL scripts for `sp_GenerarComprobanteOrden` and `sp_GenerarComprobantePago`.
- Creation of read-only DTOs to map the SP results.
- Implementation of Handlers to execute the SPs using EF Core's `SqlQueryRaw<T>`.
- UI Integration to trigger the generation of these receipts from the Service Order management screens.
