using System.Collections.ObjectModel;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP_ControlVehicular.Negocio.Reportes.Documentos;

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

        private DateTime? _fechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime? FechaDesde
        {
            get => _fechaDesde;
            set
            {
                if (SetProperty(ref _fechaDesde, value))
                {
                    AplicarFiltros();
                }
            }
        }

        private DateTime? _fechaHasta = DateTime.Today;
        public DateTime? FechaHasta
        {
            get => _fechaHasta;
            set
            {
                if (SetProperty(ref _fechaHasta, value))
                {
                    AplicarFiltros();
                }
            }
        }

        private string _mensajeValidacionFechas = string.Empty;
        public string MensajeValidacionFechas
        {
            get => _mensajeValidacionFechas;
            set => SetProperty(ref _mensajeValidacionFechas, value);
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
        public ICommand ImprimirPdfCommand { get; }
        private ICommand? _limpiarFiltrosCommand;
        public ICommand LimpiarFiltrosCommand => _limpiarFiltrosCommand ??= new RelayCommand(() =>
        {
            TextoBusqueda = string.Empty;
            EstadoFiltro = "Todos";
            FechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            FechaHasta = DateTime.Today;
            return Task.CompletedTask;
        });

        public ReporteOrdenesViewModel(ObtenerReporteOrdenesHandler obtenerReporteOrdenesHandler)
        {
            _obtenerReporteOrdenesHandler = obtenerReporteOrdenesHandler;
            CargarReporteCommand = new RelayCommand(LoadAsync);
            ImprimirPdfCommand = new RelayCommand(ImprimirPdfAsync);
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

            if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde.Value.Date > FechaHasta.Value.Date)
            {
                MensajeValidacionFechas = "La fecha 'Desde' no puede ser posterior a 'Hasta'.";
                return;
            }
            MensajeValidacionFechas = string.Empty;

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

            if (FechaDesde.HasValue)
            {
                query = query.Where(r => r.FechaIngreso >= FechaDesde.Value.Date);
            }

            if (FechaHasta.HasValue)
            {
                query = query.Where(r => r.FechaIngreso.Date <= FechaHasta.Value.Date);
            }

            foreach (var item in query)
            {
                ListadoOrdenesFiltered.Add(item);
            }
        }

        private async Task ImprimirPdfAsync()
        {
            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ReporteOrdenes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                    page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Reporte de Órdenes de Trabajo"));
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(c => c.ComposePieDePagina());
                });
            });

            await Task.Run(() => document.GeneratePdf(filePath));

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Handle or log exception
                System.Diagnostics.Debug.WriteLine($"Error al abrir PDF: {ex.Message}");
            }
        }

        private string ObtenerTextoFiltros()
        {
            var filtrosStr = $"Estado: {EstadoFiltro}";
            if (FechaDesde.HasValue) filtrosStr += $" | Desde: {FechaDesde.Value:dd/MM/yyyy}";
            if (FechaHasta.HasValue) filtrosStr += $" | Hasta: {FechaHasta.Value:dd/MM/yyyy}";
            if (!string.IsNullOrWhiteSpace(TextoBusqueda)) filtrosStr += $" | Búsqueda: '{TextoBusqueda}'";
            return filtrosStr;
        }

        private void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {
            container.PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(column => 
            {
                column.Item().PaddingBottom(10).Text($"Filtros aplicados: {ObtenerTextoFiltros()}").FontSize(11).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50); // N° Orden
                        columns.ConstantColumn(80); // Fecha
                        columns.ConstantColumn(70); // Patente
                        columns.RelativeColumn(2);  // Vehículo
                        columns.RelativeColumn(2);  // Cliente
                        columns.RelativeColumn(3);  // Servicios
                        columns.ConstantColumn(80); // Estado
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("N° Orden");
                        header.Cell().Element(CellStyle).Text("Fecha");
                        header.Cell().Element(CellStyle).Text("Patente");
                        header.Cell().Element(CellStyle).Text("Vehículo");
                        header.Cell().Element(CellStyle).Text("Cliente");
                        header.Cell().Element(CellStyle).Text("Servicios");
                        header.Cell().Element(CellStyle).Text("Estado");

                        static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Black);
                        }
                    });

                    foreach (var orden in ListadoOrdenesFiltered)
                    {
                        table.Cell().Element(Block).Text(orden.IdOrden.ToString());
                        table.Cell().Element(Block).Text(orden.FechaIngreso.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Element(Block).Text(orden.Patente);
                        table.Cell().Element(Block).Text(orden.MarcaModelo);
                        table.Cell().Element(Block).Text(orden.ClienteNombre);
                        table.Cell().Element(Block).Text(orden.ServiciosAplicados);
                        table.Cell().Element(Block).Text(orden.Estado);

                        static QuestPDF.Infrastructure.IContainer Block(QuestPDF.Infrastructure.IContainer container)
                        {
                            return container.PaddingVertical(2).PaddingHorizontal(2);
                        }
                    }
                });
            });
        }
    }
}
