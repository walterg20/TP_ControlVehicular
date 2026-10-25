using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Services;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class MisTareasViewModel : BaseViewModel
    {
        private readonly ListarTareasPorEspecialistaHandler _listarHandler;
        private readonly AvanzarEstadoTareaHandler _avanzarHandler;

        private ObservableCollection<DetalleServicioDto> _tareasAsignadas = new();
        private DetalleServicioDto? _tareaSeleccionada;
        private string _mensajeError = string.Empty;
        private string _mensajeExito = string.Empty;
        private bool _isBusy;

        public MisTareasViewModel(ListarTareasPorEspecialistaHandler listarHandler, AvanzarEstadoTareaHandler avanzarHandler)
        {
            _listarHandler = listarHandler;
            _avanzarHandler = avanzarHandler;

            CmdCargarTareas = new RelayCommand(async () => await CargarTareasAsync());
            CmdFinalizarTarea = new RelayCommand(async () => await FinalizarTareaAsync(), () => TareaSeleccionada != null && TareaSeleccionada.Estado == "En Curso" && !IsBusy);
        }

        public ObservableCollection<DetalleServicioDto> TareasAsignadas
        {
            get => _tareasAsignadas;
            set => SetProperty(ref _tareasAsignadas, value);
        }

        public DetalleServicioDto? TareaSeleccionada
        {
            get => _tareaSeleccionada;
            set 
            {
                SetProperty(ref _tareaSeleccionada, value);
                // Trigger CanExecute change on the command
                CommandManager.InvalidateRequerySuggested();
            }
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

        public bool IsBusy
        {
            get => _isBusy;
            set 
            {
                SetProperty(ref _isBusy, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand CmdCargarTareas { get; }
        public ICommand CmdFinalizarTarea { get; }

        public async Task CargarTareasAsync()
        {
            if (UserSession.CurrentUser == null)
            {
                MensajeError = "No hay una sesión activa.";
                return;
            }

            IsBusy = true;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            try
            {
                int mecanicoId = UserSession.CurrentUser.IdUsuario;
                var tareas = await _listarHandler.HandleAsync(mecanicoId);
                
                TareasAsignadas.Clear();
                foreach (var tarea in tareas)
                {
                    TareasAsignadas.Add(tarea);
                }
            }
            catch (Exception ex)
            {
                MensajeError = "Error al cargar tareas: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FinalizarTareaAsync()
        {
            if (TareaSeleccionada == null)
            {
                MensajeError = "Debe seleccionar una tarea para finalizar.";
                return;
            }

            if (UserSession.CurrentUser == null)
            {
                MensajeError = "Sesión inválida.";
                return;
            }

            IsBusy = true;
            MensajeError = string.Empty;
            MensajeExito = string.Empty;

            try
            {
                await _avanzarHandler.HandleAsync(TareaSeleccionada.Id, UserSession.CurrentUser.IdUsuario);
                
                MensajeExito = $"Tarea {TareaSeleccionada.Id} finalizada exitosamente. La orden puede haber avanzado al siguiente estado.";
                
                // Recargar lista para reflejar el estado actual
                await CargarTareasAsync();
            }
            catch (Exception ex)
            {
                MensajeError = "Error al finalizar tarea: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
