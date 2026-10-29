// walterg20
using System.Diagnostics;

namespace TP_ControlVehicular.Presentacion.Helpers
{
    /// <summary>
    /// Coalesce multiple rapid invocations into a single deferred one.
    /// Se usa para que la busqueda en el servidor no dispare una consulta a la base
    /// de datos por cada tecla escrita, manteniendo la sensacion de filtrado en vivo.
    /// </summary>
    /// <remarks>
    /// La espera se hace con <see cref="Task.Delay(TimeSpan, CancellationToken)"/>, que captura
    /// el <see cref="System.Threading.SynchronizationContext"/> vigente (el de WPF en la
    /// aplicacion), de modo que la accion diferida se ejecuta de vuelta en el hilo de interfaz.
    /// </remarks>
    public sealed class Debouncer
    {
        /// <summary>Retraso por defecto de la busqueda server-side.</summary>
        public static readonly TimeSpan RetrasoBusquedaPorDefecto = TimeSpan.FromMilliseconds(500);

        private readonly TimeSpan _delay;
        private CancellationTokenSource? _cts;
        private Func<Task>? _pendiente;

        public Debouncer() : this(RetrasoBusquedaPorDefecto)
        {
        }

        public Debouncer(TimeSpan delay)
        {
            _delay = delay;
        }

        /// <summary>Programa la accion, cancelando cualquier ejecucion pendiente anterior.</summary>
        public void Debounce(Func<Task> accion)
        {
            if (accion is null) return;

            _pendiente = accion;

            _cts?.Cancel();
            _cts?.Dispose();

            var cts = new CancellationTokenSource();
            _cts = cts;
            _ = EsperarAsync(cts.Token);
        }

        /// <summary>Ejecuta de inmediato la accion pendiente, si la hay.</summary>
        public void Flush()
        {
            var accion = _pendiente;
            if (accion is null) return;

            _pendiente = null;
            _cts?.Cancel();
            _ = EjecutarSeguro(accion);
        }

        /// <summary>Cancela la accion pendiente sin ejecutarla.</summary>
        public void Cancel()
        {
            _pendiente = null;
            _cts?.Cancel();
        }

        private async Task EsperarAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(_delay, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (token.IsCancellationRequested) return;

            var accion = _pendiente;
            _pendiente = null;
            if (accion is null) return;

            await EjecutarSeguro(accion);
        }

        private static async Task EjecutarSeguro(Func<Task> accion)
        {
            try
            {
                await accion();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Debouncer] Error al ejecutar la accion diferida: {ex.Message}");
            }
        }
    }
}
