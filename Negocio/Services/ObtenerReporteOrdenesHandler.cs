using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Context;

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

                if (registros == null || registros.Count == 0)
                {
                    return new List<OrdenTrabajoReporteDto>();
                }

                return registros.Select(r => new OrdenTrabajoReporteDto
                {
                    IdOrden = r.Id,
                    FechaIngreso = r.Fecha,
                    Patente = r.Vehiculo?.Patente ?? "N/A",
                    MarcaModelo = r.Vehiculo?.Modelo != null && r.Vehiculo.Modelo.Marca != null
                        ? $"{r.Vehiculo.Modelo.Marca.NombreMarca} - {r.Vehiculo.Modelo.NombreModelo}"
                        : "Marca/Modelo N/A",
                    ClienteNombre = r.Vehiculo?.Propietarios.FirstOrDefault(p => p.EsActual)?.Cliente != null
                        ? $"{r.Vehiculo.Propietarios.FirstOrDefault(p => p.EsActual)!.Cliente.Nombre} {r.Vehiculo.Propietarios.FirstOrDefault(p => p.EsActual)!.Cliente.Apellido}".Trim()
                        : "Cliente N/A",
                    ClienteDni = r.Vehiculo?.Propietarios.FirstOrDefault(p => p.EsActual)?.Cliente?.Dni ?? "N/A",
                    KmIngresado = r.KmIngreso,
                    RecepcionistaNombre = r.Usuario != null ? r.Usuario.Nombre : "Sin Recepcionista",
                    MecanicoNombre = r.Detalles.FirstOrDefault(d => d.Usuario != null)?.Usuario?.Nombre ?? "Sin Mecánico",
                    ServiciosAplicados = r.Detalles.Any()
                        ? string.Join(", ", r.Detalles.Where(d => d.Servicio != null).Select(d => d.Servicio.Nombre))
                        : "Sin servicios asignados",
                    Estado = r.Estado
                }).ToList();
            }
            catch
            {
                // Sin datos demostrativos: ante un error el reporte se muestra vacío.
                return new List<OrdenTrabajoReporteDto>();
            }
        }
    }
}
