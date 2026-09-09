using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Login
{
    /// <summary>
    /// Interaction logic for CtlLogin.xaml
    /// </summary>
    public partial class CtlLogin : UserControl
    {
        public LoginViewModel? ViewModel => DataContext as LoginViewModel;

        public CtlLogin()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider != null)
                {
                    var vm = App.ServiceProvider.GetService(typeof(LoginViewModel)) as LoginViewModel;
                    if (vm != null)
                    {
                        DataContext = vm;
                    }
                }
            }
            catch
            {
                // Fallback handled by caller
            }
        }

        public CtlLogin(LoginViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Contrasena = txtPassword.Password;
            }
        }
    }
}
