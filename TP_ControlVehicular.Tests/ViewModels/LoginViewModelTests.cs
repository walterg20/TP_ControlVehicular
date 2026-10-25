using Moq;
using System.Threading.Tasks;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;
using TP_ControlVehicular.Entidad;
using AutoMapper;

namespace TP_ControlVehicular.Tests.ViewModels
{
    public class LoginViewModelTests
    {
        [Fact]
        public async Task IniciarSesion_DniVacio_DeberiaMostrarError()
        {
            var mockRepo = new Mock<IUsuarioRepository>();
            var mockMapper = new Mock<IMapper>();
            var handler = new AutenticarUsuarioHandler(mockRepo.Object, mockMapper.Object);
            var viewModel = new LoginViewModel(handler);
            
            viewModel.Dni = string.Empty;
            viewModel.Contrasena = "algunaClave";

            viewModel.IniciarSesionCommand.Execute(null);
            await Task.Delay(50);

            Assert.Contains("Por favor, ingrese DNI y contraseña.", viewModel.ErrorMessage);
        }

        [Fact]
        public async Task IniciarSesion_CredencialesInvalidas_DeberiaMostrarError()
        {
            var mockRepo = new Mock<IUsuarioRepository>();
            mockRepo.Setup(r => r.GetByDniAsync(It.IsAny<string>()))
                    .ReturnsAsync((Usuario?)null);
                    
            var mockMapper = new Mock<IMapper>();
            var handler = new AutenticarUsuarioHandler(mockRepo.Object, mockMapper.Object);
            var viewModel = new LoginViewModel(handler);
            
            viewModel.Dni = "11111111";
            viewModel.Contrasena = "claveMala";

            viewModel.IniciarSesionCommand.Execute(null);
            await Task.Delay(50);

            Assert.Contains("El DNI ingresado no se encuentra registra", viewModel.ErrorMessage ?? string.Empty);
        }
    }
}
