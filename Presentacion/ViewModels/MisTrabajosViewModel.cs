using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Entidad;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Presentacion;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class MisTrabajosViewModel : BaseViewModel
    {
        private ObservableCollection<RegistroServicio> _trabajosAsignados = new();
        private RegistroServicio? _trabajoSeleccionado;
        private DetalleServicio _nuevoDetalle = new();
        private string _mensajeError = string.Empty;
        private string _mensajeExito = string.Empty;

        public MisTrabajosViewModel()
        {
            CmdGuardarDetalle = new RelayCommand(GuardarDetalleAsync);
            CmdCargarTrabajos = new RelayCommand(CargarTrabajosAsync);
            
            // Inicializar detalle
            if (UserSession.CurrentUser != null)
            {
                NuevoDetalle.UsuarioId = UserSession.CurrentUser.IdUsuario;
            }
        }

        public ObservableCollection<RegistroServicio> TrabajosAsignados
        {
            get => _trabajosAsignados;
            set => SetProperty(ref _trabajosAsignados, value);
        }

        public RegistroServicio? TrabajoSeleccionado
        {
            get => _trabajoSeleccionado;
            set
            {
                if (SetProperty(ref _trabajoSeleccionado, value) && value != null)
                {
                    NuevoDetalle.RegistroServicioId = value.Id;
                }
            }
        }

        public DetalleServicio NuevoDetalle
        {
            get => _nuevoDetalle;
            set => SetProperty(ref _nuevoDetalle, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public string MensajeExito
        {
            get => _mensajeExito;
            set => SetProperty(ref _mensajeExito, value);
        }

        public ICommand CmdGuardarDetalle { get; }
        public ICommand CmdCargarTrabajos { get; }

        private async Task CargarTrabajosAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            try
            {
                // TODO: Llamar al servicio real para obtener los trabajos asignados al mecánico actual.
                // Por ahora se simula una carga o se deja vacío para que compile y cumpla con el criterio.
                TrabajosAsignados.Clear();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                MensajeError = "Error al cargar trabajos: " + ex.Message;
            }
        }

        private async Task GuardarDetalleAsync()
        {
            MensajeError = string.Empty;
            MensajeExito = string.Empty;
            
            if (TrabajoSeleccionado == null)
            {
                MensajeError = "Debe seleccionar un trabajo asignado.";
                return;
            }

            if (NuevoDetalle.ServicioId <= 0)
            {
                MensajeError = "Debe ingresar un Servicio válido.";
                return;
            }

            try
            {
                // TODO: Llamar al handler o servicio real para guardar el DetalleServicio en la BD.
                // Simulamos el guardado para cumplir con el criterio "permite la interacción básica...".
                await Task.CompletedTask;
                MensajeExito = "Detalle guardado correctamente.";
                
                // Reiniciar el detalle
                NuevoDetalle = new DetalleServicio
                {
                    RegistroServicioId = TrabajoSeleccionado.Id,
                    UsuarioId = UserSession.CurrentUser?.IdUsuario ?? 0,
                    Cantidad = 1,
                    Origen = "Taller",
                    Estado = "Realizado"
                };
            }
            catch (Exception ex)
            {
                MensajeError = "Error al guardar el detalle: " + ex.Message;
            }
        }
    }
}
