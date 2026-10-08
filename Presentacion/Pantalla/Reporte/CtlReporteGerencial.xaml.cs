using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Reporte
{
    public partial class CtlReporteGerencial : UserControl
    {
        public CtlReporteGerencial()
        {
            InitializeComponent();
            var vmReporteGerencial = App.ServiceProvider.GetRequiredService<ReporteGerencialViewModel>();
            DataContext = vmReporteGerencial;

            Loaded += (s, e) =>
            {
                // Solo cargar si no estǭ cargando y las listas estǭn vacas
                if (!vmReporteGerencial.IsLoading && vmReporteGerencial.Ingresos.Count == 0)
                {
                    _ = vmReporteGerencial.LoadAsync();
                }
            };
        }
    }
}

