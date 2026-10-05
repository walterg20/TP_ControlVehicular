using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Reporte
{
    public partial class ReporteGerencialView : UserControl
    {
        public ReporteGerencialView(ReporteGerencialViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
