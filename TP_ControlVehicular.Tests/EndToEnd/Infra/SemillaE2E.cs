using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Entidad;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    public class SemillaE2EResult
    {
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public int TallerId { get; set; }
        public int RecepcionistaId { get; set; }
        public int MecanicoId { get; set; }
        public int RegistroServicioId { get; set; }
        public int FacturaId { get; set; }
    }

    public static class SemillaE2E
    {
        public static async Task<SemillaE2EResult> SembrarFlujoBasicoAsync(CVDbContext ctx)
        {
            var marker = TestRunContext.RunMarker;
            var now = DateTime.Now;

            var cliente = new Cliente
            {
                Dni = $"TEST{marker.GetHashCode():X}",
                Nombre = $"Cliente {marker}",
                Apellido = "Test",
                Telefono = "123456789",
                Email = $"test{marker}@local",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Observaciones = marker
            };
            await ctx.Clientes.AddAsync(cliente);
            await ctx.SaveChangesAsync();

            var marca = await ctx.Marcas.FirstOrDefaultAsync();
            var modelo = await ctx.Modelos.FirstOrDefaultAsync(m => m.MarcaId == marca!.Id);
            var vehiculo = new Vehiculo
            {
                Patente = $"AAA{marker.GetHashCode():X}".Substring(0, 6),
                Anio = 2020,
                ModeloId = modelo!.Id,
                Observaciones = marker
            };
            await ctx.Vehiculos.AddAsync(vehiculo);
            await ctx.SaveChangesAsync();

            var propietario = new PropietarioVehiculo
            {
                VehiculoId = vehiculo.Id,
                ClienteId = cliente.Id,
                EsActual = true,
                FechaDesde = now,
                Observaciones = marker
            };
            await ctx.PropietariosVehiculos.AddAsync(propietario);
            await ctx.SaveChangesAsync();

            var taller = await ctx.Talleres.FirstOrDefaultAsync();
            if (taller == null)
            {
                taller = new Taller { Nombre = $"Taller {marker}", Direccion = "Dir", Telefono = "123", Observaciones = marker, Activo = true };
                await ctx.Talleres.AddAsync(taller);
                await ctx.SaveChangesAsync();
            }

            var recepcionista = await ctx.Usuarios.FirstOrDefaultAsync(u => u.RolId == (int)RolesSistema.Recepcionista);
            var mecanico = await ctx.Usuarios.FirstOrDefaultAsync(u => u.RolId == (int)RolesSistema.Mecanico);
            if (mecanico == null)
            {
                mecanico = new Usuario { Dni = $"MEC{marker.GetHashCode():X}", Nombre = "Mec", Apellido = "Test", Contrasena = "123", RolId = (int)RolesSistema.Mecanico, Activo = true, Observaciones = marker };
                await ctx.Usuarios.AddAsync(mecanico);
                await ctx.SaveChangesAsync();
            }

            var servicio = await ctx.Servicios.FirstOrDefaultAsync();
            var registro = new RegistroServicio
            {
                VehiculoId = vehiculo.Id,
                TallerId = taller.Id,
                UsuarioId = recepcionista!.Id,
                Fecha = now,
                KmIngreso = 1000,
                Estado = RegistroServicio.EstadoEnProceso,
                Observaciones = marker
            };
            await ctx.RegistroServicios.AddAsync(registro);
            await ctx.SaveChangesAsync();

            var detalle = new DetalleServicio
            {
                RegistroServicioId = registro.Id,
                UsuarioId = mecanico.Id,
                ServicioId = servicio!.Id,
                Cantidad = 1,
                Precio = 100.00m,
                OrdenEjecucion = 1,
                Origen = "Taller",
                Estado = DetalleServicio.EstadoEnCurso,
                Observaciones = marker
            };
            await ctx.DetalleServicios.AddAsync(detalle);
            await ctx.SaveChangesAsync();

            return new SemillaE2EResult
            {
                ClienteId = cliente.Id,
                VehiculoId = vehiculo.Id,
                TallerId = taller.Id,
                RecepcionistaId = recepcionista.Id,
                MecanicoId = mecanico.Id,
                RegistroServicioId = registro.Id
            };
        }
    }
}