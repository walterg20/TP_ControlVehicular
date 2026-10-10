# Technical Design: Role-Based Restrictions & Work Orders Report (Approved ER Schema)

## Architecture & Data Flow

```
+-----------------------------------------------------------------------------------+
|                                   PRESENTACION                                    |
|                                                                                   |
|  +-----------------------+     Menu Filter     +-------------------------------+  |
|  |      MainWindow       | ------------------> | AplicarRestriccionesPorRol()  |  |
|  +-----------------------+                     +-------------------------------+  |
|              |                                                                    |
|              v                                                                    |
|  +-----------------------+                     +-------------------------------+  |
|  |  CtlReporteOrdenes    | <-----------------> |   ReporteOrdenesViewModel     |  |
|  +-----------------------+                     +-------------------------------+  |
+----------------------------------------------------------------|------------------+
                                                                 |
                                                                 v
+-----------------------------------------------------------------------------------+
|                                     NEGOCIO                                       |
|  +-----------------------------------------------------------------------------+  |
|  |                       ObtenerReporteOrdenesHandler                          |  |
|  +-----------------------------------------------------------------------------+  |
|                                        |                                          |
|                                        v                                          |
|  +-----------------------------------------------------------------------------+  |
|  |                         OrdenTrabajoReporteDto                              |  |
|  +-----------------------------------------------------------------------------+  |
+----------------------------------------|------------------------------------------+
                                         |
                                         v
+-----------------------------------------------------------------------------------+
|                                      DATOS                                        |
|  +---------------------------+                 +-------------------------------+  |
|  | IRegistroServicioRepository| <-------------> |  RegistroServicioRepository   |  |
|  +---------------------------+                 +-------------------------------+  |
+-----------------------------------------------------------------------------------+
```

## Approved ER Schema Alignment

### 1. `RegistroServicio` (Header)
- `Id`: int (PK)
- `VehiculoId`: int (FK -> `Vehiculo`)
- `TallerId`: int (FK -> `Taller`)
- `UsuarioId`: int (FK -> `Usuario` - **Recepciona**)
- `Fecha`: DateTime
- `KmIngreso`: int
- `Estado`: string / enum

### 2. `DetalleServicio` (Details)
- `Id`: int (PK)
- `RegistroServicioId`: int (FK -> `RegistroServicio`)
- `UsuarioId`: int (FK -> `Usuario` - **Confecciona / Mecánico**)
- `ServicioId`: int (FK -> `Servicio`)
- `Cantidad`: int
- `Precio`: decimal
- `Origen`: string
- `Estado`: string / enum

### 3. `Servicio` (Catalog)
- `Id`: int (PK)
- `Nombre`: string (e.g. "Cambio de aceite y filtro", "Cambio de correa", "Filtros")
- `Precio`: decimal
- `Activo`: bool

### 4. `OrdenTrabajoReporteDto` (Reporting DTO)
- `IdRegistro`: int
- `Fecha`: DateTime
- `Patente`: string
- `MarcaModelo`: string
- `ClienteNombre`: string
- `ClienteDni`: string
- `KmIngreso`: int
- `RecepcionistaNombre`: string (from `RegistroServicio.Usuario.Nombre`)
- `MecanicoNombre`: string (from `DetalleServicio.Usuario.Nombre`)
- `ServiciosAplicados`: string (aggregated from `DetalleServicio.Servicio.Nombre`)
- `Estado`: string

## Role-Based Access Control Rules
- `Administrador`: All buttons visible (`Dashboard`, `Operations`, `Administration`, `Reports`, `Users`, `Roles`).
- `Recepcionista`: Visible: `Dashboard`, `Operations`, `Customers`, `Vehicles`, `Reports`. Hidden: `Users`, `Roles`, `Workshops`.
- `Mecanico`: Visible: `Dashboard`, `Work Orders`, `Reports`. Hidden: `Users`, `Roles`, `Workshops`, `Customers`, `Vehicles`.
