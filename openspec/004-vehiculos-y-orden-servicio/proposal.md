# Proposal: Fix Vehicle List and Implement Independent Ownership

## 1. Context
In the Vehicle list menu (CtlVehiculo), the columns for Client, Brand, and Model are currently empty. 
Simultaneously, the business requires the ability to register vehicles without an assigned client (orphan vehicles) so that a workshop owner can import a pre-existing fleet, and preserve ownership history if a vehicle is sold.

## 2. Proposed Changes

### 2.1. Database Restructuring (No Data Loss)
- Create a new intermediate table PropietarioVehiculo mapping Cliente and Vehiculo with FechaAdquisicion, FechaVenta, and EsActual.
- Migrate existing ClienteId values from Vehiculo to PropietarioVehiculo.
- Drop ClienteId from the Vehiculo table.

### 2.2. EF Core Entities & DTOs
- Add PropietarioVehiculo entity.
- Remove ClienteId and Cliente navigation properties from Vehiculo.
- Add a collection Propietarios to Vehiculo.
- Update VehiculoDto and AutoMapper to resolve the "Current Client" by querying the active PropietarioVehiculo.

### 2.3. Fix Vehicle List Data (UI)
- Update ListarVehiculosHandler.cs to fetch Vehiculos including Propietarios (and the nested Cliente) and Modelo.Marca.
- Bind the datagrid so it shows the "current owner" correctly (or blank if orphan).
- Modify the DataGrid in CtlVehiculo to ensure it uses horizontal grid lines (matching UI standards).

## 3. Impact
- **Database:** Solves the historical ownership issue and allows orphan vehicles. Data is preserved via SQL migration script.
- **UI:** The CtlVehiculo DataGrid will display complete vehicle information, improving user experience and data visibility.
