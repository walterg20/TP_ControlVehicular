using System;
using System.Collections.Generic;
using System.Text;

namespace TP_ControlVehicular.Negocio.DTOs
{
    public class TallerDto
    {
        public int IdTaller { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
    }
}
