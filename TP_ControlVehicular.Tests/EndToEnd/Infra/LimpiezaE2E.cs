using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public static class LimpiezaE2E
    {
        /// <summary>
        /// Elimina los datos sembrados por <see cref="SemillaE2E"/> en el orden que respeta
        /// las claves foráneas y el trigger <c>TR_DetalleServicio_BloquearModificacionFinalizada</c>
        /// (que impide borrar detalles si la orden ya fue facturada).
        /// Orden: Pago → Factura → DetalleServicio → RegistroServicio → PropietarioVehiculo →
        /// Vehiculo → Cliente → Taller → Usuario.
        /// </summary>
        public static async Task LimpiarPorMarcaAsync(CVDbContext ctx)
        {
            var marker = TestRunContext.RunMarker;
            var patente = MarcadorE2E.PatenteVehiculo(marker);

            var registroIds = await ctx.RegistroServicios
                .Where(r => r.Vehiculo.Patente == patente)
                .Select(r => r.Id)
                .ToListAsync();

            var facturaIds = await ctx.Facturas
                .Where(f => registroIds.Contains(f.RegistroServicioId))
                .Select(f => f.Id)
                .ToListAsync();

            var pagos = await ctx.Pagos.Where(p => facturaIds.Contains(p.FacturaId)).ToListAsync();
            ctx.Pagos.RemoveRange(pagos);
            await ctx.SaveChangesAsync();

            var facturas = await ctx.Facturas.Where(f => facturaIds.Contains(f.Id)).ToListAsync();
            ctx.Facturas.RemoveRange(facturas);
            await ctx.SaveChangesAsync();

            var detalles = await ctx.DetalleServicios.Where(d => registroIds.Contains(d.RegistroServicioId)).ToListAsync();
            ctx.DetalleServicios.RemoveRange(detalles);
            await ctx.SaveChangesAsync();

            var registros = await ctx.RegistroServicios.Where(r => registroIds.Contains(r.Id)).ToListAsync();
            ctx.RegistroServicios.RemoveRange(registros);
            await ctx.SaveChangesAsync();

            var propietarios = await ctx.PropietariosVehiculos.Where(p => p.Vehiculo.Patente == patente).ToListAsync();
            ctx.PropietariosVehiculos.RemoveRange(propietarios);
            await ctx.SaveChangesAsync();

            var vehiculos = await ctx.Vehiculos.Where(v => v.Patente == patente).ToListAsync();
            ctx.Vehiculos.RemoveRange(vehiculos);
            await ctx.SaveChangesAsync();

            var nombreCliente = MarcadorE2E.NombreCliente(marker);
            var clientes = await ctx.Clientes.Where(c => c.Nombre == nombreCliente).ToListAsync();
            ctx.Clientes.RemoveRange(clientes);
            await ctx.SaveChangesAsync();

            var nombreTaller = MarcadorE2E.NombreTaller(marker);
            var talleres = await ctx.Talleres.Where(t => t.Nombre == nombreTaller).ToListAsync();
            ctx.Talleres.RemoveRange(talleres);
            await ctx.SaveChangesAsync();

            var apellidoUsuario = MarcadorE2E.ApellidoUsuario(marker);
            var usuarios = await ctx.Usuarios.Where(u => u.Apellido == apellidoUsuario).ToListAsync();
            ctx.Usuarios.RemoveRange(usuarios);
            await ctx.SaveChangesAsync();
        }
    }
}
