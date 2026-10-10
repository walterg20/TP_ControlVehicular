# Technical Design: ABM Servicio Implementation

## Architecture & Class Design

```
+-----------------------------------------------------------------------------------+
|                                   PRESENTACION                                    |
|                                                                                   |
|  +-----------------------+     Open Modal     +-------------------------------+  |
|  |     CtlServicio       | -----------------> |          FrmServicio          |  |
|  +-----------------------+                    +-------------------------------+  |
|              |                                                |                   |
|              +-----------------------+------------------------+                   |
|                                      |                                            |
|                                      v                                            |
|                       +-------------------------------+                           |
|                       |       ServicioViewModel       |                           |
|                       +-------------------------------+                           |
+--------------------------------------|--------------------------------------------+
                                       |
                                       v
+-----------------------------------------------------------------------------------+
|                                     NEGOCIO                                       |
|  +-----------------------------------------------------------------------------+  |
|  |   Listar / Registrar / Modificar / EliminarServicioHandler                  |  |
|  +-----------------------------------------------------------------------------+  |
|                                        |                                          |
|                                        v                                          |
|  +-----------------------------------------------------------------------------+  |
|  |                               ServicioDto                                   |  |
|  +-----------------------------------------------------------------------------+  |
+----------------------------------------|------------------------------------------+
                                         |
                                         v
+-----------------------------------------------------------------------------------+
|                                      DATOS                                        |
|  +-----------------------+                     +-------------------------------+  |
|  |  IServicioRepository  | <-----------------> |      ServicioRepository       |  |
|  +-----------------------+                     +-------------------------------+  |
+-----------------------------------------------------------------------------------+
```

## DTOs & Handlers
- **`ServicioDto.cs`**:
  - `int IdServicio`
  - `string Nombre`
  - `decimal Precio`
  - `bool Activo`
- **Handlers**:
  - `ListarServiciosHandler.cs`
  - `RegistrarServicioHandler.cs`
  - `ModificarServicioHandler.cs`
  - `EliminarServicioHandler.cs`

## UI & Styling Guidelines
- `CtlServicio.xaml`: UserControl with DataGrid listing services, filter text box, and buttons `"Nuevo"`, `"Editar"`, `"Borrar"`.
- `FrmServicio.xaml`: Modal window centered over owner window with `CornerRadius="12"` (`rounded-xl`), `shadow-md` styling, validating Name and Price.
