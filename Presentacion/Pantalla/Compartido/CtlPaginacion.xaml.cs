// walterg20
using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Presentacion.ViewModels;

namespace TP_ControlVehicular.Presentacion.Pantalla.Compartido
{
    /// <summary>
    /// Barra de paginación reutilizable para las pantallas de listado.
    /// Recibe su estado a través de la propiedad <see cref="Model"/> (<see cref="IPaginacion"/>).
    /// </summary>
    public partial class CtlPaginacion : UserControl
    {
        /// <summary>DependencyProperty que expone el modelo de paginación al control.</summary>
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            nameof(Model),
            typeof(IPaginacion),
            typeof(CtlPaginacion),
            new PropertyMetadata(null, OnModelChanged));

        public CtlPaginacion()
        {
            InitializeComponent();
        }

        /// <summary>Modelo de paginación que alimenta la barra.</summary>
        public IPaginacion? Model
        {
            get => (IPaginacion?)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtlPaginacion ctl)
            {
                // El contenido se enlaza contra el modelo, no contra el DataContext de la pantalla.
                ctl.LayoutRoot.DataContext = e.NewValue;
            }
        }
    }
}
