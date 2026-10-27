// walterg20
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TP_ControlVehicular.Negocio.Context;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class EmpresaConfiguracionViewModel : BaseViewModel
    {
        private readonly IEmpresaConfiguracionService _empresaService;
        private EmpresaConfiguracionDto _dto = new();
        private byte[]? _logoBytes;
        private string? _logoPreview;

        public EmpresaConfiguracionViewModel(IEmpresaConfiguracionService empresaService)
        {
            _empresaService = empresaService;
            GuardarCommand = new RelayCommand(async () => await GuardarAsync(), PuedeGuardar);
        }

        protected override string Modulo => "Configuracion.Empresa";

        public ICommand GuardarCommand { get; }

        public string Nombre
        {
            get => _dto.Nombre;
            set
            {
                if (_dto.Nombre == value) return;
                _dto.Nombre = value;
                OnPropertyChanged();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
                ValidateProperty();
            }
        }

        public string Direccion
        {
            get => _dto.Direccion;
            set
            {
                if (_dto.Direccion == value) return;
                _dto.Direccion = value;
                OnPropertyChanged();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
                ValidateProperty();
            }
        }

        public string Telefono
        {
            get => _dto.Telefono;
            set
            {
                if (_dto.Telefono == value) return;
                _dto.Telefono = value;
                OnPropertyChanged();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
                ValidateProperty();
            }
        }

        public string? Email
        {
            get => _dto.Email;
            set
            {
                if (_dto.Email == value) return;
                _dto.Email = value;
                OnPropertyChanged();
                ValidateProperty();
            }
        }

        public string? LogoPreview
        {
            get => _logoPreview;
            set
            {
                if (_logoPreview == value) return;
                _logoPreview = value;
                OnPropertyChanged();
            }
        }

        public bool TieneLogo
        {
            get => _dto.TieneLogo;
            set
            {
                if (_dto.TieneLogo == value) return;
                _dto.TieneLogo = value;
                OnPropertyChanged();
            }
        }

        public bool PuedeGuardar() => !HasErrors && IsNotLoading;

        public override bool ValidateProperty([CallerMemberName] string? propertyName = null)
        {
            if (propertyName is null) return true;
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Nombre):
                    if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Trim().Length < 3)
                        SetError(nameof(Nombre), "Nombre requerido (mínimo 3 caracteres).");
                    break;
                case nameof(Direccion):
                    if (string.IsNullOrWhiteSpace(Direccion) || Direccion.Trim().Length < 3)
                        SetError(nameof(Direccion), "Dirección requerida (mínimo 3 caracteres).");
                    break;
                case nameof(Telefono):
                    if (string.IsNullOrWhiteSpace(Telefono) || Telefono.Trim().Length < 3)
                        SetError(nameof(Telefono), "Teléfono requerido (mínimo 3 caracteres).");
                    break;
            }

            ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            return !GetErrors(propertyName).Cast<object>().Any();
        }

        public bool ValidateAll()
        {
            ValidateProperty(nameof(Nombre));
            ValidateProperty(nameof(Direccion));
            ValidateProperty(nameof(Telefono));
            ValidateProperty(nameof(Email));
            ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            return !HasErrors;
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var dto = await _empresaService.ObtenerAsync();
                _dto = dto;
                OnPropertyChanged(nameof(Nombre));
                OnPropertyChanged(nameof(Direccion));
                OnPropertyChanged(nameof(Telefono));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(TieneLogo));
                ClearAllErrors();
                ((RelayCommand)GuardarCommand).RaiseCanExecuteChanged();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void SeleccionarLogo(Window owner)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Seleccionar logo de la empresa",
                Filter = "Imágenes (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Todos los archivos (*.*)|*.*",
                Multiselect = false
            };

            if (dlg.ShowDialog(owner) == true)
            {
                try
                {
                    _logoBytes = File.ReadAllBytes(dlg.FileName);
                    LogoPreview = dlg.FileName;
                    TieneLogo = true;
                }
                catch (Exception ex)
                {
                    FrmConfirmacion.MostrarAviso($"Error al cargar el logo: {ex.Message}", "Error", owner);
                }
            }
        }

        public async Task GuardarAsync()
        {
            if (!ValidateAll()) return;

            var main = Application.Current?.MainWindow as MainWindow;
            var owner = main ?? Application.Current?.MainWindow;

            if (owner != null && !TienePermiso(Modulo + ".Gestionar"))
            {
                FrmConfirmacion.MostrarAviso("No tiene permiso para gestionar la configuración de empresa.", "Sin permiso", owner);
                return;
            }

            IsLoading = true;
            try
            {
                var dto = await _empresaService.GuardarAsync(_dto, _logoBytes);
                _dto = dto;
                OnPropertyChanged(nameof(TieneLogo));
                OnPropertyChanged(nameof(LogoPreview));
                var ownerWin = owner;
                if (ownerWin != null)
                {
                    FrmConfirmacion.MostrarAviso("La configuración de la empresa fue guardada correctamente.", "Guardado", ownerWin);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}