# Tasks: Independent Ownership (PropietarioVehiculo)

- [x] **Task 1: Actualizar Entidades Base**
  - Crear Entidad/PropietarioVehiculo.cs.
  - Modificar Entidad/Vehiculo.cs quitando ClienteId y Cliente, y agregando ICollection<PropietarioVehiculo> Propietarios.
  - Modificar Entidad/Cliente.cs agregando ICollection<PropietarioVehiculo> Propietarios.

- [x] **Task 2: Configurar DBContext y Fluent API**
  - Agregar DbSet<PropietarioVehiculo> en Datos/Data/CVDbContext.cs.
  - Configurar las relaciones en Datos/Configuracion/ClienteConfiguration.cs y VehiculoConfiguration.cs (borrar configuraciones viejas de uno a muchos y mapear la nueva tabla intermedia si es necesario).

- [x] **Task 3: Refactorizar Repositorios**
  - En Datos/Repositories/VehiculoRepository.cs, cambiar los .Include(v => v.Cliente) por .Include(v => v.Propietarios).ThenInclude(p => p.Cliente).
  - En Datos/Repositories/RegistroServicioRepository.cs, cambiar los .ThenInclude(v => v.Cliente) por .ThenInclude(v => v.Propietarios).ThenInclude(p => p.Cliente).

- [x] **Task 4: Refactorizar AutoMapper y Handlers**
  - En Negocio/Mappers/MappingProfile.cs, mapear la obtención del cliente buscando en Propietarios el que tenga EsActual == true.
  - En Negocio/Services/ObtenerReporteOrdenesHandler.cs, actualizar la forma en que se extrae el nombre y DNI del cliente del vehículo (usando el PropietarioActual).

- [x] **Task 5: UI y Listado de Vehículos**
  - Asegurar que ListarVehiculosHandler.cs (si existe y no usa AutoMapper directo) devuelva correctamente los datos.
  - En Presentacion/Pantalla/Vehiculo/CtlVehiculo.xaml, corregir las líneas del DataGrid (GridLinesVisibility="Horizontal" sin divisiones verticales).





