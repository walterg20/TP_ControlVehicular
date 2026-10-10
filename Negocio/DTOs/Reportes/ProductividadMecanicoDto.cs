using System;

namespace TP_ControlVehicular.Negocio.DTOs.Reportes
{
    public class ProductividadMecanicoDto
    {
        public int MecanicoId { get; set; }
        public string MecanicoNombre { get; set; } = string.Empty;
        public int TareasAsignadas { get; set; }
        public int Pendientes { get; set; }
        public int EnCurso { get; set; }
        public int TareasCompletadas { get; set; }

        public double PorcentajeAvance => TareasAsignadas == 0
            ? 0
            : Math.Round(TareasCompletadas * 100.0 / TareasAsignadas, 0);

        public string PorcentajeAvanceTexto => $"{PorcentajeAvance:0}%";
    }
}
