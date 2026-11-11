// walterg20
namespace TP_ControlVehicular.Negocio.DTOs
{
    /// <summary>
    /// Serie de datos para los gráficos del dashboard (una barra o porción por elemento).
    /// El mismo tipo representa conteos y montos: el control decide cómo formatear el valor.
    /// </summary>
    public class DashboardSerieDto
    {
        /// <summary>Etiqueta del eje (mes, estado, modelo, servicio...).</summary>
        public string Etiqueta { get; set; } = string.Empty;

        /// <summary>Valor de la serie.</summary>
        public decimal Valor { get; set; }
    }
}
