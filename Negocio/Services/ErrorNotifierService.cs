// walterg20
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// Implementación del notificador centralizado de errores silenciosos.
    /// Registra en Debug y en archivo de log, deduplica en 2s y dispara evento.
    /// </summary>
    public class ErrorNotifierService : IErrorNotifier
    {
        private readonly string _logPath;
        private readonly ConcurrentDictionary<string, DateTime> _dedup = new();
        private const int DedupSegundos = 2;

        public ErrorNotifierService()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TP_ControlVehicular", "logs");
            try { Directory.CreateDirectory(dir); } catch { }
            _logPath = Path.Combine(dir, "error.log");
        }

        public event EventHandler<ErrorNotificadoEventArgs>? ErrorOccurred;

        public void Notify(Exception? ex, string contexto, bool modal = false)
        {
            try
            {
                var mensaje = string.IsNullOrWhiteSpace(ex?.Message)
                    ? $"Ocurrió un error en {contexto}."
                    : ex!.Message;

                // Dedup
                var key = $"{contexto}|{mensaje}";
                var ahora = DateTime.Now;
                if (_dedup.TryGetValue(key, out var ultimo) && (ahora - ultimo).TotalSeconds < DedupSegundos)
                {
                    return;
                }
                _dedup[key] = ahora;

                // Limpiar dedup viejos
                foreach (var kv in _dedup)
                {
                    if ((ahora - kv.Value).TotalSeconds > DedupSegundos * 5)
                        _dedup.TryRemove(kv.Key, out _);
                }

                // Log a Debug
                Debug.WriteLine($"[ErrorNotifier] {contexto}: {ex?.GetType().Name} - {ex?.Message}");
                (App.ServiceProvider?.GetService(typeof(IErrorNotifier)) as IErrorNotifier)?.Notify(ex, "ErrorNotifierService");

                // Log a archivo
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"[{ahora:yyyy-MM-dd HH:mm:ss}] Contexto: {contexto}");
                    sb.AppendLine($"Mensaje: {mensaje}");
                    if (ex != null)
                    {
                        sb.AppendLine($"Tipo: {ex.GetType().FullName}");
                        sb.AppendLine($"Stack: {ex.StackTrace}");
                    }
                    sb.AppendLine(new string('-', 60));
                    File.AppendAllText(_logPath, sb.ToString());
                }
                catch { /* no romper el flujo si el log falla */ }

                // Disparar evento para la UI
                ErrorOccurred?.Invoke(this, new ErrorNotificadoEventArgs
                {
                    Mensaje = mensaje,
                    Excepcion = ex,
                    Contexto = contexto,
                    Modal = modal
                });
            }
            catch
            {
                // El notifier nunca debe lanzar
            }
        }
    }
}