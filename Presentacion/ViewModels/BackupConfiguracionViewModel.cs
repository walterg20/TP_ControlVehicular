// walterg20
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class BackupConfiguracionViewModel : BaseViewModel
    {
        private readonly IBackupConfiguracionService _configuracionService;
        private readonly IDriveBackupService _driveBackupService;

        private BackupConfiguracionDto _dto = new();

        public BackupConfiguracionViewModel(
            IBackupConfiguracionService configuracionService,
            IDriveBackupService driveBackupService)
        {
            _configuracionService = configuracionService;
            _driveBackupService = driveBackupService;

            GuardarCommand = new RelayCommand(async () => await GuardarAsync(), PuedeGuardar);
            ConectarDriveCommand = new RelayCommand(async () => await ConectarDriveAsync(), PuedeConectar);
        }

        protected override string Modulo => "Configuracion.Backup";

        public ICommand GuardarCommand { get; }
        public ICommand ConectarDriveCommand { get; }

        public string NombreBaseDatos
        {
            get => _dto.NombreBaseDatos;
            set
            {
                if (_dto.NombreBaseDatos == value) return;
                _dto.NombreBaseDatos = value;
                OnPropertyChanged();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
                ValidateProperty();
            }
        }

        public string RutaBackupServer
        {
            get => _dto.RutaBackupServer;
            set
            {
                if (_dto.RutaBackupServer == value) return;
                _dto.RutaBackupServer = value;
                OnPropertyChanged();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
                ValidateProperty();
            }
        }

        public string RutaBackupUNC
        {
            get => _dto.RutaBackupUNC;
            set
            {
                if (_dto.RutaBackupUNC == value) return;
                _dto.RutaBackupUNC = value;
                OnPropertyChanged();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
                ValidateProperty();
            }
        }

        public string? DriveFolderId
        {
            get => _dto.DriveFolderId;
            set
            {
                if (_dto.DriveFolderId == value) return;
                _dto.DriveFolderId = value;
                OnPropertyChanged();
            }
        }

        public bool TieneCredencialesDrive => _driveBackupService.TieneCredencialesGuardadas();

        public string EstadoDrive => TieneCredencialesDrive
            ? "✅ Conectado con Google Drive en esta PC."
            : "⚠️ Sin conexión con Google Drive en esta PC.";

        public string EstadoUltimaSubida => _dto.FechaUltimaSubida.HasValue
            ? $"Último archivo resguardado: {_dto.UltimaSubidaArchivo} ({_dto.FechaUltimaSubida:dd/MM/yyyy HH:mm})"
            : "Todavía no se resguardó ningún respaldo en Google Drive.";

        public bool PuedeGuardar() => !HasErrors && IsNotLoading;

        private bool PuedeConectar() => IsNotLoading;

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(NombreBaseDatos):
                    if (string.IsNullOrWhiteSpace(NombreBaseDatos))
                        SetError(nameof(NombreBaseDatos), "El nombre de la base de datos es obligatorio.");
                    break;
                case nameof(RutaBackupServer):
                    if (string.IsNullOrWhiteSpace(RutaBackupServer))
                        SetError(nameof(RutaBackupServer), "La ruta de backup en el servidor es obligatoria.");
                    break;
                case nameof(RutaBackupUNC):
                    if (string.IsNullOrWhiteSpace(RutaBackupUNC))
                        SetError(nameof(RutaBackupUNC), "La ruta compartida (UNC) es obligatoria.");
                    break;
            }

            ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(NombreBaseDatos));
            ValidateProperty(nameof(RutaBackupServer));
            ValidateProperty(nameof(RutaBackupUNC));
            ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            return !HasErrors;
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                _dto = await _configuracionService.ObtenerAsync();
                OnPropertyChanged(nameof(NombreBaseDatos));
                OnPropertyChanged(nameof(RutaBackupServer));
                OnPropertyChanged(nameof(RutaBackupUNC));
                OnPropertyChanged(nameof(DriveFolderId));
                ClearAllErrors();
                NotificarEstadoDrive();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task GuardarAsync()
        {
            if (!ValidateAll()) return;

            var owner = Owner;
            if (owner != null && !TienePermiso(Modulo + ".Gestionar"))
            {
                FrmConfirmacion.MostrarAviso("No tiene permiso para gestionar la configuración de respaldos.", "Sin permiso", owner);
                return;
            }

            IsLoading = true;
            try
            {
                _dto = await _configuracionService.GuardarAsync(_dto);
                NotificarEstadoDrive();
                if (owner != null)
                {
                    FrmConfirmacion.MostrarAviso("La configuración de respaldos fue guardada correctamente.", "Guardado", owner, icono: "✅");
                }
            }
            catch (Exception ex)
            {
                if (owner != null)
                {
                    FrmConfirmacion.MostrarAviso($"No se pudo guardar la configuración: {ex.Message}", "Error", owner);
                }
            }
            finally
            {
                IsLoading = false;
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            }
        }

        public async Task ConectarDriveAsync()
        {
            var owner = Owner;
            if (owner == null) return;

            if (!TienePermiso(Modulo + ".Gestionar"))
            {
                FrmConfirmacion.MostrarAviso("No tiene permiso para gestionar los respaldos.", "Sin permiso", owner);
                return;
            }

            IsLoading = true;
            try
            {
                var autenticado = await _driveBackupService.AutenticarAsync(owner);
                NotificarEstadoDrive();

                if (autenticado)
                {
                    FrmConfirmacion.MostrarAviso("Conexión con Google Drive establecida correctamente.", "Drive", owner, icono: "✅");
                }
                else
                {
                    FrmConfirmacion.MostrarAviso("No se pudo conectar con Google Drive.", "Drive", owner, icono: "❌");
                }
            }
            catch (Exception ex)
            {
                FrmConfirmacion.MostrarAviso($"Error al conectar con Google Drive: {ex.Message}", "Error", owner);
            }
            finally
            {
                IsLoading = false;
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            }
        }

        private void NotificarEstadoDrive()
        {
            OnPropertyChanged(nameof(TieneCredencialesDrive));
            OnPropertyChanged(nameof(EstadoDrive));
            OnPropertyChanged(nameof(EstadoUltimaSubida));
        }

        private static Window? Owner => Application.Current?.MainWindow;
    }
}
