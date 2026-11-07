// walterg20
using System.Collections.ObjectModel;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP_ControlVehicular.Negocio.Reportes.Documentos;
using TP_ControlVehicular.Presentacion.Helpers;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Reporte de ordenes con paginacion y busqueda en la base de datos.
    /// </summary>
    public class ReporteOrdenesViewModel : BasePaginableViewModel
    {
        private readonly ObtenerReporteOrdenesHandler _obtenerReporteOrdenesHandler;
        private readonly Debouncer _debouncer;

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    // El filtro cambio: el listado vuelve a la primera pagina.
                    ReiniciarPagina();
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
                    // El filtro cambio: el listado vuelve a la primera pagina.
                    ReiniciarPagina();
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
                    // El filtro cambio: el listado vuelve a la primera pagina.
                    ReiniciarPagina();
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
                    // El filtro cambio: el listado vuelve a la primera pagina.
                    ReiniciarPagina();
                }
            }
        }

        private string _mensajeValidacionFechas = string.Empty;
        public string MensajeValidacionFechas
        {
            get => _mensajeValidacionFechas;
            set => SetProperty(ref _mensajeValidacionFechas, value);
        }

        /// <summary>Registros de la pagina actual.</summary>
        public ObservableCollection<OrdenTrabajoReporteDto> Ordenes { get; } = new();

        public ICommand CargarReporteCommand { get; }
        public ICommand ImprimirPdfCommand { get; }
        private ICommand? _limpiarFiltrosCommand;
        public ICommand LimpiarFiltrosCommand => _limpiarFiltrosCommand ??= new RelayCommand(async () =>
        {
            TextoBusqueda = string.Empty;
            EstadoFiltro = "Todos";
            FechaDesde = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            FechaHasta = DateTime.Today;
            await BuscarAsync();
        });

        public ReporteOrdenesViewModel(ObtenerReporteOrdenesHandler obtenerReporteOrdenesHandler)
        {
            _obtenerReporteOrdenesHandler = obtenerReporteOrdenesHandler;
            _debouncer = new Debouncer();
            CargarReporteCommand = new RelayCommand(LoadAsync);
            ImprimirPdfCommand = new RelayCommand(ImprimirPdfAsync);
        }

        /// <summary>
        /// Carga la primera pagina del reporte (usada al abrir la pantalla).
        /// </summary>
        public async Task LoadAsync()
        {
            if (!ValidarFechas()) return;

            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Aplica los filtros actuales y recarga el reporte desde la primera pagina.
        /// </summary>
        public async Task BuscarAsync()
        {
            if (!ValidarFechas()) return;

            ReiniciarPagina();
            await CargarPaginaAsync();
        }

        /// <summary>
        /// Programa la busqueda con un retardo breve, para no consultar la base de datos
        /// en cada tecla escrita.
        /// </summary>
        public void ProgramarBusqueda() => _debouncer.Debounce(BuscarAsync);

        /// <summary>Cancela una busqueda programada que todavia no se ejecuto.</summary>
        public void CancelarBusquedaProgramada() => _debouncer.Cancel();

        protected override async Task CargarPaginaAsync()
        {
            IsLoading = true;
            try
            {
                var (items, total) = await _obtenerReporteOrdenesHandler.HandlePaginadoAsync(
                    TextoBusqueda.Trim(),
                EstadoFiltro,
                FechaDesde,
                FechaHasta,
                PaginaActual,
                TamanoPagina);

                Ordenes.Clear();
                foreach (var orden in items)
                {
                    Ordenes.Add(orden);
                }

                ActualizarTotales(total);
            }
            catch (Exception ex)
            {
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ReporteOrdenesViewModel");
                Ordenes.Clear();
                ActualizarTotales(0);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidarFechas()
        {
            if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde.Value.Date > FechaHasta.Value.Date)
            {
                MensajeValidacionFechas = "La fecha 'Desde' no puede ser posterior a 'Hasta'.";
                return false;
            }

            MensajeValidacionFechas = string.Empty;
            return true;
        }

        private async Task ImprimirPdfAsync()
        {
            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ReporteOrdenes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            // El PDF exporta el conjunto completo de registros que cumplen los filtros,
            // no solo la pagina visible.
            var ordenes = (await _obtenerReporteOrdenesHandler.HandleCompletoAsync(
                TextoBusqueda.Trim(),
                EstadoFiltro,
                FechaDesde,
                FechaHasta)).ToList();

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                    page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Reporte de Órdenes de Trabajo"));
                    page.Content().Element(c => ComposeContent(c, ordenes));
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
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ReporteOrdenesViewModel");
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

        private void ComposeContent(QuestPDF.Infrastructure.IContainer container, List<OrdenTrabajoReporteDto> ordenes)
        {
            container.PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(column =>
            {
                column.Item().PaddingBottom(10).Text($"Filtros aplicados: {ObtenerTextoFiltros()}").FontSize(11).FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(50); // Nº Orden
                        columns.ConstantColumn(80); // Fecha
                        columns.ConstantColumn(70); // Patente
                        columns.RelativeColumn(2);  // Vehículo
                        columns.RelativeColumn(2);  // Cliente
                        columns.RelativeColumn(3);  // Servicios
                        columns.ConstantColumn(80); // Estado
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Nº Orden");
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

                    foreach (var orden in ordenes)
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
