// walterg20
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Contrato de paginacion compartido por las pantallas de listado.
    /// Lo implementa <see cref="BasePaginableViewModel"/> y lo consume el control
    /// <c>Presentacion.Pantalla.Compartido.CtlPaginacion</c>.
    /// </summary>
    public interface IPaginacion : INotifyPropertyChanged
    {
        /// <summary>Pagina actual (base 1).</summary>
        int PaginaActual { get; set; }

        /// <summary>Cantidad total de paginas disponibles (minimo 1).</summary>
        int TotalPaginas { get; }

        /// <summary>Cantidad total de registros que cumplen los filtros activos.</summary>
        int TotalRegistros { get; }

        /// <summary>Cantidad de registros por pagina.</summary>
        int TamanoPagina { get; set; }

        /// <summary>Opciones de tamano de pagina ofrecidas al usuario.</summary>
        ObservableCollection<int> OpcionesTamanoPagina { get; }

        /// <summary>Texto de estado de la barra de paginacion.</summary>
        string TextoPaginacion { get; }

        /// <summary>Indica si se puede navegar a la pagina anterior.</summary>
        bool PuedeIrAnterior { get; }

        /// <summary>Indica si se puede navegar a la pagina siguiente.</summary>
        bool PuedeIrSiguiente { get; }

        ICommand IrPrimeraPaginaCommand { get; }
        ICommand PaginaAnteriorCommand { get; }
        ICommand PaginaSiguienteCommand { get; }
        ICommand IrUltimaPaginaCommand { get; }

        /// <summary>Navega a la pagina indicada, ajustando el valor al rango valido.</summary>
        Task CambiarPaginaAsync(int pagina);
    }
}
