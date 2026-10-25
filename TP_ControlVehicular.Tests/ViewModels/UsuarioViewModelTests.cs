using Moq;
using System.Threading.Tasks;
using System;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Presentacion.ViewModels;
using Xunit;
using TP_ControlVehicular.Entidad;
using AutoMapper;
using System.Collections.Generic;

namespace TP_ControlVehicular.Tests.ViewModels
{
    public class UsuarioViewModelTests
    {
        private (UsuarioViewModel viewModel, Mock<IUsuarioRepository> mockRepo) CrearInstancia()
        {
            var mockRepo = new Mock<IUsuarioRepository>();
            var mockRolRepo = new Mock<IRolRepository>();
            var mockMapper = new Mock<IMapper>();

            var listarHandler = new ListarUsuariosHandler(mockRepo.Object, mockMapper.Object);
            var regHandler = new RegistrarUsuarioHandler(mockRepo.Object, mockMapper.Object);
            var modHandler = new ModificarUsuarioHandler(mockRepo.Object, mockMapper.Object);

            mockRepo.Setup(r => r.GetWithRolAsync()).ReturnsAsync(new List<Usuario>());
            mockRolRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Rol>());

            var viewModel = new UsuarioViewModel(listarHandler, regHandler, modHandler, mockRolRepo.Object, mockMapper.Object);
            return (viewModel, mockRepo);
        }

        [Fact]
        public async Task GuardarUsuario_FaltanDatosObligatorios_NoDeberiaGuardar()
        {
            var (viewModel, mockRepo) = CrearInstancia();

            // Set empty properties to trigger validation errors
            viewModel.Nombre = string.Empty;
            viewModel.Apellido = string.Empty;
            viewModel.Dni = string.Empty;
            
            // Execute the command
            viewModel.RegistrarCommand.Execute(null);
            await Task.Delay(50);

            // Repository should NOT have been called
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
            Assert.True(viewModel.HasErrors);
        }

        [Fact]
        public void Validacion_MenorDe18Anios_DeberiaMostrarError()
        {
            var (viewModel, _) = CrearInstancia();
            
            // Set age under 18
            viewModel.FechaNacimiento = DateTime.Today.AddYears(-15);
            
            Assert.True(viewModel.HasErrors);
            var errores = viewModel.GetErrors(nameof(viewModel.FechaNacimiento));
            Assert.Contains(errores.Cast<string>(), e => e.Contains("18 años"));
        }
    }
}
