// walterg20
using System;
using System.Threading;
using System.Windows;
using Xunit;

namespace TP_ControlVehicular.Tests.ViewModels
{
    /// <summary>
    /// Verifica que los UserControl con XAML compilen y se instancien sin errores de
    /// parseo en tiempo de ejecucion (por ejemplo, un ItemTemplate junto a un
    /// DisplayMemberPath, que WPF rechaza en runtime aunque el build pase).
    /// </summary>
    public class CargaDeUserControlsTests
    {
        private static void EjecutarEnThreadSta(Action accion, out Exception? error)
        {
            Exception? capturado = null;

            var hilo = new Thread(() =>
            {
                try
                {
                    accion();
                }
                catch (Exception ex)
                {
                    capturado = ex;
                }
            });

            hilo.SetApartmentState(ApartmentState.STA);
            hilo.Start();
            hilo.Join();

            error = capturado;
        }

        [Fact]
        public void CtlBuscadorUniversal_SeInstanciaSinErrorDeParseoXaml()
        {
            EjecutarEnThreadSta(() =>
            {
                var uri = new Uri(
                    "/TP_ControlVehicular;component/presentacion/pantalla/ordenservicio/ctlbuscadoruniversal.xaml",
                    UriKind.Relative);

                var control = Application.LoadComponent(uri);
                Assert.NotNull(control);
            }, out Exception? error);

            Assert.Null(error);
        }

        [Fact]
        public void FrmClienteVehiculo_SeInstanciaSinErrorDeParseoXaml()
        {
            EjecutarEnThreadSta(() =>
            {
                var uri = new Uri(
                    "/TP_ControlVehicular;component/presentacion/pantalla/ordenservicio/frmclientevehiculo.xaml",
                    UriKind.Relative);

                var ventana = Application.LoadComponent(uri);
                Assert.NotNull(ventana);
            }, out Exception? error);

            Assert.Null(error);
        }

        [Fact]
        public void CtlOrdenServicioForm_SeInstanciaSinErrorDeParseoXaml()
        {
            EjecutarEnThreadSta(() =>
            {
                var uri = new Uri(
                    "/TP_ControlVehicular;component/presentacion/pantalla/ordenservicio/ctlordenservicioform.xaml",
                    UriKind.Relative);

                var control = Application.LoadComponent(uri);
                Assert.NotNull(control);
            }, out Exception? error);

            Assert.Null(error);
        }
    }
}
