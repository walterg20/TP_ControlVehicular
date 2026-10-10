using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;
using TP_ControlVehicular.Negocio.Reportes.Documentos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Fila de detalle: una tarea (DetalleServicio) asignada al mecanico dentro de una orden.
    /// Comparte la misma instancia de DetalleServicioDto que la orden, de modo que al guardar
    /// se persisten los cambios de todas las ordenes juntas.
    /// </summary>
    public class MiTareaItemViewModel : BaseViewModel
    {
        public RegistroServicioDto Orden { get; set; } = new RegistroServicioDto();
        public DetalleServicioDto Detalle { get; set; } = new DetalleServicioDto();

        /// <summary>Orden maestra a la que pertenece la tarea (para refrescar el progreso).</summary>
        public MiOrdenItemViewModel? OrdenPadre { get; set; }

        public int OrdenId => Orden.Id;
        public DateTime Fecha => Orden.Fecha;
        public string VehiculoPatente => Orden.VehiculoPatente;
        public string VehiculoDetalle => Orden.VehiculoDetalle;
        public string VehiculoCompleto => string.IsNullOrWhiteSpace(VehiculoDetalle) ? VehiculoPatente : $"{VehiculoDetalle} [{VehiculoPatente}]";
        public string TareaNombre => Detalle.ServicioNombre;

        public bool Realizado
        {
            get => Detalle.Realizado;
            set
            {
                if (Detalle.Realizado != value)
                {
                    Detalle.Realizado = value;
                    OnPropertyChanged();
                    IsModified = true;
                    OrdenPadre?.RefrescarProgreso();
                }
            }
        }

        public string Observaciones
        {
            get => Detalle.Observaciones;
            set
            {
                if (Detalle.Observaciones != value)
                {
                    Detalle.Observaciones = value;
                    OnPropertyChanged();
                    IsModified = true;
                }
            }
        }

        public bool IsModified { get; set; } = false;
    }

    /// <summary>
    /// Fila maestra: una orden de servicio con las tareas del mecanico logueado.
    /// </summary>
    public class MiOrdenItemViewModel : BaseViewModel
    {
        public RegistroServicioDto Orden { get; set; } = new RegistroServicioDto();
        public ObservableCollection<MiTareaItemViewModel> MisTareas { get; } = new ObservableCollection<MiTareaItemViewModel>();

        public int OrdenId => Orden.Id;
        public DateTime Fecha => Orden.Fecha;
        public string VehiculoPatente => Orden.VehiculoPatente;
        public string VehiculoDetalle => Orden.VehiculoDetalle;
        public string VehiculoCompleto => string.IsNullOrWhiteSpace(VehiculoDetalle) ? VehiculoPatente : $"{VehiculoDetalle} [{VehiculoPatente}]";
        public string ClienteDetalle => Orden.ClienteDetalle;
        public int KmIngreso => Orden.KmIngreso;

        public int TotalTareas => MisTareas.Count;
        public int TareasRealizadas => MisTareas.Count(t => t.Realizado);
        public int TareasPendientes => TotalTareas - TareasRealizadas;
        public bool TienePendientes => TareasPendientes > 0;
        public string Progreso => $"{TareasRealizadas}/{TotalTareas}";

        /// <summary>Notifica los cambios de progreso tras marcar/desmarcar una tarea.</summary>
        public void RefrescarProgreso()
        {
            OnPropertyChanged(nameof(TareasRealizadas));
            OnPropertyChanged(nameof(TareasPendientes));
            OnPropertyChanged(nameof(TienePendientes));
            OnPropertyChanged(nameof(Progreso));
        }
    }

    public class MisTrabajosViewModel : BaseViewModel
    {
        private readonly ListarRegistroServiciosHandler _listarHandler;
        private readonly ModificarRegistroServicioHandler _modificarHandler;
        private readonly ObtenerHistorialClinicoVehiculoHandler _historialHandler;
        private readonly ObtenerHojaTrabajoDiariaHandler _hojaTrabajoHandler;

        /// <summary>Ordenes (fila maestra). Cada orden agrupa las tareas del mecanico logueado.</summary>
        public ObservableCollection<MiOrdenItemViewModel> Ordenes { get; set; }

        /// <summary>Vista filtrada de ordenes (busqueda + estado).</summary>
        public ICollectionView OrdenesView { get; private set; }

        /// <summary>Detalle de la orden seleccionada (tareas del mecanico logueado).</summary>
        public ObservableCollection<MiTareaItemViewModel> TareasDeOrden { get; set; }

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

            Ordenes = new ObservableCollection<MiOrdenItemViewModel>();
            TareasDeOrden = new ObservableCollection<MiTareaItemViewModel>();
            OrdenesView = CollectionViewSource.GetDefaultView(Ordenes);
            OrdenesView.Filter = OrdenesFilter;
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                OrdenesView.Refresh();
                OnPropertyChanged(nameof(IsListEmpty));
            }
        }

        private string _filtroEstado = "Pendientes";
        public string FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                _filtroEstado = value;
                OnPropertyChanged();
                OrdenesView.Refresh();
                OnPropertyChanged(nameof(IsListEmpty));
            }
        }

        public bool IsListEmpty => OrdenesView.IsEmpty;

        private MiOrdenItemViewModel? _ordenSeleccionada;
        public MiOrdenItemViewModel? OrdenSeleccionada
        {
            get => _ordenSeleccionada;
            set
            {
                if (SetProperty(ref _ordenSeleccionada, value))
                {
                    ReconstruirTareas();
                    OnPropertyChanged(nameof(HayOrdenSeleccionada));
                    OnPropertyChanged(nameof(NoHayOrdenSeleccionada));
                    OnPropertyChanged(nameof(PuedeImprimirHistorial));
                }
            }
        }

        public bool HayOrdenSeleccionada => OrdenSeleccionada != null;
        public bool NoHayOrdenSeleccionada => OrdenSeleccionada == null;

        private bool OrdenesFilter(object item)
        {
            if (item is MiOrdenItemViewModel orden)
            {
                // Filtro por estado de las tareas del mecanico
                if (FiltroEstado == "Pendientes" && !orden.TienePendientes) return false;
                if (FiltroEstado == "Finalizados" && orden.TienePendientes) return false;

                // Filtro de busqueda: Nº de orden, patente o cliente
                if (!string.IsNullOrWhiteSpace(TextoBusqueda))
                {
                    bool match = (orden.OrdenId.ToString().IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 (orden.VehiculoPatente?.IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 (orden.VehiculoDetalle?.IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 (orden.ClienteDetalle?.IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!match) return false;
                }

                return true;
            }
            return false;
        }

        private void ReconstruirTareas()
        {
            TareasDeOrden.Clear();
            if (OrdenSeleccionada != null)
            {
                foreach (var tarea in OrdenSeleccionada.MisTareas)
                {
                    TareasDeOrden.Add(tarea);
                }
            }
        }

        public async Task LoadAsync()
        {
            if (UserSession.CurrentUser == null) return;

            int? idPrevio = OrdenSeleccionada?.OrdenId;

            Ordenes.Clear();
            var ordenes = await _listarHandler.HandleAsync();
            int currentUserId = UserSession.CurrentUser.IdUsuario;

            foreach (var orden in ordenes.Where(o => o.Estado != "Cancelada").OrderByDescending(o => o.Id))
            {
                if (orden.Detalles == null) continue;

                var misDetalles = orden.Detalles.Where(d => d.UsuarioId == currentUserId).ToList();
                if (misDetalles.Count == 0) continue;

                var master = new MiOrdenItemViewModel { Orden = orden };
                foreach (var det in misDetalles)
                {
                    master.MisTareas.Add(new MiTareaItemViewModel
                    {
                        Orden = orden,
                        Detalle = det,
                        OrdenPadre = master,
                        IsModified = false
                    });
                }
                Ordenes.Add(master);
            }

            OrdenesView.Refresh();
            OnPropertyChanged(nameof(IsListEmpty));

            // Preservar la seleccion previa si la orden sigue visible; si no, seleccionar la primera.
            var visibles = OrdenesView.Cast<MiOrdenItemViewModel>().ToList();
            OrdenSeleccionada = visibles.FirstOrDefault(o => o.OrdenId == idPrevio) ?? visibles.FirstOrDefault();
        }

        public async Task<bool> GuardarCambiosAsync()
        {
            var modifiedItems = Ordenes.SelectMany(o => o.MisTareas).Where(t => t.IsModified).ToList();
            if (!modifiedItems.Any()) return true;

            var modifiedOrders = modifiedItems.Select(m => m.Orden).Distinct().ToList();

            foreach (var ordenDto in modifiedOrders)
            {
                // Evaluate general status (informative, but ModificarRegistroServicioHandler will override it intelligently)
                if (ordenDto.Detalles.All(d => d.Realizado))
                {
                    ordenDto.Estado = "Completada";
                }
                else if (ordenDto.Detalles.Any(d => d.Realizado))
                {
                    ordenDto.Estado = "En Proceso";
                }
                else
                {
                    ordenDto.Estado = "Abierta";
                }

                // Map back to Entity
                var orden = new TP_ControlVehicular.Entidad.RegistroServicio
                {
                    Id = ordenDto.Id,
                    VehiculoId = ordenDto.VehiculoId,
                    TallerId = ordenDto.TallerId,
                    UsuarioId = ordenDto.UsuarioId,
                    Fecha = ordenDto.Fecha,
                    KmIngreso = ordenDto.KmIngreso,
                    Estado = ordenDto.Estado,
                    Detalles = ordenDto.Detalles.Select(d => new TP_ControlVehicular.Entidad.DetalleServicio
                    {
                        Id = d.Id,
                        ServicioId = d.ServicioId,
                        RegistroServicioId = d.RegistroServicioId,
                        UsuarioId = d.UsuarioId,
                        Cantidad = d.Cantidad,
                        Precio = d.Precio,
                        Observaciones = d.Observaciones ?? string.Empty,
                        Origen = d.Origen,
                        Estado = d.Realizado ? "Finalizada" : (d.Estado == "Finalizada" ? "Pendiente" : d.Estado),
                        OrdenEjecucion = d.OrdenEjecucion
                    }).ToList()
                };

                await _modificarHandler.HandleAsync(orden);
            }

            foreach (var item in modifiedItems)
            {
                item.IsModified = false;
            }

            await LoadAsync();

            return true;
        }

        /// <summary>Habilita los reportes del mecanico segun el permiso Reporte.Mecanico.Ver.</summary>
        public bool PuedeVerReportesMecanico => TienePermiso("Reporte.Mecanico.Ver");

        /// <summary>Historial clinico: requiere el permiso y una orden seleccionada.</summary>
        public bool PuedeImprimirHistorial => PuedeVerReportesMecanico && OrdenSeleccionada != null;

        private ICommand? _imprimirHistorialCommand;
        public ICommand ImprimirHistorialCommand => _imprimirHistorialCommand ??= new TP_ControlVehicular.Presentacion.RelayCommand(async () => await GenerarHistorialPdfAsync());

        private ICommand? _imprimirHojaTrabajoCommand;
        public ICommand ImprimirHojaTrabajoCommand => _imprimirHojaTrabajoCommand ??= new TP_ControlVehicular.Presentacion.RelayCommand(async () => await GenerarHojaTrabajoPdfAsync());

        private async Task GenerarHistorialPdfAsync()
        {
            if (OrdenSeleccionada == null) return;
            try
            {
                var historial = await _historialHandler.HandleAsync(OrdenSeleccionada.Orden.VehiculoId);
                var vehiculoDesc = OrdenSeleccionada.VehiculoCompleto;
                var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Historial_{OrdenSeleccionada.VehiculoPatente}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(QuestPDF.Helpers.PageSizes.A4);
                        page.Margin(1, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.PageColor(QuestPDF.Helpers.Colors.White);
                        page.Header().Element(c => c.ComposeEncabezadoTaller("Historial Clínico"));
                        page.Footer().Element(c => c.ComposePieDePagina());
                        page.Content().PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(col =>
                        {
                            col.Item().Text($"Vehículo: {vehiculoDesc}").FontSize(12).SemiBold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            col.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80);
                                    columns.ConstantColumn(80);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
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
                        page.Header().Element(c => c.ComposeEncabezadoTaller("Tareas del Día"));
                        page.Footer().Element(c => c.ComposePieDePagina());
                        page.Content().PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre).Column(col =>
                        {
                            col.Item().Text($"Mecánico: {usuario.Nombre} - {DateTime.Today:dd/MM/yyyy}").FontSize(12).SemiBold().FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            col.Item().PaddingTop(8).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(80);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(80);
                                    columns.RelativeColumn();
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
    }
}
