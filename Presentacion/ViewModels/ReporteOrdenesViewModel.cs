using System.Collections.ObjectModel;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ReporteOrdenesViewModel : BaseViewModel
    {
        private readonly ObtenerReporteOrdenesHandler _obtenerReporteOrdenesHandler;

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    AplicarFiltros();
                }
            }
        }

        private string _estadoFiltro = "Todos";
        public string EstadoFiltro
        {
            get => _estadoFiltro;
            set
            {
                if (SetProperty(ref _estadoFiltro, value))
                {
                    AplicarFiltros();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private readonly List<OrdenTrabajoReporteDto> _todosLosRegistros = new();
        public ObservableCollection<OrdenTrabajoReporteDto> ListadoOrdenesFiltered { get; } = new();

        public ICommand CargarReporteCommand { get; }

        public ReporteOrdenesViewModel(ObtenerReporteOrdenesHandler obtenerReporteOrdenesHandler)
        {
            _obtenerReporteOrdenesHandler = obtenerReporteOrdenesHandler;
            CargarReporteCommand = new RelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var resultados = await _obtenerReporteOrdenesHandler.HandleAsync();
                _todosLosRegistros.Clear();
                _todosLosRegistros.AddRange(resultados);
                AplicarFiltros();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AplicarFiltros()
        {
            ListadoOrdenesFiltered.Clear();

            var query = _todosLosRegistros.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                var txt = TextoBusqueda.Trim().ToLower();
                query = query.Where(r =>
                    r.Patente.ToLower().Contains(txt) ||
                    r.MarcaModelo.ToLower().Contains(txt) ||
                    r.ClienteNombre.ToLower().Contains(txt) ||
                    r.MecanicoNombre.ToLower().Contains(txt) ||
                    r.RecepcionistaNombre.ToLower().Contains(txt) ||
                    r.ServiciosAplicados.ToLower().Contains(txt));
            }

            if (!string.IsNullOrEmpty(EstadoFiltro) && EstadoFiltro != "Todos")
            {
                query = query.Where(r => r.Estado.Equals(EstadoFiltro, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var item in query)
            {
                ListadoOrdenesFiltered.Add(item);
            }
        }
    }
}
