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
            RestaurarBackupCommand = new RelayCommand(async () => await RestaurarBackupAsync(), () => PuedeRestaurar);
            RestaurarDesdeDriveCommand = new RelayCommand(async () => await RestaurarDesdeDriveAsync(), () => PuedeGestionar);
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
                    OnPropertyChanged(nameof(PuedeRestaurar));
                    NotificarComandos();
                }
            }
        }

        public ICommand HacerBackupCommand { get; }
        public ICommand ProbarBackupCommand { get; }
        public ICommand RestaurarBackupCommand { get; }
        public ICommand RestaurarDesdeDriveCommand { get; }
        public ICommand ResguardarCommand { get; }
        public ICommand ActualizarCommand { get; }

        public bool PuedeGestionar => TienePermiso($"{Modulo}.Gestionar") && IsNotLoading;
        public bool PuedeProbar => PuedeGestionar && RespaldoSeleccionado != null;
        public bool PuedeRestaurar => PuedeGestionar && RespaldoSeleccionado != null;

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

        public async Task RestaurarBackupAsync()
        {
            if (!PuedeGestionarSinCargando())
            {
                MostrarAviso("No tiene permiso para gestionar los respaldos.", "Sin permiso");
                return;
            }

            if (RespaldoSeleccionado == null)
            {
                MostrarAviso("Por favor, seleccione un respaldo para restaurar.", "Sin selección");
                return;
            }

            var archivo = RespaldoSeleccionado;
            if (!Confirmar($"¿Desea RESTAURAR la base de datos desde '{archivo.Nombre}'?\n\nEsta operación SOBREESCRIBE la base de datos actual y no se puede deshacer. Requiere autorización de un segundo usuario.", "Confirmar restauración"))
                return;

            var frm = new TP_ControlVehicular.Presentacion.Pantalla.Configuracion.FrmAutorizacionBackup();
            frm.Owner = Owner;
            if (frm.ShowDialog() != true)
                return;

            var dni = frm.DniAutorizador;
            var contrasena = frm.ContrasenaAutorizador;

            IsLoading = true;
            NotificarComandos();
            try
            {
                var resultado = await _databaseBackupService.RestaurarAsync(archivo.RutaCompleta, dni, contrasena);
                if (resultado.Exitoso)
                {
                    MostrarAviso($"{resultado.Mensaje}", "Restauración completada", icono: "✅");
                    await CargarRespaldosAsync();
                }
                else
                {
                    MostrarAviso($"No se pudo restaurar:\n{resultado.Mensaje}", "Error");
                }
            }
            catch (Exception ex)
            {
                MostrarAviso($"Error al restaurar la base de datos: {ex.Message}", "Error");
            }
            finally
            {
                IsLoading = false;
                NotificarComandos();
            }
        }

        public async Task RestaurarDesdeDriveAsync()
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

            IsLoading = true;
            NotificarComandos();
            IReadOnlyList<DriveArchivoDto> archivos;
            try
            {
                archivos = await _driveBackupService.ListarArchivosAsync();
            }
            catch (Exception ex)
            {
                IsLoading = false;
                NotificarComandos();
                MostrarAviso($"No se pudieron listar los respaldos de Drive: {ex.Message}", "Error");
                return;
            }
            finally
            {
                IsLoading = false;
                NotificarComandos();
            }

            if (archivos == null || archivos.Count == 0)
            {
                MostrarAviso("No se encontraron respaldos (.bak) en Google Drive.", "Sin archivos");
                return;
            }

            var frmSeleccion = new TP_ControlVehicular.Presentacion.Pantalla.Configuracion.FrmSeleccionarBackupDrive(archivos);
            frmSeleccion.Owner = Owner;
            if (frmSeleccion.ShowDialog() != true || frmSeleccion.ArchivoSeleccionado == null)
                return;

            var seleccionado = frmSeleccion.ArchivoSeleccionado;

            if (!Confirmar($"¿Desea descargar y restaurar la base de datos desde '{seleccionado.Nombre}' (Google Drive)?\n\nEl archivo se descargará a la carpeta local de respaldos y luego se restaurará, sobrescribiendo la base de datos actual.", "Confirmar restauración desde Drive"))
                return;

            IsLoading = true;
            NotificarComandos();
            string rutaLocal;
            try
            {
                rutaLocal = await _driveBackupService.DescargarAsync(seleccionado.Id, seleccionado.Nombre);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                NotificarComandos();
                MostrarAviso($"Error al descargar el respaldo: {ex.Message}", "Error");
                return;
            }

            // Segundo factor.
            var frmAuth = new TP_ControlVehicular.Presentacion.Pantalla.Configuracion.FrmAutorizacionBackup();
            frmAuth.Owner = Owner;
            if (frmAuth.ShowDialog() != true)
            {
                IsLoading = false;
                NotificarComandos();
                return;
            }

            try
            {
                var resultado = await _databaseBackupService.RestaurarAsync(rutaLocal, frmAuth.DniAutorizador, frmAuth.ContrasenaAutorizador);
                if (resultado.Exitoso)
                {
                    MostrarAviso($"{resultado.Mensaje}", "Restauración completada", icono: "✅");
                    await CargarRespaldosAsync();
                }
                else
                {
                    MostrarAviso($"No se pudo restaurar:\n{resultado.Mensaje}", "Error");
                }
            }
            catch (Exception ex)
            {
                MostrarAviso($"Error al restaurar la base de datos: {ex.Message}", "Error");
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
            ((RelayCommand)RestaurarBackupCommand).RaiseCanExecuteChanged();
            ((RelayCommand)RestaurarDesdeDriveCommand).RaiseCanExecuteChanged();
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
