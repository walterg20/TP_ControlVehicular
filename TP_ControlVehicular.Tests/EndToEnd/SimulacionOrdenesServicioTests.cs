// walterg20
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TP_ControlVehicular.Datos.Data;
using TP_ControlVehicular.Datos.Repositories;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Tests.EndToEnd.Infra;
using Xunit;

namespace TP_ControlVehicular.Tests.EndToEnd
{
    /// <summary>
    /// Test automatizado de simulación integral que reproduce el flujo real de
    /// operación del taller vehicular:
    /// - Creación de clientes con sus vehículos vinculados.
    /// - Creación de 15 órdenes de servicio distribuidas en asignaciones de 1, 2 y 3 mecánicos.
    /// - Transición a través de los diversos estados canónicos (Abierta, En Proceso, Completada, Pagada, Cancelada).
    /// - Emisión de factura y procesamiento de pagos con cálculo de vuelto a través de IBillingService.
    /// </summary>
    [Collection("e2e-sql")]
    public class SimulacionOrdenesServicioTests : IAsyncLifetime
    {
        private readonly CVDbContext _ctx;
        private readonly ContextoTestFactoryAdapter _factory;
        private const string PrefijoSimulacion = "SIM";
        private readonly List<string> _patentesCreadas = new();
        private readonly List<string> _dnisClientesCreados = new();
        private readonly List<int> _registroIdsCreados = new();

        public SimulacionOrdenesServicioTests()
        {
            _ctx = ContextoTestFactory.Create();
            _factory = new ContextoTestFactoryAdapter(TestRunContext.ConnectionString);
        }

        public async Task InitializeAsync()
        {
            await LimpiarDatosSimulacionAsync();
        }

        public async Task DisposeAsync()
        {
            await LimpiarDatosSimulacionAsync();
            await _ctx.DisposeAsync();
        }

        [RequiereSqlServerFact]
        public async Task SimularCreacionDe15OrdenesConMecanicosYFacturacion_EjecutaFlujoCompleto()
        {
            // -----------------------------------------------------------------
            // 1. PRERREQUISITOS DEL SISTEMA
            // -----------------------------------------------------------------
            var taller = await _ctx.Talleres.FirstOrDefaultAsync(t => t.Activo);
            if (taller == null)
            {
                taller = new Taller
                {
                    Nombre = "Taller Central de Prueba",
                    Direccion = "Av. San Martín 1500",
                    Telefono = "3624123456",
                    Activo = true
                };
                await _ctx.Talleres.AddAsync(taller);
                await _ctx.SaveChangesAsync();
            }

            var recepcionista = await _ctx.Usuarios.FirstOrDefaultAsync(u => u.RolId == (int)RolesSistema.Recepcionista && u.Activo);
            if (recepcionista == null)
            {
                recepcionista = new Usuario
                {
                    Nombre = "Laura",
                    Apellido = "Fernández",
                    Dni = "89999001",
                    Email = "recepcion.auto@taller.local",
                    Telefono = "3624000101",
                    Domicilio = "Calle Mitre 350",
                    FechaNacimiento = new DateTime(1988, 5, 10),
                    Contrasena = "123",
                    RolId = (int)RolesSistema.Recepcionista,
                    Activo = true
                };
                await _ctx.Usuarios.AddAsync(recepcionista);
                await _ctx.SaveChangesAsync();
            }

            // Asegurar 3 mecánicos activos (RolId = 3)
            var mecanicos = await _ctx.Usuarios
                .Where(u => u.RolId == (int)RolesSistema.Mecanico && u.Activo)
                .OrderBy(u => u.Id)
                .Take(3)
                .ToListAsync();

            string[] nombresMecanicos = { "Carlos Benítez", "Marcos Romero", "Lucas Almirón" };
            while (mecanicos.Count < 3)
            {
                int index = mecanicos.Count;
                var nuevoMecanico = new Usuario
                {
                    Nombre = nombresMecanicos[index].Split(' ')[0],
                    Apellido = nombresMecanicos[index].Split(' ')[1],
                    Dni = $"8999901{index + 1}",
                    Email = $"mecanico{index + 1}.auto@taller.local",
                    Telefono = $"362400020{index + 1}",
                    Domicilio = "Sección Taller",
                    FechaNacimiento = new DateTime(1990 + index, 2, 15),
                    Contrasena = "123",
                    RolId = (int)RolesSistema.Mecanico,
                    Activo = true
                };
                await _ctx.Usuarios.AddAsync(nuevoMecanico);
                await _ctx.SaveChangesAsync();
                mecanicos.Add(nuevoMecanico);
            }

            var mecanico1 = mecanicos[0];
            var mecanico2 = mecanicos[1];
            var mecanico3 = mecanicos[2];

            // Asegurar catálogo de servicios
            var servicios = await _ctx.Servicios.Where(s => s.Activo).Take(6).ToListAsync();
            if (servicios.Count < 4)
            {
                var serviciosBase = new[]
                {
                    new Servicio { Nombre = "Cambio de Aceite y Filtro", Precio = 45000m, Activo = true },
                    new Servicio { Nombre = "Alineación y Balanceo Computarizado", Precio = 38000m, Activo = true },
                    new Servicio { Nombre = "Revisión y Pastillas de Freno", Precio = 52000m, Activo = true },
                    new Servicio { Nombre = "Diagnóstico Computarizado de Motor", Precio = 30000m, Activo = true }
                };
                foreach (var sb in serviciosBase)
                {
                    if (!await _ctx.Servicios.AnyAsync(s => s.Nombre == sb.Nombre))
                    {
                        await _ctx.Servicios.AddAsync(sb);
                    }
                }
                await _ctx.SaveChangesAsync();
                servicios = await _ctx.Servicios.Where(s => s.Activo).Take(6).ToListAsync();
            }

            var servicioAceite = servicios[0];
            var servicioAlineacion = servicios[1];
            var servicioFrenos = servicios[2];
            var servicioDiagnostico = servicios.Count > 3 ? servicios[3] : servicios[0];

            // Asegurar métodos de pago
            var metodoEfectivo = await _ctx.MetodosPago.FirstOrDefaultAsync(m => m.Nombre == "Efectivo");
            if (metodoEfectivo == null)
            {
                metodoEfectivo = new MetodoPago { Nombre = "Efectivo" };
                await _ctx.MetodosPago.AddAsync(metodoEfectivo);
                await _ctx.SaveChangesAsync();
            }

            var metodoTarjeta = await _ctx.MetodosPago.FirstOrDefaultAsync(m => m.Nombre == "Tarjeta");
            if (metodoTarjeta == null)
            {
                metodoTarjeta = new MetodoPago { Nombre = "Tarjeta" };
                await _ctx.MetodosPago.AddAsync(metodoTarjeta);
                await _ctx.SaveChangesAsync();
            }

            // Asegurar marca y modelo para los vehículos
            var modelo = await _ctx.Modelos.FirstOrDefaultAsync(m => m.Activo);
            if (modelo == null)
            {
                var marca = await _ctx.Marcas.FirstOrDefaultAsync(m => m.Activo);
                if (marca == null)
                {
                    marca = new Marca { NombreMarca = "Toyota", Activo = true };
                    await _ctx.Marcas.AddAsync(marca);
                    await _ctx.SaveChangesAsync();
                }
                modelo = new Modelo { MarcaId = marca.Id, NombreModelo = "Corolla", Activo = true };
                await _ctx.Modelos.AddAsync(modelo);
                await _ctx.SaveChangesAsync();
            }

            // -----------------------------------------------------------------
            // 2. CREACIÓN DE CLIENTES Y VEHÍCULOS VINCULADOS
            // -----------------------------------------------------------------
            var clientes = new List<Cliente>();
            var vehiculos = new List<Vehiculo>();

            string[] nombresClientes = { "Martín Gómez", "Lucía Navarro", "Esteban Quiroga", "Valeria Rossi", "Federico Silva", "Mariana Costa", "Gabriel Acosta", "Paula Méndez" };

            for (int i = 0; i < nombresClientes.Length; i++)
            {
                var partes = nombresClientes[i].Split(' ');
                var dni = $"850000{i:D2}";
                _dnisClientesCreados.Add(dni);

                var cliente = new Cliente
                {
                    Nombre = partes[0],
                    Apellido = partes[1],
                    Dni = dni,
                    Direccion = $"Av. Italia {1000 + i * 50}",
                    Email = $"cliente{i + 1}.sim@correo.com",
                    Telefono = $"36245550{i:D2}",
                    FechaNacimiento = new DateTime(1985 + i, 3, 10),
                    Activo = true
                };
                await _ctx.Clientes.AddAsync(cliente);
                await _ctx.SaveChangesAsync();
                clientes.Add(cliente);

                var patente = $"{PrefijoSimulacion}{i:D2}{new Random().Next(10, 99)}";
                _patentesCreadas.Add(patente);

                var vehiculo = new Vehiculo
                {
                    Patente = patente,
                    ModeloId = modelo.Id,
                    Anio = 2021 + (i % 4),
                    KmActual = 50000 + (i * 2000)
                };
                await _ctx.Vehiculos.AddAsync(vehiculo);
                await _ctx.SaveChangesAsync();
                vehiculos.Add(vehiculo);

                var titularidad = new PropietarioVehiculo
                {
                    ClienteId = cliente.Id,
                    VehiculoId = vehiculo.Id,
                    FechaAdquisicion = DateTime.Now.AddMonths(-12),
                    EsActual = true
                };
                await _ctx.PropietariosVehiculos.AddAsync(titularidad);
                await _ctx.SaveChangesAsync();
            }

            // -----------------------------------------------------------------
            // 3. GENERACIÓN DE LAS 15 ÓRDENES DE SERVICIO
            // -----------------------------------------------------------------
            var facturaRepo = new FacturaRepository(_factory);
            var pagoRepo = new PagoRepository(_factory);
            var metodoRepo = new MetodoPagoRepository(_factory);
            var billingService = new BillingService(_factory, facturaRepo, pagoRepo, metodoRepo);

            var ordenesCreadas = new List<RegistroServicio>();

            // --- GRUPO 1: Órdenes 1 a 5 (Asignadas a 1 solo Mecánico) ---

            // Orden 1: 1 mecánico (mecanico1), 1 servicio, Estado Abierta
            var orden1 = await CrearOrdenConDetallesAsync(vehiculos[0], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoAbierta, vehiculos[0].KmActual + 100,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoPendiente)
                });
            ordenesCreadas.Add(orden1);

            // Orden 2: 1 mecánico (mecanico1), 2 servicios, Estado En Proceso
            var orden2 = await CrearOrdenConDetallesAsync(vehiculos[0], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoEnProceso, vehiculos[0].KmActual + 200,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoEnCurso),
                    (servicioAlineacion, mecanico1, DetalleServicio.EstadoEnCurso)
                });
            ordenesCreadas.Add(orden2);

            // Orden 3: 1 mecánico (mecanico2), 1 servicio, Estado Completada
            var orden3 = await CrearOrdenConDetallesAsync(vehiculos[1], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCompletada, vehiculos[1].KmActual + 150,
                new (Servicio, Usuario, string)[]
                {
                    (servicioFrenos, mecanico2, DetalleServicio.EstadoFinalizada)
                });
            ordenesCreadas.Add(orden3);

            // Orden 4: 1 mecánico (mecanico3), 2 servicios, Estado Pagada (con Factura y Pago)
            var orden4 = await CrearOrdenConDetallesAsync(vehiculos[1], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCompletada, vehiculos[1].KmActual + 300,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico3, DetalleServicio.EstadoFinalizada),
                    (servicioDiagnostico, mecanico3, DetalleServicio.EstadoFinalizada)
                });
            var factura4 = await billingService.GenerarFactura(orden4.Id);
            await billingService.RegistrarPago(factura4.Id, metodoEfectivo.Id, factura4.Total + 5000m);
            ordenesCreadas.Add(orden4);

            // Orden 5: 1 mecánico (mecanico2), 1 servicio, Estado Cancelada
            var orden5 = await CrearOrdenConDetallesAsync(vehiculos[2], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCancelada, vehiculos[2].KmActual + 50,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAlineacion, mecanico2, DetalleServicio.EstadoPendiente)
                });
            ordenesCreadas.Add(orden5);

            // --- GRUPO 2: Órdenes 6 a 10 (Asignadas a 2 Mecánicos en simultáneo) ---

            // Orden 6: 2 mecánicos (mecanico1 y mecanico2), 2 servicios, Estado Abierta
            var orden6 = await CrearOrdenConDetallesAsync(vehiculos[2], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoAbierta, vehiculos[2].KmActual + 120,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoPendiente),
                    (servicioFrenos, mecanico2, DetalleServicio.EstadoPendiente)
                });
            ordenesCreadas.Add(orden6);

            // Orden 7: 2 mecánicos (mecanico1 y mecanico3), 2 servicios, Estado En Proceso
            var orden7 = await CrearOrdenConDetallesAsync(vehiculos[3], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoEnProceso, vehiculos[3].KmActual + 180,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAlineacion, mecanico1, DetalleServicio.EstadoEnCurso),
                    (servicioDiagnostico, mecanico3, DetalleServicio.EstadoPendiente)
                });
            ordenesCreadas.Add(orden7);

            // Orden 8: 2 mecánicos (mecanico2 y mecanico3), 2 servicios, Estado Completada
            var orden8 = await CrearOrdenConDetallesAsync(vehiculos[3], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCompletada, vehiculos[3].KmActual + 260,
                new (Servicio, Usuario, string)[]
                {
                    (servicioFrenos, mecanico2, DetalleServicio.EstadoFinalizada),
                    (servicioAceite, mecanico3, DetalleServicio.EstadoFinalizada)
                });
            ordenesCreadas.Add(orden8);

            // Orden 9: 2 mecánicos (mecanico1 y mecanico2), 2 servicios, Estado Pagada (Tarjeta)
            var orden9 = await CrearOrdenConDetallesAsync(vehiculos[4], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCompletada, vehiculos[4].KmActual + 110,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoFinalizada),
                    (servicioAlineacion, mecanico2, DetalleServicio.EstadoFinalizada)
                });
            var factura9 = await billingService.GenerarFactura(orden9.Id);
            await billingService.RegistrarPago(factura9.Id, metodoTarjeta.Id, factura9.Total);
            ordenesCreadas.Add(orden9);

            // Orden 10: 2 mecánicos (mecanico2 y mecanico3), 2 servicios, Estado Cancelada
            var orden10 = await CrearOrdenConDetallesAsync(vehiculos[4], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCancelada, vehiculos[4].KmActual + 300,
                new (Servicio, Usuario, string)[]
                {
                    (servicioFrenos, mecanico2, DetalleServicio.EstadoPendiente),
                    (servicioDiagnostico, mecanico3, DetalleServicio.EstadoPendiente)
                });
            ordenesCreadas.Add(orden10);

            // --- GRUPO 3: Órdenes 11 a 15 (Asignadas a 3 Mecánicos colaborando en la misma orden) ---

            // Orden 11: 3 mecánicos (mecanico1, 2 y 3), 3 servicios, Estado Abierta
            var orden11 = await CrearOrdenConDetallesAsync(vehiculos[5], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoAbierta, vehiculos[5].KmActual + 150,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoPendiente),
                    (servicioAlineacion, mecanico2, DetalleServicio.EstadoPendiente),
                    (servicioFrenos, mecanico3, DetalleServicio.EstadoPendiente)
                });
            ordenesCreadas.Add(orden11);

            // Orden 12: 3 mecánicos (mecanico1, 2 y 3), 3 servicios, Estado En Proceso
            var orden12 = await CrearOrdenConDetallesAsync(vehiculos[5], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoEnProceso, vehiculos[5].KmActual + 280,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoEnCurso),
                    (servicioFrenos, mecanico2, DetalleServicio.EstadoEnCurso),
                    (servicioDiagnostico, mecanico3, DetalleServicio.EstadoPendiente)
                });
            ordenesCreadas.Add(orden12);

            // Orden 13: 3 mecánicos (mecanico1, 2 y 3), 3 servicios, Estado Completada
            var orden13 = await CrearOrdenConDetallesAsync(vehiculos[6], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCompletada, vehiculos[6].KmActual + 100,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoFinalizada),
                    (servicioAlineacion, mecanico2, DetalleServicio.EstadoFinalizada),
                    (servicioFrenos, mecanico3, DetalleServicio.EstadoFinalizada)
                });
            ordenesCreadas.Add(orden13);

            // Orden 14: 3 mecánicos (mecanico1, 2 y 3), 4 servicios, Estado Pagada (Efectivo con vuelto)
            var orden14 = await CrearOrdenConDetallesAsync(vehiculos[6], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCompletada, vehiculos[6].KmActual + 350,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoFinalizada),
                    (servicioAlineacion, mecanico2, DetalleServicio.EstadoFinalizada),
                    (servicioFrenos, mecanico3, DetalleServicio.EstadoFinalizada),
                    (servicioDiagnostico, mecanico1, DetalleServicio.EstadoFinalizada)
                });
            var factura14 = await billingService.GenerarFactura(orden14.Id);
            var resPago14 = await billingService.RegistrarPago(factura14.Id, metodoEfectivo.Id, factura14.Total + 10000m);
            Assert.True(resPago14.vuelto == 10000m, "El vuelto calculado para el pago en efectivo debe ser exactamente 10000.");
            ordenesCreadas.Add(orden14);

            // Orden 15: 3 mecánicos (mecanico1, 2 y 3), 3 servicios, Estado Pagada (Tarjeta)
            var orden15 = await CrearOrdenConDetallesAsync(vehiculos[7], taller.Id, recepcionista.Id,
                RegistroServicio.EstadoCompletada, vehiculos[7].KmActual + 120,
                new (Servicio, Usuario, string)[]
                {
                    (servicioAceite, mecanico1, DetalleServicio.EstadoFinalizada),
                    (servicioFrenos, mecanico2, DetalleServicio.EstadoFinalizada),
                    (servicioDiagnostico, mecanico3, DetalleServicio.EstadoFinalizada)
                });
            var factura15 = await billingService.GenerarFactura(orden15.Id);
            await billingService.RegistrarPago(factura15.Id, metodoTarjeta.Id, factura15.Total);
            ordenesCreadas.Add(orden15);

            // -----------------------------------------------------------------
            // 4. ASERCIONES Y VALIDACIONES DEL FLUJO COMPLETO
            // -----------------------------------------------------------------
            Assert.Equal(15, ordenesCreadas.Count);

            // Refrescar órdenes desde base de datos con sus detalles
            var ids = ordenesCreadas.Select(o => o.Id).ToList();
            var ordenesDb = await _ctx.RegistroServicios
                .AsNoTracking()
                .Include(o => o.Detalles)
                .Where(o => ids.Contains(o.Id))
                .ToListAsync();

            Assert.Equal(15, ordenesDb.Count);

            // Traer facturas con sus pagos
            var facturasDb = await _ctx.Facturas
                .AsNoTracking()
                .Include(f => f.Pagos)
                .Where(f => ids.Contains(f.RegistroServicioId))
                .ToListAsync();

            // 4.1 Validar que ningún detalle de ninguna orden quedó sin mecánico asignado
            foreach (var orden in ordenesDb)
            {
                Assert.NotEmpty(orden.Detalles);
                Assert.All(orden.Detalles, d => Assert.True(d.UsuarioId > 0, $"Detalle {d.Id} de orden {orden.Id} no tiene mecánico asignado."));
            }

            // 4.2 Validar Órdenes 1 a 5 (exactamente 1 mecánico por orden)
            for (int i = 0; i < 5; i++)
            {
                var orden = ordenesDb.First(o => o.Id == ordenesCreadas[i].Id);
                var mecanicosDistintos = orden.Detalles.Select(d => d.UsuarioId).Distinct().Count();
                Assert.Equal(1, mecanicosDistintos);
            }

            // 4.3 Validar Órdenes 6 a 10 (exactamente 2 mecánicos distintos por orden)
            for (int i = 5; i < 10; i++)
            {
                var orden = ordenesDb.First(o => o.Id == ordenesCreadas[i].Id);
                var mecanicosDistintos = orden.Detalles.Select(d => d.UsuarioId).Distinct().Count();
                Assert.Equal(2, mecanicosDistintos);
            }

            // 4.4 Validar Órdenes 11 a 15 (exactamente 3 mecánicos distintos por orden)
            for (int i = 10; i < 15; i++)
            {
                var orden = ordenesDb.First(o => o.Id == ordenesCreadas[i].Id);
                var mecanicosDistintos = orden.Detalles.Select(d => d.UsuarioId).Distinct().Count();
                Assert.Equal(3, mecanicosDistintos);
            }

            // 4.5 Validar Estados
            Assert.Contains(ordenesDb, o => o.Estado == RegistroServicio.EstadoAbierta);
            Assert.Contains(ordenesDb, o => o.Estado == RegistroServicio.EstadoEnProceso);
            Assert.Contains(ordenesDb, o => o.Estado == RegistroServicio.EstadoCompletada);
            Assert.Contains(ordenesDb, o => o.Estado == "Pagada");
            Assert.Contains(ordenesDb, o => o.Estado == RegistroServicio.EstadoCancelada);

            // 4.6 Validar Facturación y Pagos en las órdenes pagadas (4, 9, 14, 15)
            var ordenesPagadas = ordenesDb.Where(o => o.Estado == "Pagada").ToList();
            Assert.Equal(4, ordenesPagadas.Count);

            foreach (var ordenPagada in ordenesPagadas)
            {
                var factura = facturasDb.FirstOrDefault(f => f.RegistroServicioId == ordenPagada.Id);
                Assert.NotNull(factura);

                var totalEsperado = ordenPagada.Detalles.Sum(d => d.Precio * d.Cantidad);
                Assert.Equal(totalEsperado, factura.Total);

                Assert.NotEmpty(factura.Pagos);
                var totalPagado = factura.Pagos.Sum(p => p.Monto);
                Assert.Equal(factura.Total, totalPagado);
            }
        }

        private async Task<RegistroServicio> CrearOrdenConDetallesAsync(
            Vehiculo vehiculo,
            int tallerId,
            int recepcionistaId,
            string estadoOrden,
            int kmIngreso,
            (Servicio servicio, Usuario mecanico, string estadoDetalle)[] items)
        {
            var orden = new RegistroServicio
            {
                VehiculoId = vehiculo.Id,
                TallerId = tallerId,
                UsuarioId = recepcionistaId,
                Fecha = DateTime.Now,
                KmIngreso = kmIngreso,
                Estado = estadoOrden,
                Activo = true
            };
            await _ctx.RegistroServicios.AddAsync(orden);
            await _ctx.SaveChangesAsync();
            _registroIdsCreados.Add(orden.Id);

            int ordenEjecucion = 1;
            foreach (var item in items)
            {
                var detalle = new DetalleServicio
                {
                    RegistroServicioId = orden.Id,
                    ServicioId = item.servicio.Id,
                    UsuarioId = item.mecanico.Id,
                    Cantidad = 1,
                    Precio = item.servicio.Precio,
                    Estado = item.estadoDetalle,
                    OrdenEjecucion = ordenEjecucion++,
                    Origen = "Manual",
                    Observaciones = $"Servicio realizado por {item.mecanico.Nombre}"
                };
                await _ctx.DetalleServicios.AddAsync(detalle);
            }
            await _ctx.SaveChangesAsync();

            return orden;
        }

        private async Task LimpiarDatosSimulacionAsync()
        {
            if (_ctx == null) return;

            try
            {
                // Limpieza FK-safe: Pago -> Factura -> DetalleServicio -> RegistroServicio -> PropietarioVehiculo -> Vehiculo -> Cliente
                var registroIds = await _ctx.RegistroServicios
                    .Where(r => r.Vehiculo.Patente.StartsWith(PrefijoSimulacion) || _registroIdsCreados.Contains(r.Id))
                    .Select(r => r.Id)
                    .ToListAsync();

                if (registroIds.Any())
                {
                    var facturaIds = await _ctx.Facturas
                        .Where(f => registroIds.Contains(f.RegistroServicioId))
                        .Select(f => f.Id)
                        .ToListAsync();

                    var pagos = await _ctx.Pagos.Where(p => facturaIds.Contains(p.FacturaId)).ToListAsync();
                    if (pagos.Any())
                    {
                        _ctx.Pagos.RemoveRange(pagos);
                        await _ctx.SaveChangesAsync();
                    }

                    var facturas = await _ctx.Facturas.Where(f => facturaIds.Contains(f.Id)).ToListAsync();
                    if (facturas.Any())
                    {
                        _ctx.Facturas.RemoveRange(facturas);
                        await _ctx.SaveChangesAsync();
                    }

                    var detalles = await _ctx.DetalleServicios.Where(d => registroIds.Contains(d.RegistroServicioId)).ToListAsync();
                    if (detalles.Any())
                    {
                        _ctx.DetalleServicios.RemoveRange(detalles);
                        await _ctx.SaveChangesAsync();
                    }

                    var registros = await _ctx.RegistroServicios.Where(r => registroIds.Contains(r.Id)).ToListAsync();
                    if (registros.Any())
                    {
                        _ctx.RegistroServicios.RemoveRange(registros);
                        await _ctx.SaveChangesAsync();
                    }
                }

                var vehiculos = await _ctx.Vehiculos
                    .Where(v => v.Patente.StartsWith(PrefijoSimulacion) || _patentesCreadas.Contains(v.Patente))
                    .ToListAsync();

                if (vehiculos.Any())
                {
                    var vehiculoIds = vehiculos.Select(v => v.Id).ToList();
                    var propietarios = await _ctx.PropietariosVehiculos.Where(p => vehiculoIds.Contains(p.VehiculoId)).ToListAsync();
                    if (propietarios.Any())
                    {
                        _ctx.PropietariosVehiculos.RemoveRange(propietarios);
                        await _ctx.SaveChangesAsync();
                    }

                    _ctx.Vehiculos.RemoveRange(vehiculos);
                    await _ctx.SaveChangesAsync();
                }

                var clientes = await _ctx.Clientes
                    .Where(c => _dnisClientesCreados.Contains(c.Dni) || (c.Email != null && c.Email.EndsWith(".sim@correo.com")))
                    .ToListAsync();

                if (clientes.Any())
                {
                    _ctx.Clientes.RemoveRange(clientes);
                    await _ctx.SaveChangesAsync();
                }
            }
            catch
            {
                // Silenciar errores durante limpieza defensiva
            }
        }
    }
}
