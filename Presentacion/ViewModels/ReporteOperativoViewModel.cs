using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs.Reportes;
using TP_ControlVehicular.Negocio.Reportes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TP_ControlVehicular.Negocio.Reportes.Documentos;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class ReporteOperativoViewModel : BaseViewModel
    {
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

        // Totales del reporte de productividad
        private int _totalTareas;
        public int TotalTareas
        {
            get => _totalTareas;
            private set
            {
                if (SetProperty(ref _totalTareas, value)) NotificarPorcentajeGeneral();
            }
        }

        private int _totalPendientes;
        public int TotalPendientes
        {
            get => _totalPendientes;
            private set => SetProperty(ref _totalPendientes, value);
        }

        private int _totalEnCurso;
        public int TotalEnCurso
        {
            get => _totalEnCurso;
            private set => SetProperty(ref _totalEnCurso, value);
        }

        private int _totalTareasCompletadas;
        public int TotalTareasCompletadas
        {
            get => _totalTareasCompletadas;
            private set
            {
                if (SetProperty(ref _totalTareasCompletadas, value)) NotificarPorcentajeGeneral();
            }
        }

        public double PorcentajeAvanceGeneral => TotalTareas == 0
            ? 0
            : Math.Round(TotalTareasCompletadas * 100.0 / TotalTareas, 0);

        public string PorcentajeAvanceGeneralTexto => $"{PorcentajeAvanceGeneral:0}%";

        private void NotificarPorcentajeGeneral()
        {
            OnPropertyChanged(nameof(PorcentajeAvanceGeneral));
            OnPropertyChanged(nameof(PorcentajeAvanceGeneralTexto));
        }

        public ICommand GenerarReporteCommand { get; }
        public ICommand ExportarPdfCommand { get; }

        public ReporteOperativoViewModel(IReporteGerencialRepository reporteGerencialRepo)
        {
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

                // 1. Productividad por mecánico, agregada a nivel de tarea (DetalleServicio)
                var productividades = (await _reporteGerencialRepo.ObtenerProductividadMecanicosAsync(FechaDesde, FechaHasta)).ToList();
                foreach (var p in productividades) Productividad.Add(p);
                ActualizarTotalesProductividad();

                // 2. Obtener vehiculos frecuentes (Reusando SP de Gerencial)
                var vehiculos = await _reporteGerencialRepo.ObtenerModelosMasReparadosAsync(FechaDesde, FechaHasta, 10);
                foreach (var v in vehiculos) VehiculosFrecuentes.Add(v);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ActualizarTotalesProductividad()
        {
            TotalTareas = Productividad.Sum(p => p.TareasAsignadas);
            TotalPendientes = Productividad.Sum(p => p.Pendientes);
            TotalEnCurso = Productividad.Sum(p => p.EnCurso);
            TotalTareasCompletadas = Productividad.Sum(p => p.TareasCompletadas);
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

                    page.Header().Element(c => c.ComposeEncabezadoTaller("Reporte Operativo de Taller"));
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
                System.Diagnostics.Debug.WriteLine($"Error al abrir PDF: {ex.Message}");
            }
        }

        private string ObtenerTextoFiltros()
        {
            var filtrosStr = "";
            if (FechaDesde.HasValue) filtrosStr += $"Desde: {FechaDesde.Value:dd/MM/yyyy} ";
            if (FechaHasta.HasValue) filtrosStr += $"Hasta: {FechaHasta.Value:dd/MM/yyyy}";
            return filtrosStr;
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(20);

                column.Item().Text(ObtenerTextoFiltros()).FontSize(11).FontColor(Colors.Grey.Darken2);

                // Productividad
                column.Item().Text("Productividad por Mecánico").FontSize(14).SemiBold();
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Mecánico");
                        header.Cell().Element(CellStyle).Text("Tareas Asignadas");
                        header.Cell().Element(CellStyle).Text("Pendientes");
                        header.Cell().Element(CellStyle).Text("En Curso");
                        header.Cell().Element(CellStyle).Text("Completadas");
                        header.Cell().Element(CellStyle).Text("% Avance");
                        static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    foreach (var p in Productividad)
                    {
                        table.Cell().Element(Block).Text(p.MecanicoNombre);
                        table.Cell().Element(Block).Text(p.TareasAsignadas.ToString());
                        table.Cell().Element(Block).Text(p.Pendientes.ToString());
                        table.Cell().Element(Block).Text(p.EnCurso.ToString());
                        table.Cell().Element(Block).Text(p.TareasCompletadas.ToString());
                        table.Cell().Element(Block).Text(p.PorcentajeAvanceTexto);
                        static IContainer Block(IContainer container) => container.PaddingVertical(2).PaddingHorizontal(2);
                    }

                    // Fila de totales
                    table.Cell().Element(TotalStyle).Text("Total");
                    table.Cell().Element(TotalStyle).Text(TotalTareas.ToString());
                    table.Cell().Element(TotalStyle).Text(TotalPendientes.ToString());
                    table.Cell().Element(TotalStyle).Text(TotalEnCurso.ToString());
                    table.Cell().Element(TotalStyle).Text(TotalTareasCompletadas.ToString());
                    table.Cell().Element(TotalStyle).Text(PorcentajeAvanceGeneralTexto);
                    static IContainer TotalStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(4).PaddingHorizontal(2).BorderTop(1).BorderColor(Colors.Black);
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
    }
}
