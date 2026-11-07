// walterg20
using System;

namespace TP_ControlVehicular.Negocio.Services
{
    /// <summary>
    /// Notificador centralizado de errores silenciosos. Captura excepciones de bloques catch
    /// y las expone al usuario mediante toast/modal unificado.
    /// </summary>
    public interface IErrorNotifier
    {
        /// <summary>
        /// Notifica un error al usuario. Deduplica notificaciones idénticas en 2 segundos.
        /// </summary>
        /// <param name="ex">Excepción capturada (puede ser null).</param>
        /// <param name="contexto">Contexto donde ocurrió (ej. "OrdenServicio.Guardar").</param>
        /// <param name="modal">Si true, muestra modal bloqueante; si false, toast.</param>
        void Notify(Exception? ex, string contexto, bool modal = false);

        /// <summary>
        /// Evento disparado cuando se notifica un error (para que la UI lo muestre).
        /// </summary>
        event EventHandler<ErrorNotificadoEventArgs>? ErrorOccurred;
    }

    /// <summary>
    /// Argumentos del evento de error notificado.
    /// </summary>
    public class ErrorNotificadoEventArgs : EventArgs
    {
        public string Mensaje { get; set; } = string.Empty;
        public Exception? Excepcion { get; set; }
        public string Contexto { get; set; } = string.Empty;
        public bool Modal { get; set; }
    }
}