// walterg20
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TP_ControlVehicular.Negocio.DTOs;

namespace TP_ControlVehicular.Presentacion.Pantalla.Compartido
{
    /// <summary>
    /// Control reutilizable para mostrar ayuda contextual con secciones expandibles.
    /// </summary>
    public partial class CtlAyudaContextual : UserControl
    {
        /// <summary>
        /// DependencyProperty para la colección de secciones de ayuda.
        /// </summary>
        public static readonly DependencyProperty SeccionesProperty =
            DependencyProperty.Register(
                nameof(Secciones),
                typeof(IEnumerable<AyudaSeccionDto>),
                typeof(CtlAyudaContextual),
                new PropertyMetadata(null, OnSeccionesChanged));

        /// <summary>
        /// Propiedad que contiene las secciones de ayuda a mostrar.
        /// </summary>
        public IEnumerable<AyudaSeccionDto>? Secciones
        {
            get => (IEnumerable<AyudaSeccionDto>?)GetValue(SeccionesProperty);
            set => SetValue(SeccionesProperty, value);
        }

        public CtlAyudaContextual()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Manejador del cambio en la propiedad Secciones.
        /// </summary>
        private static void OnSeccionesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtlAyudaContextual control)
            {
                control.ActualizarAyudas();
            }
        }

        /// <summary>
        /// Actualiza la visualización de ayudas cuando cambia la propiedad Secciones.
        /// </summary>
        private void ActualizarAyudas()
        {
            var secciones = Secciones?.ToList() ?? new List<AyudaSeccionDto>();

            if (secciones.Count == 0)
            {
                // No hay secciones: mostrar mensaje "Sin ayuda disponible"
                LstAyudas.ItemsSource = null;
                TxtSinAyuda.Visibility = Visibility.Visible;
            }
            else
            {
                // Hay secciones: ordenar por Orden y mostrar
                LstAyudas.ItemsSource = secciones.OrderBy(s => s.Orden).ToList();
                TxtSinAyuda.Visibility = Visibility.Collapsed;
            }
        }
    }
}
