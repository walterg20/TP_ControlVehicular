using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Mappers;
using TP_ControlVehicular.Negocio.Reportes;
using TP_ControlVehicular.Negocio.Reportes.Documentos;
using TP_ControlVehicular.Negocio.Services;
using Xunit;

namespace TP_ControlVehicular.Tests.Unit
{
    /// <summary>
    /// Pruebas unitarias (L1) de handlers y servicios de negocio.
    /// Usan repositorios mockeados con Moq y un AutoMapper real construido con <see cref="MappingProfile"/>
    /// para validar el mapeo Entidad -> DTO sin depender de una base de datos.
    /// </summary>
    public class HandlersUnitTests
    {
        /// <summary>Construye un IMapper real a partir del perfil de la aplicación.</summary>
        private static IMapper CrearMapper()
        {
            var configuracion = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
            return configuracion.CreateMapper();
        }

        private static Usuario CrearUsuario(string dni, bool estado, string contrasena)
        {
            return new Usuario
            {
                Id = 7,
                RolId = 2,
                Nombre = "Ana",
                Apellido = "Gómez",
                Dni = dni,
                Email = "ana@taller.com",
                Telefono = "3624000000",
                Domicilio = "Calle 1",
                FechaNacimiento = new DateTime(1990, 5, 20),
                Contrasena = contrasena,
                Activo = estado,
                Rol = new Rol
                {
                    Id = 2,
                    Nombre = "Recepcionista",
                    Permisos = new List<Permiso>
                    {
                        new Permiso { IdPermiso = 1, Nombre = "Ordenes.Crear" },
                        new Permiso { IdPermiso = 2, Nombre = "Clientes.Ver" }
                    }
                }
            };
        }

        // ---------------------------------------------------------------------
        // AutenticarUsuarioHandler
        // ---------------------------------------------------------------------

        [Fact]
        public async Task AutenticarUsuario_CredencialesValidas_DeberiaRetornarExitosoConUsuarioMapeado()
        {
            // Arrange
            var usuario = CrearUsuario("30111222", estado: true, contrasena: "clave-segura");
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.GetByDniAsync("30111222")).ReturnsAsync(usuario);
            var handler = new AutenticarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var respuesta = await handler.HandleAsync("30111222", "clave-segura");

            // Assert
            Assert.Equal(ResultadoAutenticacion.Exitoso, respuesta.Resultado);
            Assert.Equal("Autenticación exitosa.", respuesta.Mensaje);
            Assert.NotNull(respuesta.Usuario);
            Assert.Equal(7, respuesta.Usuario!.IdUsuario);
            Assert.Equal(2, respuesta.Usuario.IdRol);
            Assert.Equal("Recepcionista", respuesta.Usuario.RolNombre);
            Assert.Equal("Ana Gómez [30111222]", respuesta.Usuario.NombreCompletoYDni);
            Assert.Equal(new[] { "Ordenes.Crear", "Clientes.Ver" }, respuesta.Usuario.Permisos);
        }

        [Fact]
        public async Task AutenticarUsuario_DniNoRegistrado_DeberiaRetornarUsuarioNoEncontrado()
        {
            // Arrange
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.GetByDniAsync(It.IsAny<string>())).ReturnsAsync((Usuario?)null);
            var handler = new AutenticarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var respuesta = await handler.HandleAsync("99999999", "cualquiera");

            // Assert
            Assert.Equal(ResultadoAutenticacion.UsuarioNoEncontrado, respuesta.Resultado);
            Assert.Equal("El DNI ingresado no se encuentra registrado.", respuesta.Mensaje);
            Assert.Null(respuesta.Usuario);
        }

        [Fact]
        public async Task AutenticarUsuario_UsuarioInactivo_DeberiaRetornarUsuarioInactivo()
        {
            // Arrange
            var usuario = CrearUsuario("30000000", estado: false, contrasena: "clave-segura");
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.GetByDniAsync("30000000")).ReturnsAsync(usuario);
            var handler = new AutenticarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var respuesta = await handler.HandleAsync("30000000", "clave-segura");

            // Assert
            Assert.Equal(ResultadoAutenticacion.UsuarioInactivo, respuesta.Resultado);
            Assert.Equal("El usuario se encuentra inactivo.", respuesta.Mensaje);
            Assert.Null(respuesta.Usuario);
        }

        [Fact]
        public async Task AutenticarUsuario_ContrasenaIncorrecta_DeberiaRetornarContrasenaIncorrecta()
        {
            // Arrange
            var usuario = CrearUsuario("30111222", estado: true, contrasena: "clave-segura");
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.GetByDniAsync("30111222")).ReturnsAsync(usuario);
            var handler = new AutenticarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var respuesta = await handler.HandleAsync("30111222", "clave-equivocada");

            // Assert
            Assert.Equal(ResultadoAutenticacion.ContrasenaIncorrecta, respuesta.Resultado);
            Assert.Equal("Contraseña incorrecta.", respuesta.Mensaje);
            Assert.Null(respuesta.Usuario);
        }

        [Fact]
        public async Task AutenticarUsuario_ErrorBdConExcepcionInterna_DeberiaRetornarMensajeInterno()
        {
            // Arrange
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.GetByDniAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error de red", new Exception("Servidor SQL no disponible")));
            var handler = new AutenticarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var respuesta = await handler.HandleAsync("30111222", "clave-segura");

            // Assert
            Assert.Equal(ResultadoAutenticacion.ErrorBaseDatos, respuesta.Resultado);
            Assert.Equal("Error BD: Servidor SQL no disponible", respuesta.Mensaje);
            Assert.Null(respuesta.Usuario);
        }

        [Fact]
        public async Task AutenticarUsuario_ErrorBdSinExcepcionInterna_DeberiaRetornarMensajeDirecto()
        {
            // Arrange
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.GetByDniAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Timeout de conexión"));
            var handler = new AutenticarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var respuesta = await handler.HandleAsync("30111222", "clave-segura");

            // Assert
            Assert.Equal(ResultadoAutenticacion.ErrorBaseDatos, respuesta.Resultado);
            Assert.Equal("Error BD: Timeout de conexión", respuesta.Mensaje);
        }

        // ---------------------------------------------------------------------
        // EmpresaConfiguracionService.ObtenerAsync
        // ---------------------------------------------------------------------

        [Fact]
        public async Task EmpresaConfiguracion_SinRegistro_DeberiaCrearConfiguracionPorDefectoConBranding()
        {
            // Arrange
            ConfiguracionEmpresa? capturada = null;
            var repo = new Mock<IConfiguracionEmpresaRepository>();
            repo.Setup(r => r.ObtenerAsync()).ReturnsAsync((ConfiguracionEmpresa?)null);
            repo.Setup(r => r.AddAsync(It.IsAny<ConfiguracionEmpresa>()))
                .Callback<ConfiguracionEmpresa>(e => capturada = e)
                .Returns(Task.CompletedTask);
            var servicio = new EmpresaConfiguracionService(repo.Object, CrearMapper());

            // Act
            var dto = await servicio.ObtenerAsync();

            // Assert
            Assert.NotNull(capturada);
            Assert.Equal(1, capturada!.Id);
            Assert.Null(capturada.Logo);
            Assert.Equal(EmpresaBranding.Nombre, dto.Nombre);
            Assert.Equal(EmpresaBranding.Direccion, dto.Direccion);
            Assert.Equal(EmpresaBranding.Telefono, dto.Telefono);
            Assert.Null(dto.Email);
            Assert.False(dto.TieneLogo);
            repo.Verify(r => r.AddAsync(It.IsAny<ConfiguracionEmpresa>()), Times.Once);
        }

        [Fact]
        public async Task EmpresaConfiguracion_ConRegistroYLogo_DeberiaMapearTieneLogoVerdadero()
        {
            // Arrange
            var entidad = new ConfiguracionEmpresa
            {
                Id = 1,
                Nombre = "Taller Norte",
                Direccion = "Av. Siempre Viva 742",
                Telefono = "3624111111",
                Email = "contacto@tallernorte.com",
                Logo = new byte[] { 1, 2, 3, 4, 5 }
            };
            var repo = new Mock<IConfiguracionEmpresaRepository>();
            repo.Setup(r => r.ObtenerAsync()).ReturnsAsync(entidad);
            var servicio = new EmpresaConfiguracionService(repo.Object, CrearMapper());

            // Act
            var dto = await servicio.ObtenerAsync();

            // Assert
            Assert.Equal("Taller Norte", dto.Nombre);
            Assert.Equal("Av. Siempre Viva 742", dto.Direccion);
            Assert.Equal("3624111111", dto.Telefono);
            Assert.Equal("contacto@tallernorte.com", dto.Email);
            Assert.True(dto.TieneLogo);
            repo.Verify(r => r.AddAsync(It.IsAny<ConfiguracionEmpresa>()), Times.Never);
        }

        [Fact]
        public async Task EmpresaConfiguracion_ConRegistroSinLogo_DeberiaMapearTieneLogoFalso()
        {
            // Arrange
            var entidad = new ConfiguracionEmpresa
            {
                Id = 1,
                Nombre = "Taller Sur",
                Direccion = "Ruta 16 Km 10",
                Telefono = "3624222222",
                Logo = null
            };
            var repo = new Mock<IConfiguracionEmpresaRepository>();
            repo.Setup(r => r.ObtenerAsync()).ReturnsAsync(entidad);
            var servicio = new EmpresaConfiguracionService(repo.Object, CrearMapper());

            // Act
            var dto = await servicio.ObtenerAsync();

            // Assert
            Assert.False(dto.TieneLogo);
            repo.Verify(r => r.AddAsync(It.IsAny<ConfiguracionEmpresa>()), Times.Never);
        }

        // ---------------------------------------------------------------------
        // RegistrarUsuarioHandler / ModificarUsuarioHandler
        // ---------------------------------------------------------------------

        [Fact]
        public async Task RegistrarUsuario_DeberiaAgregarYRetornarDtoMapeadoDelRecargado()
        {
            // Arrange
            var nuevo = new Usuario
            {
                RolId = 3,
                Nombre = "Demo",
                Apellido = "Uno",
                Dni = "20000000",
                Contrasena = "clave",
                Activo = true
            };
            var recargado = new Usuario
            {
                Id = 42,
                RolId = 3,
                Nombre = "Demo",
                Apellido = "Uno",
                Dni = "20000000",
                Contrasena = "clave",
                Activo = true,
                Rol = new Rol { Id = 3, Nombre = "Mecanico", Permisos = new List<Permiso>() }
            };
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.AddAsync(nuevo)).Returns(Task.CompletedTask);
            repo.Setup(r => r.GetByNombreAsync("Demo")).ReturnsAsync(recargado);
            var handler = new RegistrarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var dto = await handler.HandleAsync(nuevo);

            // Assert
            Assert.Equal(42, dto.IdUsuario);
            Assert.Equal(3, dto.IdRol);
            Assert.Equal("Mecanico", dto.RolNombre);
            Assert.Equal("Demo Uno [20000000]", dto.NombreCompletoYDni);
            repo.Verify(r => r.AddAsync(nuevo), Times.Once);
            repo.Verify(r => r.GetByNombreAsync("Demo"), Times.Once);
        }

        [Fact]
        public async Task RegistrarUsuario_CuandoNoSePuedeRecargar_DeberiaMapearEntidadOriginal()
        {
            // Arrange
            var nuevo = new Usuario
            {
                Id = 0,
                RolId = 3,
                Nombre = "Demo",
                Apellido = "Dos",
                Dni = "20000001",
                Contrasena = "clave",
                Activo = true
            };
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.AddAsync(nuevo)).Returns(Task.CompletedTask);
            repo.Setup(r => r.GetByNombreAsync("Demo")).ReturnsAsync((Usuario?)null);
            var handler = new RegistrarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var dto = await handler.HandleAsync(nuevo);

            // Assert
            Assert.Equal(0, dto.IdUsuario);
            Assert.Equal(3, dto.IdRol);
            Assert.Equal(string.Empty, dto.RolNombre);
            Assert.Empty(dto.Permisos);
        }

        [Fact]
        public async Task ModificarUsuario_DeberiaActualizarYRetornarDtoMapeadoDelRecargado()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 5,
                RolId = 2,
                Nombre = "Mod",
                Apellido = "Tres",
                Dni = "30000001",
                Contrasena = "clave",
                Activo = false
            };
            var recargado = new Usuario
            {
                Id = 5,
                RolId = 2,
                Nombre = "Mod",
                Apellido = "Tres",
                Dni = "30000001",
                Contrasena = "clave",
                Activo = false,
                Rol = new Rol { Id = 2, Nombre = "Recepcionista", Permisos = new List<Permiso>() }
            };
            var repo = new Mock<IUsuarioRepository>();
            repo.Setup(r => r.UpdateAsync(usuario)).Returns(Task.CompletedTask);
            repo.Setup(r => r.GetByNombreAsync("Mod")).ReturnsAsync(recargado);
            var handler = new ModificarUsuarioHandler(repo.Object, CrearMapper());

            // Act
            var dto = await handler.HandleAsync(usuario);

            // Assert
            Assert.Equal(5, dto.IdUsuario);
            Assert.Equal("Recepcionista", dto.RolNombre);
            Assert.False(dto.Activo);
            Assert.Equal("Mod Tres [30000001]", dto.NombreCompletoYDni);
            repo.Verify(r => r.UpdateAsync(usuario), Times.Once);
            repo.Verify(r => r.GetByNombreAsync("Mod"), Times.Once);
        }

        // ---------------------------------------------------------------------
        // ListarModelosHandler & Modelo Handlers
        // ---------------------------------------------------------------------

        [Fact]
        public async Task ListarModelosHandler_RetornaModelosConMarcaNombrePoblado()
        {
            // Arrange
            var modelos = new List<Modelo>
            {
                new Modelo
                {
                    Id = 1,
                    NombreModelo = "Corolla",
                    MarcaId = 10,
                    Marca = new Marca { Id = 10, NombreMarca = "Toyota" }
                },
                new Modelo
                {
                    Id = 2,
                    NombreModelo = "Hilux",
                    MarcaId = 10,
                    Marca = new Marca { Id = 10, NombreMarca = "Toyota" }
                }
            };

            var repo = new Mock<IModeloRepository>();
            repo.Setup(r => r.GetAllAsync()).ReturnsAsync(modelos);
            var handler = new ListarModelosHandler(repo.Object, CrearMapper());

            // Act
            var resultado = await handler.HandleAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Toyota", resultado[0].MarcaNombre);
            Assert.Equal("Corolla", resultado[0].NombreModelo);
            Assert.Equal("Toyota", resultado[1].MarcaNombre);
            Assert.Equal("Hilux", resultado[1].NombreModelo);
            repo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task RegistrarModeloHandler_GuardaYRetornaDtoConMarcaNombre()
        {
            // Arrange
            var input = new Modelo { Id = 0, NombreModelo = "Focus", MarcaId = 20 };
            var recargado = new Modelo
            {
                Id = 3,
                NombreModelo = "Focus",
                MarcaId = 20,
                Marca = new Marca { Id = 20, NombreMarca = "Ford" }
            };

            var repo = new Mock<IModeloRepository>();
            repo.Setup(r => r.AddAsync(input)).Returns(Task.CompletedTask);
            repo.Setup(r => r.GetByIdAsync(input.Id)).ReturnsAsync(recargado);
            var handler = new RegistrarModeloHandler(repo.Object, CrearMapper());

            // Act
            var dto = await handler.HandleAsync(input);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(3, dto.Id);
            Assert.Equal("Focus", dto.NombreModelo);
            Assert.Equal("Ford", dto.MarcaNombre);
            repo.Verify(r => r.AddAsync(input), Times.Once);
            repo.Verify(r => r.GetByIdAsync(input.Id), Times.Once);
        }

        [Fact]
        public async Task ModificarModeloHandler_ActualizaYRetornaDtoConMarcaNombre()
        {
            // Arrange
            var input = new Modelo { Id = 4, NombreModelo = "Cruze", MarcaId = 30 };
            var recargado = new Modelo
            {
                Id = 4,
                NombreModelo = "Cruze",
                MarcaId = 30,
                Marca = new Marca { Id = 30, NombreMarca = "Chevrolet" }
            };

            var repo = new Mock<IModeloRepository>();
            repo.Setup(r => r.UpdateAsync(input)).Returns(Task.CompletedTask);
            repo.Setup(r => r.GetByIdAsync(input.Id)).ReturnsAsync(recargado);
            var handler = new ModificarModeloHandler(repo.Object, CrearMapper());

            // Act
            var dto = await handler.HandleAsync(input);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(4, dto.Id);
            Assert.Equal("Cruze", dto.NombreModelo);
            Assert.Equal("Chevrolet", dto.MarcaNombre);
            repo.Verify(r => r.UpdateAsync(input), Times.Once);
            repo.Verify(r => r.GetByIdAsync(input.Id), Times.Once);
        }

        [Fact]
        public async Task ObtenerDashboardHandler_Administrador_ConsultaModelosYServiciosUltimos6Meses()
        {
            // Arrange
            var prevUser = UserSession.CurrentUser;
            UserSession.CurrentUser = new UsuarioDto
            {
                IdUsuario = 1,
                Nombre = "Admin",
                IdRol = (int)RolesSistema.Administrador
            };

            try
            {
                var repoRegistro = new Mock<IRegistroServicioRepository>();
                repoRegistro.Setup(r => r.GetReporteCompletoAsync(null))
                    .ReturnsAsync(new List<RegistroServicio>());

                var repoReportes = new Mock<IReporteGerencialRepository>();
                repoReportes.Setup(r => r.ObtenerIngresosPorFechaAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(new List<ReporteIngresoDto>());
                repoReportes.Setup(r => r.ObtenerIngresosMensualesAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                    .ReturnsAsync(new List<ReporteIngresoMensualDto>());
                repoReportes.Setup(r => r.ObtenerModelosMasReparadosPaginadoAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), 1, 8))
                    .ReturnsAsync((new List<ReporteModeloReparadoDto>(), 0));
                repoReportes.Setup(r => r.ObtenerServiciosPorRangoPaginadoAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), 1, 0))
                    .ReturnsAsync((new List<ReporteServicioCantidadDto>(), 0));

                var handler = new ObtenerDashboardHandler(repoRegistro.Object, repoReportes.Object);

                // Act
                var metricas = await handler.HandleAsync();

                // Assert
                Assert.NotNull(metricas);
                Assert.True(metricas.MostrarGraficosGerenciales);

                // Verifica que Modelos y Servicios se consulten con la fecha de inicio de 5 meses atrás
                var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var fechaEsperadaDesde = inicioMes.AddMonths(-5);

                repoReportes.Verify(r => r.ObtenerModelosMasReparadosPaginadoAsync(
                    It.Is<DateTime?>(d => d.HasValue && d.Value.Date == fechaEsperadaDesde.Date),
                    It.Is<DateTime?>(d => d.HasValue && d.Value.Date == DateTime.Today),
                    1, 8), Times.Once);

                repoReportes.Verify(r => r.ObtenerServiciosPorRangoPaginadoAsync(
                    It.Is<DateTime?>(d => d.HasValue && d.Value.Date == fechaEsperadaDesde.Date),
                    It.Is<DateTime?>(d => d.HasValue && d.Value.Date == DateTime.Today),
                    1, 0), Times.Once);
            }
            finally
            {
                UserSession.CurrentUser = prevUser;
            }
        }
    }
}
