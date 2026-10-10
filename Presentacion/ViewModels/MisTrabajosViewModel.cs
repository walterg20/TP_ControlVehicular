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

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class MiTrabajoItemViewModel : BaseViewModel
    {
        public RegistroServicioDto Orden { get; set; } = new RegistroServicioDto();
        public DetalleServicioDto Detalle { get; set; } = new DetalleServicioDto();

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

    public class MisTrabajosViewModel : BaseViewModel
    {
        private readonly ListarRegistroServiciosHandler _listarHandler;
        private readonly ModificarRegistroServicioHandler _modificarHandler;

        public ObservableCollection<MiTrabajoItemViewModel> TodasMisTareas { get; set; }
        public ObservableCollection<string> FiltroVehiculos { get; set; }

        private string _vehiculoSeleccionado = "Todos los Autos";
        public string VehiculoSeleccionado
        {
            get => _vehiculoSeleccionado;
            set
            {
                _vehiculoSeleccionado = value;
                OnPropertyChanged();
                TareasView.Refresh();
                OnPropertyChanged(nameof(IsListEmpty));
            }
        }

        public ICollectionView TareasView { get; private set; }

        public MisTrabajosViewModel(ListarRegistroServiciosHandler listarHandler, ModificarRegistroServicioHandler modificarHandler)
        {
            _listarHandler = listarHandler;
            _modificarHandler = modificarHandler;

            TodasMisTareas = new ObservableCollection<MiTrabajoItemViewModel>();
            FiltroVehiculos = new ObservableCollection<string> { "Todos los Autos" };
            TareasView = CollectionViewSource.GetDefaultView(TodasMisTareas);
            TareasView.Filter = TareasFilter;
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                TareasView.Refresh();
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
                TareasView.Refresh();
                OnPropertyChanged(nameof(IsListEmpty));
            }
        }

        public bool IsListEmpty => TareasView.IsEmpty;

        private bool TareasFilter(object item)
        {
            if (item is MiTrabajoItemViewModel tarea)
            {
                // Status Filter
                if (FiltroEstado == "Pendientes" && tarea.Realizado) return false;
                if (FiltroEstado == "Finalizados" && !tarea.Realizado) return false;

                // Vehicle Filter
                if (VehiculoSeleccionado != "Todos los Autos" && tarea.VehiculoCompleto != VehiculoSeleccionado) return false;

                // Search Filter
                if (!string.IsNullOrWhiteSpace(TextoBusqueda))
                {
                    bool match = (tarea.VehiculoPatente?.IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 (tarea.OrdenId.ToString().IndexOf(TextoBusqueda, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!match) return false;
                }

                return true;
            }
            return false;
        }

        public async Task LoadAsync()
        {
            if (UserSession.CurrentUser == null) return;

            TodasMisTareas.Clear();
            var ordenes = await _listarHandler.HandleAsync();

            int currentUserId = UserSession.CurrentUser.IdUsuario;

            foreach (var orden in ordenes.Where(o => o.Estado != "Cancelada"))
            {
                if (orden.Detalles != null)
                {
                    foreach (var det in orden.Detalles.Where(d => d.UsuarioId == currentUserId))
                    {
                        TodasMisTareas.Add(new MiTrabajoItemViewModel
                        {
                            Orden = orden,
                            Detalle = det,
                            IsModified = false
                        });
                    }
                }
            }

            
            var patentes = TodasMisTareas.Select(t => t.VehiculoCompleto).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            FiltroVehiculos.Clear();
            FiltroVehiculos.Add("Todos los Autos");
            foreach (var patente in patentes)
            {
                FiltroVehiculos.Add(patente);
            }
            if (!FiltroVehiculos.Contains(VehiculoSeleccionado))
            {
                VehiculoSeleccionado = "Todos los Autos";
            }

            TareasView.Refresh();
            OnPropertyChanged(nameof(IsListEmpty));
        }

        public async Task<bool> GuardarCambiosAsync()
        {
            var modifiedItems = TodasMisTareas.Where(t => t.IsModified).ToList();
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
    }
}
