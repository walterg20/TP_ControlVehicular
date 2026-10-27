// walterg20
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Interfaces;
using TP_ControlVehicular.Presentacion.Pantalla.Compartido;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        private readonly IDatabaseBackupService _databaseBackupService;
        private readonly IDriveBackupService _driveBackupService;

        private BackupArchivoDto? _respaldoSeleccionado;

        public BackupViewModel(
            IDatabaseBackupService databaseBackupService,
            IDriveBackupService driveBackupService)
        {
            _databaseBackupService = databaseBackupService;
            _driveBackupService = driveBackupService;

            Respaldos = new ObservableCollection<BackupArchivoDto>();

            HacerBackupCommand = new RelayCommand(async () => await HacerBackupAsync(), () => PuedeGestionar);
            ProbarBackupCommand = new RelayCommand(async () => await ProbarBackupAsync(), () => PuedeProbar);
            ResguardarCommand = new RelayCommand(async () => await ResguardarAsync(), () => PuedeGestionar);
            ActualizarCommand = new RelayCommand(async () => await CargarListaAsync(), () => PuedeGestionar);
        }

        protected override string Modulo => "Configuracion.Backup";

        public ObservableCollection<BackupArchivoDto> Respaldos { get; }

        public BackupArchivoDto? RespaldoSeleccionado
        {
            get => _respaldoSeleccionado;
            set
            {
                if (SetProperty(ref _respaldoSeleccionado, value))
                {
                    OnPropertyChanged(nameof(PuedeProbar));
                    NotificarComandos();
                }
            }
        }

        public ICommand HacerBackupCommand { get; }
        public ICommand ProbarBackupCommand { get; }
        public ICommand ResguardarCommand { get; }
        public ICommand ActualizarCommand { get; }

        public bool PuedeGestionar => TienePermiso($"{Modulo}.Gestionar") && IsNotLoading;
        public bool PuedeProbar => PuedeGestionar && RespaldoSeleccionado != null;

        public async Task LoadAsync()
        {
            await CargarListaAsync();
        }

        public async Task CargarListaAsync()
        {
            IsLoading = true;
            NotificarComandos();
            try
            {
                await CargarRespaldosAsync();
            }
            catch (Exception ex)
            {
                MostrarAviso($"No se pudieron listar los respaldos: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
                NotificarComandos();
            }
        }

        public async Task HacerBackupAsync()
        {
            if (!PuedeGestionarSinCargando())
            {
                MostrarAviso("No tiene permiso para gestionar los respaldos.", "Sin permiso");
                return;
            }

            if (!Confirmar("¿Desea generar un nuevo respaldo de la base de datos ahora?", "Confirmar respaldo"))
                return;

            IsLoading = true;
            NotificarComandos();
            try
            {
                var resultado = await _databaseBackupService.HacerBackupAsync();
                if (resultado.Exitoso)
                {
                    MostrarAviso($"Respaldo generado correctamente:\n{resultado.Archivo}", "Respaldo completado");
                    await CargarRespaldosAsync();
                }
                else
                {
                    MostrarAviso($"No se pudo generar el respaldo:\n{resultado.Mensaje}", "Error");
                }
            }
            catch (Exception ex)
            {
                MostrarAviso($"Error al generar el respaldo: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
                NotificarComandos();
            }
        }

        public async Task ProbarBackupAsync()
        {
            if (!PuedeGestionarSinCargando())
            {
                MostrarAviso("No tiene permiso para gestionar los respaldos.", "Sin permiso");
                return;
            }

            if (RespaldoSeleccionado == null)
            {
                MostrarAviso("Por favor, seleccione un respaldo para probar.", "Sin selección");
                return;
            }

            var archivo = RespaldoSeleccionado;
            if (!Confirmar($"¿Desea probar el respaldo '{archivo.Nombre}'?\n\nSe restaurará en una base de prueba temporal; los datos reales no se modifican.", "Confirmar prueba"))
                return;

            IsLoading = true;
            NotificarComandos();
            try
            {
                var resultado = await _databaseBackupService.ProbarBackupAsync(archivo.RutaCompleta);
                if (resultado.Exitoso)
                {
                    MostrarAviso($"El respaldo es válido.\nBase de prueba: {resultado.NombreBasePrueba}\n\n{resultado.Detalle}", "Prueba exitosa", icono: "✅");
                }
                else
                {
                    MostrarAviso($"La prueba del respaldo falló:\n{resultado.Detalle}", "Prueba fallida", icono: "❌");
                }
            }
            catch (Exception ex)
            {
                MostrarAviso($"Error al probar el respaldo: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
                NotificarComandos();
            }
        }

        public async Task ResguardarAsync()
        {
            if (!PuedeGestionarSinCargando())
            {
                MostrarAviso("No tiene permiso para gestionar los respaldos.", "Sin permiso");
                return;
            }

            if (!_driveBackupService.TieneCredencialesGuardadas())
            {
                var owner = Owner;
                if (owner == null) return;

                var autenticado = await _driveBackupService.AutenticarAsync(owner);
                if (!autenticado)
                {
                    MostrarAviso("No se pudo conectar con Google Drive. Verifique las credenciales.", "Drive");
                    return;
                }
            }

            if (!Confirmar("¿Desea resguardar el último respaldo en Google Drive?", "Confirmar resguardo"))
                return;

            IsLoading = true;
            NotificarComandos();
            try
            {
                var resultado = await _driveBackupService.SubirUltimoAsync();
                MostrarAviso(resultado.Mensaje, resultado.Subido ? "Resguardo completado" : "Resguardo", icono: resultado.Subido ? "✅" : "⚠️");
            }
            catch (Exception ex)
            {
                MostrarAviso($"Error al resguardar en Drive: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
                NotificarComandos();
            }
        }

        private async Task CargarRespaldosAsync()
        {
            var lista = await _databaseBackupService.ListarAsync();
            Respaldos.Clear();
            foreach (var item in lista)
            {
                Respaldos.Add(item);
            }

            RespaldoSeleccionado = Respaldos.Count > 0 ? Respaldos[0] : null;
        }

        private bool PuedeGestionarSinCargando() => TienePermiso($"{Modulo}.Gestionar");

        private void NotificarComandos()
        {
            ((RelayCommand)HacerBackupCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ProbarBackupCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ResguardarCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ActualizarCommand).RaiseCanExecuteChanged();
        }

        private static Window? Owner => Application.Current?.MainWindow;

        private static bool Confirmar(string mensaje, string titulo)
        {
            return FrmConfirmacion.Mostrar(mensaje, titulo, Owner);
        }

        private static void MostrarAviso(string mensaje, string titulo, string icono = "⚠️")
        {
            FrmConfirmacion.MostrarAviso(mensaje, titulo, Owner, icono: icono);
        }
    }
}
