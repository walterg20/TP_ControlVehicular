# Specification: Button States and Billing Fixes

## 1. UI Changes

### 1.1 `OrdenServicioViewModel.cs`
Update the `OrdenSeleccionada` property to notify changes:
```csharp
private RegistroServicioDto? _ordenSeleccionada;
public RegistroServicioDto? OrdenSeleccionada
{
    get => _ordenSeleccionada;
    set
    {
        _ordenSeleccionada = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(PuedePagar));
        OnPropertyChanged(nameof(PuedeVerComprobantePago));
    }
}

public bool PuedePagar => OrdenSeleccionada != null && OrdenSeleccionada.Estado == "Completado";
public bool PuedeVerComprobantePago => OrdenSeleccionada != null && OrdenSeleccionada.Estado == "Pagado";
```

### 1.2 `CtlOrdenServicio.xaml`
Update the action buttons to use the new bindings:
```xml
<Button Content="📄 Comp. Pago" Click="BtnComprobantePago_Click" IsEnabled="{Binding PuedeVerComprobantePago}" ... />
<Button Content="💳 Pagar" Click="BtnPagar_Click" IsEnabled="{Binding PuedePagar}" ... />
```
*(Remove any manual code-behind checks that show popups for these conditions, as they are now handled by the UI).*

## 2. Service Changes

### 2.1 Database Constraints (SQL)
To ensure data integrity, a database migration/script is required to alter the `Factura` table:
1. Ensure the `Numero` column is an auto-incrementing identity field.
2. Add a `UNIQUE` constraint to the `RegistroServicioId` column to prevent duplicate invoices for the same order.

### 2.2 `BillingService.cs` - `GenerarFactura`
Update the logic to prevent duplicates in code and let the database handle the invoice number generation:
```csharp
public async Task<Factura> GenerarFactura(int registroServicioId)
{
    // 1. Check if invoice already exists
    var existingFactura = await _context.Facturas
        .FirstOrDefaultAsync(f => f.RegistroServicioId == registroServicioId);
    
    if (existingFactura != null)
    {
        return existingFactura;
    }

    var detalles = await _context.DetalleServicios
        .Where(d => d.RegistroServicioId == registroServicioId)
        .ToListAsync();

    decimal total = detalles.Sum(d => d.Precio * d.Cantidad);

    // 2. Let the database generate the sequential invoice number
    var factura = new Factura
    {
        RegistroServicioId = registroServicioId,
        // Numero is omitted or ignored to let DB IDENTITY handle it
        Fecha = DateTime.Now,
        Total = total
    };

    await _facturaRepo.Create(factura);

    return factura;
}
```

### 2.2 Payment Validation (Optional but Recommended)
In `RegistrarPago`, ensure that the order is actually in a state that allows payments (e.g., "Completado") before processing, to prevent API-level abuse even if the UI restricts it.

## 3. Testing Plan
1. **UI Tests:** Select orders in different states ("Pendiente", "En Proceso", "Completado", "Pagado") and verify the "Pagar" and "Comp. Pago" buttons enable/disable correctly.
2. **Invoice Generation:** Attempt to pay an order multiple times (or generate an invoice twice) and verify that the same invoice number is retrieved and no duplicates are created.
3. **Sequential Numbers:** Create two new invoices and verify their numbers are strictly sequential without collisions.
