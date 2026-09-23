using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Presentacion.Cliente;
using TP_ControlVehicular.Presentacion.Marca;
using TP_ControlVehicular.Presentacion.Modelo;
using TP_ControlVehicular.Presentacion.Pantalla.Dashboard;
using TP_ControlVehicular.Presentacion.Pantalla.Login;
using TP_ControlVehicular.Presentacion.Pantalla.Reporte;
using TP_ControlVehicular.Presentacion.Pantalla.Servicio;
using TP_ControlVehicular.Presentacion.Rol;
using TP_ControlVehicular.Presentacion.Taller;
using TP_ControlVehicular.Presentacion.Usuario;
using TP_ControlVehicular.Presentacion.Vehiculo;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UsuarioDto? UsuarioSesionActual { get; private set; }
        private CtlLogin? _ctlLogin;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MostrarPantallaLogin();
        }

        public void MostrarPantallaLogin()
        {
            UsuarioSesionActual = null;
            TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser = null;
            lblUsuarioNombre.Text = "👤 Usuario";
            lblUsuarioRol.Text = "🛡️ Rol: -";

            // Ocultar menú lateral y ajustar espacio a 0
            pnlSidebar.Visibility = Visibility.Collapsed;
            colMenu.Width = new GridLength(0);

            // Instanciar o resolver CtlLogin desde DI
            if (App.ServiceProvider != null)
            {
                _ctlLogin = App.ServiceProvider.GetService(typeof(CtlLogin)) as CtlLogin;
            }

            if (_ctlLogin == null)
            {
                var vm = App.ServiceProvider?.GetService(typeof(LoginViewModel)) as LoginViewModel;
                if (vm != null)
                {
                    _ctlLogin = new CtlLogin(vm);
                }
                else
                {
                    _ctlLogin = new CtlLogin();
                }
            }

            if (_ctlLogin.ViewModel != null)
            {
                _ctlLogin.ViewModel.LimpiarFormulario();
                _ctlLogin.ViewModel.OnLoginSuccess -= OnLoginExitoso;
                _ctlLogin.ViewModel.OnLoginSuccess += OnLoginExitoso;
            }

            // Inyectar CtlLogin en el área principal de la ventana
            grdContenido.Children.Clear();
            Grid.SetColumn(_ctlLogin, 0);
            Grid.SetColumnSpan(_ctlLogin, 2);
            grdContenido.Children.Add(_ctlLogin);
        }

        private void OnLoginExitoso(UsuarioDto usuario)
        {
            UsuarioSesionActual = usuario;
            TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser = usuario;

            // Mostrar el nombre del usuario logueado y su rol en la tarjeta del menú
            lblUsuarioNombre.Text = $"👤 {usuario.Nombre}";
            var rol = string.IsNullOrWhiteSpace(usuario.RolNombre) ? "Sin Rol" : usuario.RolNombre;
            lblUsuarioRol.Text = $"🛡️ Rol: {rol}";

            // Aplicar restricciones de menú según el rol asignado
            AplicarRestriccionesPorRol();

            if (_ctlLogin?.ViewModel != null)
            {
                _ctlLogin.ViewModel.OnLoginSuccess -= OnLoginExitoso;
            }

            // Limpiar y remover el formulario de Login
            grdContenido.Children.Clear();
            _ctlLogin = null;

            // Mostrar el menú lateral y restaurar su ancho
            colMenu.Width = new GridLength(240);
            pnlSidebar.Visibility = Visibility.Visible;

            // Mostrar el Dashboard por defecto tras iniciar sesión
            AgregarPagina(new CtlDashboard());
        }

        private void AplicarRestriccionesPorRol()
        {
            var currentUser = TP_ControlVehicular.Negocio.Context.UserSession.CurrentUser;
            if (currentUser == null) return;

            // Resetear visibilidad (modo Administrador)
            btnDashboard.Visibility = Visibility.Visible;
            secOperaciones.Visibility = Visibility.Visible;
            btnOrdenesTrabajo.Visibility = Visibility.Visible;
            secAdministracion.Visibility = Visibility.Visible;
            btnServicio.Visibility = Visibility.Visible;
            btnCliente.Visibility = Visibility.Visible;
            btnVehiculo.Visibility = Visibility.Visible;
            btnModelo.Visibility = Visibility.Collapsed;
            btnMarca.Visibility = Visibility.Collapsed;
            btnTaller.Visibility = Visibility.Visible;
            btnUsuario.Visibility = Visibility.Visible;
            btnRol.Visibility = Visibility.Visible;
            secReportes.Visibility = Visibility.Visible;
            btnReporteOrdenes.Visibility = Visibility.Visible;

            int rolId = currentUser.IdRol;

            if (rolId == (int)TP_ControlVehicular.Negocio.Context.RolesSistema.Recepcionista)
            {
                // Recepcionista: No administra usuarios, roles ni talleres
                btnUsuario.Visibility = Visibility.Collapsed;
                btnRol.Visibility = Visibility.Collapsed;
                btnTaller.Visibility = Visibility.Collapsed;
            }
            else if (rolId == (int)TP_ControlVehicular.Negocio.Context.RolesSistema.Mecanico)
            {
                // Mecánico: Oculta sección administración completa
                secAdministracion.Visibility = Visibility.Collapsed;
                btnUsuario.Visibility = Visibility.Collapsed;
                btnRol.Visibility = Visibility.Collapsed;
                btnTaller.Visibility = Visibility.Collapsed;
                btnCliente.Visibility = Visibility.Collapsed;
                btnVehiculo.Visibility = Visibility.Collapsed;
                btnModelo.Visibility = Visibility.Collapsed;
                btnMarca.Visibility = Visibility.Collapsed;
                btnServicio.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItem_Click_Dashboard(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlDashboard());
        }

        private void MenuItem_Click_ReporteOrdenes(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlReporteOrdenes());
        }

        private void MenuItem_Click_OrdenesTrabajo(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            if (App.ServiceProvider?.GetService(typeof(TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio.CtlOrdenServicio)) is TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio.CtlOrdenServicio ctl)
            {
                AgregarPagina(ctl);
            }
            else
            {
                AgregarPagina(new TP_ControlVehicular.Presentacion.Pantalla.OrdenServicio.CtlOrdenServicio());
            }
        }

        private void MenuItem_Click_Servicio(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            if (App.ServiceProvider?.GetService(typeof(CtlServicio)) is CtlServicio ctl)
            {
                AgregarPagina(ctl);
            }
            else
            {
                AgregarPagina(new CtlServicio());
            }
        }

        private void MenuItem_Click_Cliente(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlCliente());
        }

        private void MenuItem_Click_Taller(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlTaller());
        }

        private void MenuItem_Click_Usuario(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlUsuario());
        }

        private void MenuItem_Click_Rol(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlRol());
        }

        private void MenuItem_Click_Vehiculo(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlVehiculo());
        }

        private void MenuItem_Click_Modelo(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlModelo());
        }

        private void MenuItem_Click_Marca(object sender, RoutedEventArgs e)
        {
            ResaltarBotonActivo(sender as Button);
            AgregarPagina(new CtlMarca());
        }

        private void MenuItem_Click_Salir(object sender, RoutedEventArgs e)
        {
            // Al salir/cerrar sesión, volvemos a mostrar el login
            MostrarPantallaLogin();
        }

        private void BtnCerrarApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AgregarPagina(UserControl userControl)
        {
            this.grdContenido.Children.Clear();
            Grid.SetColumn(userControl, 0);
            var span = Math.Max(1, this.grdContenido.ColumnDefinitions.Count);
            Grid.SetColumnSpan(userControl, span);
            this.grdContenido.Children.Add(userControl);
        }

        private void ResaltarBotonActivo(Button botonActivo)
        {
            if (botonActivo == null) return;
            var transparente = System.Windows.Media.Brushes.Transparent;
            var normalWeight = FontWeights.Normal;

            btnDashboard.Background = transparente; btnDashboard.FontWeight = normalWeight;
            btnOrdenesTrabajo.Background = transparente; btnOrdenesTrabajo.FontWeight = normalWeight;
            btnServicio.Background = transparente; btnServicio.FontWeight = normalWeight;
            btnCliente.Background = transparente; btnCliente.FontWeight = normalWeight;
            btnVehiculo.Background = transparente; btnVehiculo.FontWeight = normalWeight;
            btnModelo.Background = transparente; btnModelo.FontWeight = normalWeight;
            btnMarca.Background = transparente; btnMarca.FontWeight = normalWeight;
            btnTaller.Background = transparente; btnTaller.FontWeight = normalWeight;
            btnUsuario.Background = transparente; btnUsuario.FontWeight = normalWeight;
            btnRol.Background = transparente; btnRol.FontWeight = normalWeight;
            btnReporteOrdenes.Background = transparente; btnReporteOrdenes.FontWeight = normalWeight;

            var colorAzul = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2980B9");
            botonActivo.Background = new System.Windows.Media.SolidColorBrush(colorAzul);
            botonActivo.FontWeight = FontWeights.Bold;
        }
    }
}
