using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Reportes;
using TP_ControlVehicular.Presentacion.ViewModels;
using TP_ControlVehicular.Negocio.Reportes.Documentos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ReporteGerencialViewModel : BaseViewModel
    {
        private readonly IReporteGerencialRepository _repository;

        private DateTime? _fechaDesde;
        public DateTime? FechaDesde
        {
            get => _fechaDesde;
            set => SetProperty(ref _fechaDesde, value);
        }

        private DateTime? _fechaHasta;
        public DateTime? FechaHasta
        {
            get => _fechaHasta;
            set => SetProperty(ref _fechaHasta, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _mensajeValidacion = string.Empty;
        public string MensajeValidacion
        {
            get => _mensajeValidacion;
            set => SetProperty(ref _mensajeValidacion, value);
        }

        public ObservableCollection<ReporteIngresoDto> Ingresos { get; } = new();
        public ObservableCollection<ReporteTopClienteDto> TopClientes { get; } = new();
        public ObservableCollection<ReporteModeloReparadoDto> ModelosReparados { get; } = new();
        public ObservableCollection<ReporteServicioCantidadDto> ServiciosPorRango { get; } = new();

        public ICommand CargarReporteCommand { get; }
        public ICommand ImprimirPdfIngresosCommand { get; }
        public ICommand ImprimirPdfTopClientesCommand { get; }
        public ICommand ImprimirPdfModelosCommand { get; }
        public ICommand ImprimirPdfServiciosCommand { get; }

        public ReporteGerencialViewModel(IReporteGerencialRepository repository)
        {
            _repository = repository;
            CargarReporteCommand = new RelayCommand(LoadAsync);
            ImprimirPdfIngresosCommand = new RelayCommand(ImprimirPdfIngresosAsync);
            ImprimirPdfTopClientesCommand = new RelayCommand(ImprimirPdfTopClientesAsync);
            ImprimirPdfModelosCommand = new RelayCommand(ImprimirPdfModelosAsync);
            ImprimirPdfServiciosCommand = new RelayCommand(ImprimirPdfServiciosAsync);
        }

        public async Task LoadAsync()
        {
            if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde.Value.Date > FechaHasta.Value.Date)
            {
                MensajeValidacion = "La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.";
                return;
            }

            MensajeValidacion = string.Empty;
            IsLoading = true;
            try
            {
                var ingresos = await _repository.ObtenerIngresosPorFechaAsync(FechaDesde, FechaHasta);
                Ingresos.Clear();
                foreach (var i in ingresos) Ingresos.Add(i);

                var clientes = await _repository.ObtenerTopClientesAsync(FechaDesde, FechaHasta, 10);
                TopClientes.Clear();
                foreach (var c in clientes) TopClientes.Add(c);

                var modelos = await _repository.ObtenerModelosMasReparadosAsync(FechaDesde, FechaHasta, 10);
                ModelosReparados.Clear();
                foreach (var m in modelos) ModelosReparados.Add(m);

                var servicios = await _repository.ObtenerServiciosPorRangoAsync(FechaDesde, FechaHasta);
                ServiciosPorRango.Clear();
                foreach (var s in servicios) ServiciosPorRango.Add(s);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ImprimirPdfIngresosAsync()
        {
            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ReporteIngresos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Ingresos Totales por Fecha"));
                    page.Content().Element(ComposeIngresosContent);
                    page.Footer().Element(c => c.ComposePieDePagina());
                });
            });

            await Task.Run(() => document.GeneratePdf(filePath));
            AbrirPdf(filePath);
        }

        private void ComposeIngresosContent(IContainer container)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column => 
            {
                var total = Ingresos.Sum(i => i.IngresosTotales);
                column.Item().PaddingBottom(10).Text($"Total Recaudado en el periodo: {total:C}").FontSize(14).Bold();

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Fecha").Bold();
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Facturas Generadas").Bold();
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Ingresos").Bold();
                    });

                    foreach (var ingreso in Ingresos)
                    {
                        table.Cell().Element(c => c.CeldaDato()).Text(ingreso.Fecha.ToString("dd/MM/yyyy"));
                        table.Cell().Element(c => c.CeldaDato()).Text(ingreso.CantidadFacturas.ToString());
                        table.Cell().Element(c => c.CeldaDato()).Text(ingreso.IngresosTotales.ToString("C"));
                    }
                });
            });
        }

        private async Task ImprimirPdfTopClientesAsync()
        {
            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ReporteTopClientes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Top 10 Clientes Frecuentes"));
                    page.Content().Element(ComposeTopClientesContent);
                    page.Footer().Element(c => c.ComposePieDePagina());
                });
            });

            await Task.Run(() => document.GeneratePdf(filePath));
            AbrirPdf(filePath);
        }

        private void ComposeTopClientesContent(IContainer container)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column => 
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Cliente").Bold();
                        header.Cell().Element(c => c.CeldaCabecera()).Text("DNI").Bold();
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Cant. Servicios").Bold();
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Total Gastado").Bold();
                    });

                    foreach (var c in TopClientes)
                    {
                        table.Cell().Element(cell => cell.CeldaDato()).Text(c.NombreCompleto);
                        table.Cell().Element(cell => cell.CeldaDato()).Text(c.Dni);
                        table.Cell().Element(cell => cell.CeldaDato()).Text(c.CantidadServicios.ToString());
                        table.Cell().Element(cell => cell.CeldaDato()).Text(c.TotalGastado.ToString("C"));
                    }
                });
            });
        }

        private async Task ImprimirPdfModelosAsync()
        {
            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ReporteModelosReparados_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Modelos de Vehículos Más Reparados"));
                    page.Content().Element(ComposeModelosContent);
                    page.Footer().Element(c => c.ComposePieDePagina());
                });
            });

            await Task.Run(() => document.GeneratePdf(filePath));
            AbrirPdf(filePath);
        }

        private void ComposeModelosContent(IContainer container)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column => 
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Modelo").Bold();
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Cant. Reparaciones").Bold();
                    });

                    foreach (var m in ModelosReparados)
                    {
                        table.Cell().Element(c => c.CeldaDato()).Text(m.MarcaModelo);
                        table.Cell().Element(c => c.CeldaDato()).Text(m.CantidadReparaciones.ToString());
                    }
                });
            });
        }

        private async Task ImprimirPdfServiciosAsync()
        {
            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ReporteServiciosPorRango_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Cantidad de Servicios por Rango de Fechas"));
                    page.Content().Element(ComposeServiciosContent);
                    page.Footer().Element(c => c.ComposePieDePagina());
                });
            });

            await Task.Run(() => document.GeneratePdf(filePath));
            AbrirPdf(filePath);
        }

        private void ComposeServiciosContent(IContainer container)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                var total = ServiciosPorRango.Sum(s => s.Cantidad);
                var rango = (FechaDesde.HasValue || FechaHasta.HasValue)
                    ? $"{FechaDesde:dd/MM/yyyy} - {FechaHasta:dd/MM/yyyy}"
                    : "Todo el periodo";
                column.Item().PaddingBottom(10).Text($"Servicios realizados en el rango ({rango}) - Total: {total}").FontSize(14).Bold();

                if (ServiciosPorRango.Count > 0)
                {
                    var series = ServiciosPieChartGenerator.Normalizar(
                        ServiciosPorRango.Select(s => (s.Servicio, s.Cantidad)).ToList());

                    column.Item().PaddingVertical(10).Row(row =>
                    {
                        row.RelativeItem(2).Svg(ServiciosPieChartGenerator.BuildSvg(series));

                        row.RelativeItem(3).PaddingLeft(20).AlignMiddle().Column(legend =>
                        {
                            for (var i = 0; i < series.Count; i++)
                            {
                                var index = i;
                                var color = ServiciosPieChartGenerator.Paleta[index % ServiciosPieChartGenerator.Paleta.Length];
                                var porcentaje = total > 0 ? (double)series[index].Cantidad / total * 100.0 : 0;
                                legend.Item().PaddingVertical(3).Row(line =>
                                {
                                    line.ConstantItem(14).Height(14).Background(color);
                                    line.RelativeItem().PaddingLeft(8).Text($"{series[index].Etiqueta} ({series[index].Cantidad} - {porcentaje:0.#}%)").FontSize(10);
                                });
                            }
                        });
                    });
                }

                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Servicio").Bold();
                        header.Cell().Element(c => c.CeldaCabecera()).Text("Cantidad").Bold();
                    });

                    foreach (var s in ServiciosPorRango)
                    {
                        table.Cell().Element(c => c.CeldaDato()).Text(s.Servicio);
                        table.Cell().Element(c => c.CeldaDato()).Text(s.Cantidad.ToString());
                    }
                });
            });
        }

        private void AbrirPdf(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al abrir PDF: {ex.Message}");
            }
        }
    }
}
