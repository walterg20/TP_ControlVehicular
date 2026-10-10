# Technical Design: Comprobantes con Stored Procedures

## 1. Database Level (Unmanaged by EF Migrations)
Scripts will be placed in `docs/`:
- `docs/sp_GenerarComprobanteOrden.sql`
- `docs/sp_GenerarComprobantePago.sql`
These scripts will execute a `SELECT` returning a flat structure containing all necessary data.

## 2. DTOs (Data Transfer Objects)
Placed in `Negocio/DTOs/Reportes/`:
- `ComprobanteOrdenDto.cs`: properties like `OrdenId`, `ClienteNombre`, `VehiculoPatente`, `VehiculoModelo`, `FechaRecepcion`, `FechaEstimadaEntrega`, `Observaciones`.
- `ComprobantePagoDto.cs`: properties like `OrdenId`, `ClienteNombre`, `VehiculoPatente`, `FechaPago`, `TotalManoObra`, `TotalRepuestos`, `TotalGeneral`.

## 3. Handlers
Placed in `Negocio/Handlers/Reportes/`:
- `GenerarComprobanteOrdenHandler.cs`
- `GenerarComprobantePagoHandler.cs`

**Data Access Strategy:**
Instead of `FromSqlRaw` on a tracked `DbSet`, we will use EF Core's `Database.SqlQueryRaw<T>` which is ideal for flat, unmapped DTOs from Stored Procedures.
Example:
```csharp
var comprobante = await _dbContext.Database
    .SqlQueryRaw<ComprobanteOrdenDto>(
        "EXEC sp_GenerarComprobanteOrden @OrdenId", 
        new SqlParameter("@OrdenId", request.OrdenId))
    .FirstOrDefaultAsync(cancellationToken);
```

## 4. UI Integration
In the corresponding ViewModel (e.g., `OrdenServicioViewModel`), add `ICommand`s for:
- `GenerarComprobanteOrdenCommand`
- `GenerarComprobantePagoCommand`

These will invoke the Handlers and display the results using a read-only popup (e.g., `FrmReporteViewer.xaml`) to present the data to the receptionist before "printing".
