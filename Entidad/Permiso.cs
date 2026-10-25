using System.Collections.Generic;

namespace TP_ControlVehicular.Entidad
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Navegación
        public ICollection<Rol> Roles { get; set; } = new List<Rol>();
    }
}
