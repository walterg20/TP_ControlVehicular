import io
import re

with io.open('Presentacion/ViewModels/MisTrabajosViewModel.cs', 'r', encoding='utf-8') as f:
    content = f.read()

# Dependencies
deps = '''
        private readonly ListarRegistroServiciosHandler _listarHandler;
        private readonly ModificarRegistroServicioHandler _modificarHandler;
        private readonly ObtenerHistorialClinicoVehiculoHandler _historialHandler;
        private readonly ObtenerHojaTrabajoDiariaHandler _hojaTrabajoHandler;
'''
content = re.sub(r'private readonly ListarRegistroServiciosHandler.*?_modificarHandler;', deps.strip(), content, flags=re.DOTALL)

# Constructor
ctor = '''
        public MisTrabajosViewModel(
            ListarRegistroServiciosHandler listarHandler, 
            ModificarRegistroServicioHandler modificarHandler,
            ObtenerHistorialClinicoVehiculoHandler historialHandler,
            ObtenerHojaTrabajoDiariaHandler hojaTrabajoHandler)
        {
            _listarHandler = listarHandler;
            _modificarHandler = modificarHandler;
            _historialHandler = historialHandler;
            _hojaTrabajoHandler = hojaTrabajoHandler;
'''
content = re.sub(r'public MisTrabajosViewModel\(ListarRegistroServiciosHandler listarHandler, ModificarRegistroServicioHandler modificarHandler\)\s*\{[\s\S]*?_modificarHandler = modificarHandler;', ctor.strip(), content)

# TareaSeleccionada & PDF logic
pdf_logic = '''
        private MiTrabajoItemViewModel? _tareaSeleccionada;
        public MiTrabajoItemViewModel? TareaSeleccionada
        {
            get => _tareaSeleccionada;
            set
            {
                _tareaSeleccionada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuedeImprimirHistorial));
            }
        }

        public bool PuedeImprimirHistorial => TareaSeleccionada != null;

        private ICommand? _imprimirHistorialCommand;
        public ICommand ImprimirHistorialCommand => _imprimirHistorialCommand ??= new TP_ControlVehicular.Presentacion.RelayCommand(async () => await GenerarHistorialPdfAsync());

        private ICommand? _imprimirHojaTrabajoCommand;
        public ICommand ImprimirHojaTrabajoCommand => _imprimirHojaTrabajoCommand ??= new TP_ControlVehicular.Presentacion.RelayCommand(async () => await GenerarHojaTrabajoPdfAsync());

        private async Task GenerarHistorialPdfAsync()
        {
            if (TareaSeleccionada == null) return;
            try
            {
                var historial = await _historialHandler.HandleAsync(TareaSeleccionada.Orden.VehiculoId);
                var vehiculoDesc = TareaSeleccionada.VehiculoCompleto;
                
                var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Historial_{TareaSeleccionada.VehiculoPatente}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4);
                        page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.PageColor(QuestPDF.Helpers.Colors.White);

                        page.Header().Text($"Historial Clínico - {vehiculoDesc}")
                            .FontSize(20).SemiBold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                        
                        page.Content().PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(col =>
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80); // Fecha
                                    columns.ConstantColumn(80); // Km
                                    columns.RelativeColumn();   // Servicio
                                    columns.RelativeColumn();   // Observaciones
                                });
                                
                                table.Header(header =>
                                {
                                    header.Cell().Text("Fecha").SemiBold();
                                    header.Cell().Text("Km");
                                    header.Cell().Text("Servicio");
                                    header.Cell().Text("Observaciones");
                                });

                                foreach (var h in historial)
                                {
                                    table.Cell().Text(h.Fecha.ToString("dd/MM/yyyy"));
                                    table.Cell().Text(h.KmIngreso.ToString());
                                    table.Cell().Text(h.Servicio);
                                    table.Cell().Text(h.Observaciones);
                                }
                            });
                        });
                    });
                });
                
                await Task.Run(() => document.GeneratePdf(filePath));
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al generar PDF: " + ex.Message);
            }
        }

        private async Task GenerarHojaTrabajoPdfAsync()
        {
            try
            {
                var usuario = (System.Windows.Application.Current.MainWindow as MainWindow)?.UsuarioSesionActual;
                if (usuario == null) return;

                var tareas = await _hojaTrabajoHandler.HandleAsync(usuario.IdUsuario, DateTime.Today);
                
                var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"TareasDia_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4.Landscape());
                        page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.PageColor(QuestPDF.Helpers.Colors.White);

                        page.Header().Text($"Tareas del Día - Mecánico: {usuario.Nombre} - {DateTime.Today:dd/MM/yyyy}")
                            .FontSize(20).SemiBold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken2);
                        
                        page.Content().PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(col =>
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80); // Patente
                                    columns.RelativeColumn();   // Vehiculo
                                    columns.RelativeColumn();   // Tarea
                                    columns.ConstantColumn(80); // Estado
                                    columns.RelativeColumn();   // Obs
                                });
                                
                                table.Header(header =>
                                {
                                    header.Cell().Text("Patente").SemiBold();
                                    header.Cell().Text("Vehículo");
                                    header.Cell().Text("Servicio a Realizar");
                                    header.Cell().Text("Estado");
                                    header.Cell().Text("Obs");
                                });

                                foreach (var t in tareas)
                                {
                                    table.Cell().Text(t.Patente);
                                    table.Cell().Text(t.Vehiculo);
                                    table.Cell().Text(t.Servicio);
                                    table.Cell().Text(t.Estado);
                                    table.Cell().Text(t.Observaciones);
                                }
                            });
                        });
                    });
                });
                
                await Task.Run(() => document.GeneratePdf(filePath));
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al generar PDF: " + ex.Message);
            }
        }
'''
content = content.replace('public event EventHandler<string>? RegistrationFailed;', pdf_logic + '\n        public event EventHandler<string>? RegistrationFailed;')

with io.open('Presentacion/ViewModels/MisTrabajosViewModel.cs', 'w', encoding='utf-8') as f:
    f.write(content)