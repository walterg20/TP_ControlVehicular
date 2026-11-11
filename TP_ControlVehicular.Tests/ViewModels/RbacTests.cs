// walterg20
using AutoMapper;
using Moq;
using System.Threading;
using System.Windows;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    public class RbacTests
    {
        [Fact]
        public void MainWindow_RecepcionistaLogin_OcultaMenusAdministrativos()
        {
            // Como MainWindow es un elemento de UI de WPF, necesitamos un hilo STA
            Thread thread = new Thread(() =>
            {
                // Simulamos login
                UserSession.CurrentUser = new UsuarioDto { IdRol = (int)RolesSistema.Recepcionista, Nombre = "Recepcionista Test" };

                var window = new MainWindow();

                // AplicarRestriccionesPorRol lee la fuente real de la sesion (UsuarioSesionActual).
                typeof(MainWindow).GetProperty("UsuarioSesionActual")!.SetValue(window, UserSession.CurrentUser);
                
                // Ejecutamos el m�todo que aplica los permisos (que idealmente se lanza en el OnLoginExitoso)
                // Usamos reflection porque el m�todo es privado
                var method = typeof(MainWindow).GetMethod("AplicarRestriccionesPorRol", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(window, null);

                // Verificamos que el bot�n de Usuarios est� oculto
                var btnUsuario = typeof(MainWindow).GetField("btnUsuario", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(window) as System.Windows.Controls.Button;
                
                Assert.NotNull(btnUsuario);
                Assert.Equal(Visibility.Collapsed, btnUsuario.Visibility);
            });
            
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        [Fact]
        public void MainWindow_MecanicoLogin_OcultaMenuClientesYVehiculos()
        {
            Thread thread = new Thread(() =>
            {
                UserSession.CurrentUser = new UsuarioDto { IdRol = (int)RolesSistema.Mecanico, Nombre = "Mecanico Test" };

                var window = new MainWindow();

                // AplicarRestriccionesPorRol lee la fuente real de la sesion (UsuarioSesionActual).
                typeof(MainWindow).GetProperty("UsuarioSesionActual")!.SetValue(window, UserSession.CurrentUser);
                
                var method = typeof(MainWindow).GetMethod("AplicarRestriccionesPorRol", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Invoke(window, null);

                var btnCliente = typeof(MainWindow).GetField("btnCliente", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(window) as System.Windows.Controls.Button;
                var btnVehiculo = typeof(MainWindow).GetField("btnVehiculo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(window) as System.Windows.Controls.Button;
                
                Assert.NotNull(btnCliente);
                Assert.Equal(Visibility.Collapsed, btnCliente.Visibility);

                Assert.NotNull(btnVehiculo);
                Assert.Equal(Visibility.Collapsed, btnVehiculo.Visibility);
            });
            
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        [Fact]
        public void ServicioViewModel_Mecanico_NoPuedeModificarServicios()
        {
            // Simulamos sesion de Mecanico con sus permisos reales de bd/02_datos.sql
            UserSession.CurrentUser = new UsuarioDto
            {
                IdRol = (int)RolesSistema.Mecanico,
                Nombre = "Mecánico Pedro",
                Permisos = new System.Collections.Generic.List<string>
                {
                    "Cliente.Ver",
                    "Vehiculo.Ver",
                    "Marca.Ver",
                    "Modelo.Ver",
                    "OrdenServicio.Ver",
                    "OrdenServicio.Editar",
                    "Servicio.Ver",
                    "MisTrabajos.Ver",
                    "Reporte.Mecanico.Ver"
                }
            };

            var repo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IServicioRepository>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<TP_ControlVehicular.Negocio.Mappers.MappingProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance).CreateMapper();
            var vm = new ServicioViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarServiciosHandler(repo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarServicioHandler(repo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarServicioHandler(repo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarServicioHandler(repo.Object),
                new TP_ControlVehicular.Negocio.Services.ReactivarServicioHandler(repo.Object));

            vm.NotificarPermisos();

            // El mecanico NO debe poder crear, editar ni eliminar servicios
            Assert.False(vm.PuedeCrear);
            Assert.False(vm.PuedeEditar);
            Assert.False(vm.PuedeEliminar);
        }

        [Fact]
        public void ServicioViewModel_Administrador_PuedeModificarServicios()
        {
            UserSession.CurrentUser = new UsuarioDto
            {
                IdRol = (int)RolesSistema.Administrador,
                Nombre = "Admin Walter",
                Permisos = new System.Collections.Generic.List<string>
                {
                    "Servicio.Ver",
                    "Servicio.Crear",
                    "Servicio.Editar",
                    "Servicio.Eliminar"
                }
            };

            var repo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IServicioRepository>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<TP_ControlVehicular.Negocio.Mappers.MappingProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance).CreateMapper();
            var vm = new ServicioViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarServiciosHandler(repo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarServicioHandler(repo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarServicioHandler(repo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarServicioHandler(repo.Object),
                new TP_ControlVehicular.Negocio.Services.ReactivarServicioHandler(repo.Object));

            vm.NotificarPermisos();

            Assert.True(vm.PuedeCrear);
            Assert.True(vm.PuedeEditar);
            Assert.True(vm.PuedeEliminar);
        }

        [Fact]
        public void Mecanico_MenuAdministracion_SoloPuedeListar_EnTodosLosModulos()
        {
            // Permisos exactos asignados al rol Mecanico en bd/02_datos.sql
            UserSession.CurrentUser = new UsuarioDto
            {
                IdRol = (int)RolesSistema.Mecanico,
                Nombre = "Mecánico Pedro",
                Permisos = new System.Collections.Generic.List<string>
                {
                    "Cliente.Ver",
                    "Vehiculo.Ver",
                    "Marca.Ver",
                    "Modelo.Ver",
                    "OrdenServicio.Ver",
                    "OrdenServicio.Editar",
                    "Servicio.Ver",
                    "MisTrabajos.Ver",
                    "Reporte.Mecanico.Ver"
                }
            };

            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<TP_ControlVehicular.Negocio.Mappers.MappingProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance).CreateMapper();

            // 1. ClienteViewModel
            var clienteRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IClienteRepository>();
            var vmCliente = new ClienteViewModel(
                new TP_ControlVehicular.Negocio.Services.RegistrarClienteHandler(clienteRepo.Object, mapper),
                clienteRepo.Object,
                mapper);
            vmCliente.NotificarPermisos();
            Assert.False(vmCliente.PuedeCrear);
            Assert.False(vmCliente.PuedeEditar);
            Assert.False(vmCliente.PuedeEliminar);

            // 2. VehiculoViewModel
            var vehiculoRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IVehiculoRepository>();
            var modeloRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IModeloRepository>();
            var marcaRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IMarcaRepository>();
            var vmVehiculo = new VehiculoViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarVehiculosHandler(vehiculoRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarVehiculoHandler(vehiculoRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarVehiculoHandler(vehiculoRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarVehiculoHandler(vehiculoRepo.Object),
                clienteRepo.Object,
                new TP_ControlVehicular.Negocio.Services.ListarModelosHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ListarMarcaHandler(marcaRepo.Object, mapper),
                mapper);
            vmVehiculo.NotificarPermisos();
            Assert.False(vmVehiculo.PuedeCrear);
            Assert.False(vmVehiculo.PuedeEditar);
            Assert.False(vmVehiculo.PuedeEliminar);

            // 3. ModeloViewModel
            var vmModelo = new ModeloViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarModelosHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarModeloHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarModeloHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarModeloHandler(modeloRepo.Object),
                new TP_ControlVehicular.Negocio.Services.ListarMarcaHandler(marcaRepo.Object, mapper));
            vmModelo.NotificarPermisos();
            Assert.False(vmModelo.PuedeCrear);
            Assert.False(vmModelo.PuedeEditar);
            Assert.False(vmModelo.PuedeEliminar);

            // 4. MarcaViewModel
            var vmMarca = new MarcaViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarMarcaHandler(marcaRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarMarcaHandler(marcaRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarMarcaHandler(marcaRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarMarcaHandler(marcaRepo.Object));
            vmMarca.NotificarPermisos();
            Assert.False(vmMarca.PuedeCrear);
            Assert.False(vmMarca.PuedeEditar);
            Assert.False(vmMarca.PuedeEliminar);

            // 5. ServicioViewModel
            var servicioRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IServicioRepository>();
            var vmServicio = new ServicioViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarServiciosHandler(servicioRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarServicioHandler(servicioRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarServicioHandler(servicioRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarServicioHandler(servicioRepo.Object),
                new TP_ControlVehicular.Negocio.Services.ReactivarServicioHandler(servicioRepo.Object));
            vmServicio.NotificarPermisos();
            Assert.False(vmServicio.PuedeCrear);
            Assert.False(vmServicio.PuedeEditar);
            Assert.False(vmServicio.PuedeEliminar);
        }

        [Fact]
        public void Administrador_MenuAdministracion_PuedeModificar_EnTodosLosModulos()
        {
            UserSession.CurrentUser = new UsuarioDto
            {
                IdRol = (int)RolesSistema.Administrador,
                Nombre = "Admin Walter",
                Permisos = new System.Collections.Generic.List<string>
                {
                    "Cliente.Ver", "Cliente.Crear", "Cliente.Editar", "Cliente.Eliminar",
                    "Vehiculo.Ver", "Vehiculo.Crear", "Vehiculo.Editar", "Vehiculo.Eliminar",
                    "Modelo.Ver", "Modelo.Crear", "Modelo.Editar", "Modelo.Eliminar",
                    "Marca.Ver", "Marca.Crear", "Marca.Editar", "Marca.Eliminar",
                    "Servicio.Ver", "Servicio.Crear", "Servicio.Editar", "Servicio.Eliminar"
                }
            };

            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<TP_ControlVehicular.Negocio.Mappers.MappingProfile>(), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance).CreateMapper();

            var clienteRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IClienteRepository>();
            var vmCliente = new ClienteViewModel(
                new TP_ControlVehicular.Negocio.Services.RegistrarClienteHandler(clienteRepo.Object, mapper),
                clienteRepo.Object,
                mapper);
            vmCliente.NotificarPermisos();
            Assert.True(vmCliente.PuedeCrear);
            Assert.True(vmCliente.PuedeEditar);
            Assert.True(vmCliente.PuedeEliminar);

            var vehiculoRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IVehiculoRepository>();
            var modeloRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IModeloRepository>();
            var marcaRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IMarcaRepository>();
            var vmVehiculo = new VehiculoViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarVehiculosHandler(vehiculoRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarVehiculoHandler(vehiculoRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarVehiculoHandler(vehiculoRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarVehiculoHandler(vehiculoRepo.Object),
                clienteRepo.Object,
                new TP_ControlVehicular.Negocio.Services.ListarModelosHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ListarMarcaHandler(marcaRepo.Object, mapper),
                mapper);
            vmVehiculo.NotificarPermisos();
            Assert.True(vmVehiculo.PuedeCrear);
            Assert.True(vmVehiculo.PuedeEditar);
            Assert.True(vmVehiculo.PuedeEliminar);

            var vmModelo = new ModeloViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarModelosHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarModeloHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarModeloHandler(modeloRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarModeloHandler(modeloRepo.Object),
                new TP_ControlVehicular.Negocio.Services.ListarMarcaHandler(marcaRepo.Object, mapper));
            vmModelo.NotificarPermisos();
            Assert.True(vmModelo.PuedeCrear);
            Assert.True(vmModelo.PuedeEditar);
            Assert.True(vmModelo.PuedeEliminar);

            var vmMarca = new MarcaViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarMarcaHandler(marcaRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarMarcaHandler(marcaRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarMarcaHandler(marcaRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarMarcaHandler(marcaRepo.Object));
            vmMarca.NotificarPermisos();
            Assert.True(vmMarca.PuedeCrear);
            Assert.True(vmMarca.PuedeEditar);
            Assert.True(vmMarca.PuedeEliminar);

            var servicioRepo = new Mock<TP_ControlVehicular.Negocio.Interfaces.IServicioRepository>();
            var vmServicio = new ServicioViewModel(
                new TP_ControlVehicular.Negocio.Services.ListarServiciosHandler(servicioRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.RegistrarServicioHandler(servicioRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.ModificarServicioHandler(servicioRepo.Object, mapper),
                new TP_ControlVehicular.Negocio.Services.EliminarServicioHandler(servicioRepo.Object),
                new TP_ControlVehicular.Negocio.Services.ReactivarServicioHandler(servicioRepo.Object));
            vmServicio.NotificarPermisos();
            Assert.True(vmServicio.PuedeCrear);
            Assert.True(vmServicio.PuedeEditar);
            Assert.True(vmServicio.PuedeEliminar);
        }
    }
}
