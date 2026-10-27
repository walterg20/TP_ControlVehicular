// walterg20
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

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
            catch
            {
                // Silenciar si DI no está listo
            }
        }
    }
}
