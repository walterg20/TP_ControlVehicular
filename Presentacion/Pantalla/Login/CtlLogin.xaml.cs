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

        private bool _isSyncing = false;

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            if (ViewModel != null)
            {
                ViewModel.Contrasena = txtPassword.Password;
            }
            txtPasswordVisible.Text = txtPassword.Password;
            _isSyncing = false;
        }

        private void TxtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            if (ViewModel != null)
            {
                ViewModel.Contrasena = txtPasswordVisible.Text;
            }
            txtPassword.Password = txtPasswordVisible.Text;
            _isSyncing = false;
        }

        private void BtnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Visibility == Visibility.Visible)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Visibility = Visibility.Visible;
                btnTogglePassword.Content = "🙈";
                txtPasswordVisible.Focus();
                txtPasswordVisible.SelectionStart = txtPasswordVisible.Text.Length;
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                btnTogglePassword.Content = "👁️";
                txtPassword.Focus();
            }
        }
    }
}
