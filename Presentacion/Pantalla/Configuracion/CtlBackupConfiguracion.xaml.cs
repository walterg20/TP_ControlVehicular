// walterg20
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.Pantalla.Configuracion
{
    public partial class CtlBackupConfiguracion : UserControl
    {
        public CtlBackupConfiguracion()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider.GetService(typeof(BackupConfiguracionViewModel)) is BackupConfiguracionViewModel vmBackupConfiguracion)
                {
                    this.DataContext = vmBackupConfiguracion;
                    this.Loaded += async (s, e) => { await vmBackupConfiguracion.LoadAsync(); };
                }
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "CtlBackupConfiguracion.xaml");
                // Silenciar si DI no está listo
            }
        }
    }
}
