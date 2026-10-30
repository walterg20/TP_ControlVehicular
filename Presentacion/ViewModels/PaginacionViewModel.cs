// walterg20
using System;
using System.Threading.Tasks;

namespace TP_ControlVehicular.Presentacion.ViewModels
{
    /// <summary>
    /// Estado de paginacion independiente del ViewModel principal. Se usa en las pantallas
    /// con mas de una grilla paginada (por ejemplo Reportes de Recepcion), donde cada grilla
    /// necesita su propia pagina actual.
    /// </summary>
    public class PaginacionViewModel : BasePaginableViewModel
    {
        private readonly Func<int, int, Task> _cargar;

        /// <param name="cargar">Delegado que recibe (pagina, tamanoPagina) y recarga los datos.</param>
        public PaginacionViewModel(Func<int, int, Task> cargar)
        {
            _cargar = cargar;
        }

        /// <summary>Recarga los datos de la pagina actual.</summary>
        public Task RecargarAsync() => CargarPaginaAsync();

        protected override Task CargarPaginaAsync() => _cargar(PaginaActual, TamanoPagina);
    }
}
