using Moq;
using System.Threading;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Context;
using Xunit;
using System.Windows;

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
    }
}
