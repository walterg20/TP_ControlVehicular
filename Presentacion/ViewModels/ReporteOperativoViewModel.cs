using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Reportes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ReporteOperativoViewModel : BaseViewModel
    {
        private readonly ObtenerReporteOrdenesHandler _obtenerReportesHandler;
        private readonly IReporteGerencialRepository _reporteGerencialRepo;

        public ObservableCollection<ProductividadMecanicoDto> Productividad { get; } = new();
        public ObservableCollection<ReporteModeloReparadoDto> VehiculosFrecuentes { get; } = new();

        private DateTime? _fechaDesde = DateTime.Today.AddMonths(-1);
        public DateTime? FechaDesde
        {
            get => _fechaDesde;
            set => SetProperty(ref _fechaDesde, value);
        }

        private DateTime? _fechaHasta = DateTime.Today;
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

        public ICommand GenerarReporteCommand { get; }
        public ICommand ExportarPdfCommand { get; }

        public ReporteOperativoViewModel(ObtenerReporteOrdenesHandler obtenerReportesHandler, IReporteGerencialRepository reporteGerencialRepo)
        {
            _obtenerReportesHandler = obtenerReportesHandler;
            _reporteGerencialRepo = reporteGerencialRepo;
            GenerarReporteCommand = new RelayCommand(LoadAsync);
            ExportarPdfCommand = new RelayCommand(ExportarPdfAsync);
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                Productividad.Clear();
                VehiculosFrecuentes.Clear();

                // 1. Obtener productividad
                var registros = await _obtenerReportesHandler.HandleAsync();
                var query = registros.AsEnumerable();
                
                if (FechaDesde.HasValue) query = query.Where(r => r.FechaIngreso >= FechaDesde.Value.Date);
                if (FechaHasta.HasValue) query = query.Where(r => r.FechaIngreso.Date <= FechaHasta.Value.Date);

                var productividades = query
                    .Where(r => !string.IsNullOrEmpty(r.MecanicoNombre) && r.MecanicoNombre != "N/A")
                    .GroupBy(d => d.MecanicoNombre)
                    .Select(g => new ProductividadMecanicoDto
                    {
                        MecanicoId = 0,
                        MecanicoNombre = g.Key,
                        TareasCompletadas = g.Count(d => d.Estado == "Finalizada"),
                        TareasAsignadas = g.Count()
                    })
                    .OrderByDescending(p => p.TareasCompletadas)
                    .ToList();

                foreach (var p in productividades) Productividad.Add(p);

                // 2. Obtener vehiculos frecuentes (Reusando SP de Gerencial)
                var vehiculos = await _reporteGerencialRepo.ObtenerModelosMasReparadosAsync(FechaDesde, FechaHasta, 10);
                foreach (var v in vehiculos) VehiculosFrecuentes.Add(v);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExportarPdfAsync()
        {
            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ReporteOperativo_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
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
                System.Diagnostics.Debug.WriteLine($"Error al abrir PDF: {ex.Message}");
            }
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Reporte Operativo de Taller").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    var filtrosStr = "";
                    if (FechaDesde.HasValue) filtrosStr += $"Desde: {FechaDesde.Value:dd/MM/yyyy} ";
                    if (FechaHasta.HasValue) filtrosStr += $"Hasta: {FechaHasta.Value:dd/MM/yyyy}";
                    column.Item().Text(filtrosStr).FontSize(11).FontColor(Colors.Grey.Darken2);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column => 
            {
                column.Spacing(20);
                
                // Productividad
                column.Item().Text("Productividad por Mecánico").FontSize(14).SemiBold();
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Mecánico");
                        header.Cell().Element(CellStyle).Text("Tareas Asignadas");
                        header.Cell().Element(CellStyle).Text("Tareas Completadas");
                        static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    foreach (var p in Productividad)
                    {
                        table.Cell().Element(Block).Text(p.MecanicoNombre);
                        table.Cell().Element(Block).Text(p.TareasAsignadas.ToString());
                        table.Cell().Element(Block).Text(p.TareasCompletadas.ToString());
                        static IContainer Block(IContainer container) => container.PaddingVertical(2).PaddingHorizontal(2);
                    }
                });

                // Vehículos Frecuentes
                column.Item().Text("Vehículos Más Frecuentes (Top 10)").FontSize(14).SemiBold();
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Marca");
                        header.Cell().Element(CellStyle).Text("Modelo");
                        header.Cell().Element(CellStyle).Text("Reparaciones");
                        static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    foreach (var v in VehiculosFrecuentes)
                    {
                        table.Cell().Element(Block).Text(v.Marca);
                        table.Cell().Element(Block).Text(v.Modelo);
                        table.Cell().Element(Block).Text(v.CantidadReparaciones.ToString());
                        static IContainer Block(IContainer container) => container.PaddingVertical(2).PaddingHorizontal(2);
                    }
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        }
    }

    public class ProductividadMecanicoDto
    {
        public int MecanicoId { get; set; }
        public string MecanicoNombre { get; set; } = string.Empty;
        public int TareasAsignadas { get; set; }
        public int TareasCompletadas { get; set; }
    }
}