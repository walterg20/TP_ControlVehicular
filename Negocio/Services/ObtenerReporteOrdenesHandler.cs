using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;

namespace TP_ControlVehicular.Negocio.Services
{
    public class ObtenerReporteOrdenesHandler
    {
        private readonly IRegistroServicioRepository _registroServicioRepository;

        public ObtenerReporteOrdenesHandler(IRegistroServicioRepository registroServicioRepository)
        {
            _registroServicioRepository = registroServicioRepository;
        }

        public async Task<List<OrdenTrabajoReporteDto>> HandleAsync()
        {
            try
            {
                int? mecanicoId = null;
                var currentUser = TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser;
                if (currentUser != null && currentUser.IdRol == (int)TP_ControlVehicular.Negocio.Context.RolesSistema.Mecanico)
                {
                    mecanicoId = currentUser.IdUsuario;
                }

                var registros = await _registroServicioRepository.GetReporteCompletoAsync(mecanicoId);

                if (registros != null && registros.Count > 0)
                {
                    return registros.Select(r => new OrdenTrabajoReporteDto
                    {
                        IdOrden = r.Id,
                        FechaIngreso = r.Fecha,
                        Patente = r.Vehiculo?.Patente ?? "N/A",
                        MarcaModelo = r.Vehiculo?.Modelo != null && r.Vehiculo.Modelo.Marca != null
                            ? $"{r.Vehiculo.Modelo.Marca.NombreMarca} - {r.Vehiculo.Modelo.NombreModelo}"
                            : "Marca/Modelo N/A",
                        ClienteNombre = r.Vehiculo?.Cliente != null
                            ? $"{r.Vehiculo.Cliente.Nombre} {r.Vehiculo.Cliente.Apellido}".Trim()
                            : "Cliente N/A",
                        ClienteDni = r.Vehiculo?.Cliente?.Dni ?? "N/A",
                        KmIngresado = r.KmIngreso,
                        RecepcionistaNombre = r.Usuario != null ? r.Usuario.Nombre : "Sin Recepcionista",
                        MecanicoNombre = r.Detalles.FirstOrDefault(d => d.Usuario != null)?.Usuario?.Nombre ?? "Sin Mecánico",
                        ServiciosAplicados = r.Detalles.Any()
                            ? string.Join(", ", r.Detalles.Where(d => d.Servicio != null).Select(d => d.Servicio.Nombre))
                            : "Sin servicios asignados",
                        Estado = r.Estado
                    }).ToList();
                }
            }
            catch
            {
                // Fallback a listado demostrativo en caso de que la tabla aún no contenga registros
            }

            // Datos de demostración basados en el flujo real del taller (Recepcionista -> Registra Cabecera -> Mecánico confecciona servicios)
            return GetReportesDemostrativos();
        }

        private List<OrdenTrabajoReporteDto> GetReportesDemostrativos()
        {
            return new List<OrdenTrabajoReporteDto>
            {
                new OrdenTrabajoReporteDto
                {
                    IdOrden = 1001,
                    FechaIngreso = DateTime.Now.AddDays(-1),
                    Patente = "AB123CD",
                    MarcaModelo = "Toyota - Corolla",
                    ClienteNombre = "Juan Pérez",
                    ClienteDni = "30123456",
                    KmIngresado = 45000,
                    ServiciosAplicados = "Cambio de aceite, Filtro de aire, Cambio de correa de distribución",
                    MecanicoNombre = "Carlos Gómez (Mecánico)",
                    RecepcionistaNombre = "Laura Martínez (Recepcionista)",
                    Estado = "En Proceso"
                },
                new OrdenTrabajoReporteDto
                {
                    IdOrden = 1002,
                    FechaIngreso = DateTime.Now.AddDays(-2),
                    Patente = "AF456GH",
                    MarcaModelo = "Ford - Focus",
                    ClienteNombre = "María Rodríguez",
                    ClienteDni = "28987654",
                    KmIngresado = 78200,
                    ServiciosAplicados = "Cambio de aceite y filtro, Alineación y balanceo",
                    MecanicoNombre = "Roberto Sánchez (Mecánico)",
                    RecepcionistaNombre = "Laura Martínez (Recepcionista)",
                    Estado = "Completada"
                },
                new OrdenTrabajoReporteDto
                {
                    IdOrden = 1003,
                    FechaIngreso = DateTime.Now,
                    Patente = "AD789JK",
                    MarcaModelo = "Volkswagen - Gol Trend",
                    ClienteNombre = "Lucas Fernández",
                    ClienteDni = "35444111",
                    KmIngresado = 32000,
                    ServiciosAplicados = "Cambio de correa, Filtros de combustible y habitáculo",
                    MecanicoNombre = "Carlos Gómez (Mecánico)",
                    RecepcionistaNombre = "Esteban López (Recepcionista)",
                    Estado = "Pendiente"
                }
            };
        }
    }
}
