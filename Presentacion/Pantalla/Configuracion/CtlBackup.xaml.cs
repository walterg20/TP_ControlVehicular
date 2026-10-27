// walterg20
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Configuracion
{
    public partial class CtlBackup : UserControl
    {
        public CtlBackup()
        {
            InitializeComponent();
            try
            {
                if (App.ServiceProvider.GetService(typeof(BackupViewModel)) is BackupViewModel vmBackup)
                {
                    this.DataContext = vmBackup;
                    this.Loaded += async (s, e) => { await vmBackup.LoadAsync(); };
                }
            }
            catch
            {
                // Silenciar si DI no está listo
            }
        }
    }
}
