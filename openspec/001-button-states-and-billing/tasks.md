# Task Breakdown: Button States and Billing Fixes

- [ ] **Task 1: Database Migration / SQL Alterations**
  - Create and apply a SQL script to alter the `Factura` table.
  - Set the `Numero` column to be an `IDENTITY` (auto-incrementing) field.
  - Add a `UNIQUE` constraint to the `RegistroServicioId` column.

- [ ] **Task 2: Update EF Core Entity Configuration (if applicable)**
  - Ensure `Factura.Numero` is decorated with `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]` or configured via Fluent API so EF Core allows the DB to generate the value.

- [ ] **Task 3: Refactor `BillingService.cs`**
  - Update `GenerarFactura` to check for an existing invoice by `RegistroServicioId` before creating a new one.
  - Remove the manual `DateTime.Ticks` logic for `Numero`.

- [ ] **Task 4: Update `OrdenServicioViewModel.cs`**
  - Implement the `PuedePagar` property (enabled only when state is "Completado").
  - Implement the `PuedeVerComprobantePago` property (enabled only when state is "Pagado").
  - Refactor the `OrdenSeleccionada` property to trigger `OnPropertyChanged` for the new properties.

- [ ] **Task 5: Update `CtlOrdenServicio.xaml` UI bindings**
  - Bind the `IsEnabled` attribute of the "Comp. Pago" button to `PuedeVerComprobantePago`.
  - Bind the `IsEnabled` attribute of the "Pagar" button to `PuedePagar`.
  - Remove any redundant warning popups in the code-behind for these validations.

- [ ] **Task 6: Testing & Validation**
  - Verify UI button states match the selected order's state.
  - Verify sequential invoice number generation.
  - Attempt concurrent generation (or deliberate double clicks) to verify the DB UNIQUE constraint holds and duplicates are avoided.
