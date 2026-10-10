# Technical Design: Button States and Billing Fixes

## 1. Database Layer
The `Factura` table will be altered to enforce two critical business rules at the database level:
- **Auto-increment Invoice Numbers:** The `Numero` column will be configured to automatically generate sequential IDs upon insert (e.g., using `IDENTITY(1,1)` in SQL Server).
- **Unique Invoices per Order:** A `UNIQUE` constraint will be added to the `RegistroServicioId` column in the `Factura` table. This acts as a strict guard against the generation of multiple invoices for a single service order, avoiding double billing scenarios under concurrency.

## 2. Data Access & Services Layer
- **Entity Framework Core adjustments:** Depending on the current entity configuration, the `Factura` entity mapping might need to explicitly configure the `Numero` property to be `DatabaseGeneratedOption.Identity`, ensuring EF doesn't attempt to insert explicit values for it unless needed.
- **`BillingService.GenerarFactura`:**
  - Will query the database before creation to return an existing `Factura` if one already exists for the `RegistroServicioId`.
  - The object creation of `Factura` will no longer assign `Numero = (int)(DateTime.Now.Ticks % int.MaxValue)`.
  - Exception handling can be added in case the unique constraint is violated due to race conditions, mapping it to a friendly error or re-fetching the newly created invoice.

## 3. Presentation Layer
- **`OrdenServicioViewModel`:**
  - Introduce calculated boolean properties `PuedePagar` and `PuedeVerComprobantePago`.
  - Ensure the setter of `OrdenSeleccionada` invokes `OnPropertyChanged` for these new properties so the UI evaluates them immediately upon row selection.
- **`CtlOrdenServicio.xaml`:**
  - Data-bind the `IsEnabled` properties of the "Comp. Pago" and "Pagar" buttons to the new ViewModel properties.
  - This declarative UI approach simplifies the imperative C# code-behind and improves user experience by hiding/disabling invalid actions upfront.
