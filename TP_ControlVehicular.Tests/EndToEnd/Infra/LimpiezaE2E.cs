using System.Linq;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public static class LimpiezaE2E
    {
        public static async System.Threading.Tasks.Task LimpiarPorMarcaAsync(CVDbContext ctx)
        {
            var marker = TestRunContext.RunMarker;
            var detalles = await ctx.DetalleServicios.Where(d => d.Observaciones == marker).ToListAsync();
            ctx.DetalleServicios.RemoveRange(detalles);
            await ctx.SaveChangesAsync();

            var registros = await ctx.RegistroServicios.Where(r => r.Observaciones == marker).ToListAsync();
            ctx.RegistroServicios.RemoveRange(registros);
            await ctx.SaveChangesAsync();

            var pagos = await ctx.Pagos.Where(p => p.Factura != null && p.Factura.RegistroServicio != null && p.Factura.RegistroServicio.Observaciones == marker).ToListAsync();
            ctx.Pagos.RemoveRange(pagos);
            await ctx.SaveChangesAsync();

            var facturas = await ctx.Facturas.Where(f => f.RegistroServicio != null && f.RegistroServicio.Observaciones == marker).ToListAsync();
            ctx.Facturas.RemoveRange(facturas);
            await ctx.SaveChangesAsync();

            var propietarios = await ctx.PropietariosVehiculos.Where(p => p.Observaciones == marker).ToListAsync();
            ctx.PropietariosVehiculos.RemoveRange(propietarios);
            await ctx.SaveChangesAsync();

            var vehiculos = await ctx.Vehiculos.Where(v => v.Observaciones == marker).ToListAsync();
            ctx.Vehiculos.RemoveRange(vehiculos);
            await ctx.SaveChangesAsync();

            var clientes = await ctx.Clientes.Where(c => c.Observaciones == marker).ToListAsync();
            ctx.Clientes.RemoveRange(clientes);
            await ctx.SaveChangesAsync();

            var talleres = await ctx.Talleres.Where(t => t.Observaciones == marker).ToListAsync();
            ctx.Talleres.RemoveRange(talleres);
            await ctx.SaveChangesAsync();

            var usuarios = await ctx.Usuarios.Where(u => u.Observaciones == marker).ToListAsync();
            ctx.Usuarios.RemoveRange(usuarios);
            await ctx.SaveChangesAsync();
        }
    }
}