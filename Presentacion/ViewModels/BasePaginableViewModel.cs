// walterg20
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Base para los ViewModels de listados paginados. Centraliza el estado de paginacion
    /// (pagina actual, total de paginas, total de registros, tamano de pagina), los comandos
    /// de navegacion y el ajuste (clamp) del indice de pagina.
    /// Las implementaciones deben proporcionar los registros de la pagina solicitada
    /// mediante <see cref="CargarPaginaAsync"/>.
    /// </summary>
    public abstract class BasePaginableViewModel : BaseViewModel, IPaginacion
    {
        /// <summary>Tamano de pagina minimo aceptado.</summary>
        public const int TamanoPaginaMinimo = 5;

        /// <summary>Tamano de pagina maximo aceptado.</summary>
        public const int TamanoPaginaMaximo = 200;

        private int _paginaActual = 1;
        private int _totalPaginas = 1;
        private int _totalRegistros;
        private int _tamanoPagina = 10;
        private string _textoPaginacion = "Página 1 de 1 (0 registros)";

        protected BasePaginableViewModel()
        {
            IrPrimeraPaginaCommand = new RelayCommand(async () => await CambiarPaginaAsync(1), () => PuedeIrAnterior);
            PaginaAnteriorCommand = new RelayCommand(async () => await CambiarPaginaAsync(PaginaActual - 1), () => PuedeIrAnterior);
            PaginaSiguienteCommand = new RelayCommand(async () => await CambiarPaginaAsync(PaginaActual + 1), () => PuedeIrSiguiente);
            IrUltimaPaginaCommand = new RelayCommand(async () => await CambiarPaginaAsync(TotalPaginas), () => PuedeIrSiguiente);
        }

        // ---------------------------------------------------------------------
        // IPaginacion
        // ---------------------------------------------------------------------

        public int PaginaActual
        {
            get => _paginaActual;
            set
            {
                if (SetProperty(ref _paginaActual, value))
                {
                    NotificarComandosPaginacion();
                    ActualizarTextoPaginacion();
                }
            }
        }

        public int TotalPaginas
        {
            get => _totalPaginas;
            private set
            {
                if (SetProperty(ref _totalPaginas, value))
                {
                    NotificarComandosPaginacion();
                    ActualizarTextoPaginacion();
                }
            }
        }

        public int TotalRegistros
        {
            get => _totalRegistros;
            private set
            {
                if (SetProperty(ref _totalRegistros, value))
                {
                    ActualizarTextoPaginacion();
                }
            }
        }

        public int TamanoPagina
        {
            get => _tamanoPagina;
            set
            {
                var normalizado = NormalizarTamanoPagina(value);
                if (SetProperty(ref _tamanoPagina, normalizado))
                {
                    // Cambiar el tamano de pagina reinicia el listado en la primera pagina.
                    PaginaActual = 1;
                    if (!IsLoading)
                    {
                        _ = CargarPaginaAsync();
                    }
                }
            }
        }

        public ObservableCollection<int> OpcionesTamanoPagina { get; } = new() { 5, 10, 20, 50 };

        public string TextoPaginacion
        {
            get => _textoPaginacion;
            private set => SetProperty(ref _textoPaginacion, value);
        }

        public bool PuedeIrAnterior => PaginaActual > 1 && !IsLoading;

        public bool PuedeIrSiguiente => PaginaActual < TotalPaginas && !IsLoading;

        public RelayCommand IrPrimeraPaginaCommand { get; }

        public RelayCommand PaginaAnteriorCommand { get; }

        public RelayCommand PaginaSiguienteCommand { get; }

        public RelayCommand IrUltimaPaginaCommand { get; }

        // Implementacion explicita para exponer los comandos como ICommand sin cambiar
        // el tipo publico concreto (RelayCommand) que usa el resto de la aplicacion.
        ICommand IPaginacion.IrPrimeraPaginaCommand => IrPrimeraPaginaCommand;
        ICommand IPaginacion.PaginaAnteriorCommand => PaginaAnteriorCommand;
        ICommand IPaginacion.PaginaSiguienteCommand => PaginaSiguienteCommand;
        ICommand IPaginacion.IrUltimaPaginaCommand => IrUltimaPaginaCommand;

        /// <summary>
        /// Navega a la pagina indicada. El indice se ajusta al rango valido
        /// (1 .. TotalPaginas) antes de recargar los datos.
        /// </summary>
        public virtual async Task CambiarPaginaAsync(int nuevaPagina)
        {
            if (nuevaPagina < 1) nuevaPagina = 1;
            if (nuevaPagina > TotalPaginas && TotalPaginas > 0) nuevaPagina = TotalPaginas;
            if (nuevaPagina == PaginaActual) return;

            PaginaActual = nuevaPagina;
            await CargarPaginaAsync();
        }

        // ---------------------------------------------------------------------
        // Soporte para las implementaciones
        // ---------------------------------------------------------------------

        /// <summary>
        /// Carga en la coleccion expuesta los registros de la pagina actual.
        /// La implementacion debe llamar a <see cref="ActualizarTotales"/> con la
        /// cantidad total de registros que cumplen los filtros.
        /// </summary>
        protected abstract Task CargarPaginaAsync();

        /// <summary>
        /// Actualiza el total de registros y recalcula el total de paginas,
        /// ajustando la pagina actual si quedo fuera de rango.
        /// </summary>
        public void ActualizarTotales(int totalRegistros)
        {
            TotalRegistros = totalRegistros;
            TotalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling(totalRegistros / (double)TamanoPagina)
                : 1;

            if (PaginaActual > TotalPaginas)
            {
                _paginaActual = TotalPaginas;
                OnPropertyChanged(nameof(PaginaActual));
            }

            NotificarComandosPaginacion();
            ActualizarTextoPaginacion();
        }

        /// <summary>Reinicia el listado en la primera pagina.</summary>
        public void ReiniciarPagina() => PaginaActual = 1;

        /// <summary>Notifica el cambio de estado de los comandos de navegacion.</summary>
        protected void NotificarComandosPaginacion()
        {
            OnPropertyChanged(nameof(PuedeIrAnterior));
            OnPropertyChanged(nameof(PuedeIrSiguiente));
            IrPrimeraPaginaCommand?.RaiseCanExecuteChanged();
            PaginaAnteriorCommand?.RaiseCanExecuteChanged();
            PaginaSiguienteCommand?.RaiseCanExecuteChanged();
            IrUltimaPaginaCommand?.RaiseCanExecuteChanged();
        }

        private void ActualizarTextoPaginacion()
        {
            TextoPaginacion = $"Página {PaginaActual} de {TotalPaginas} ({TotalRegistros} {(TotalRegistros == 1 ? "registro" : "registros")})";
        }

        private static int NormalizarTamanoPagina(int valor)
        {
            if (valor < TamanoPaginaMinimo) return TamanoPaginaMinimo;
            if (valor > TamanoPaginaMaximo) return TamanoPaginaMaximo;
            return valor;
        }

        /// <summary>
        /// Al cambiar el estado de carga se actualiza la disponibilidad de los
        /// comandos de navegacion, que dependen de <see cref="IsLoading"/>.
        /// </summary>
        public override bool IsLoading
        {
            get => base.IsLoading;
            set
            {
                base.IsLoading = value;
                NotificarComandosPaginacion();
            }
        }
    }
}
