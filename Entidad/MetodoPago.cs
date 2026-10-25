using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP_ControlVehicular.Entidad
{
    [Table("MetodoPago")]
    public class MetodoPago
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;
    }
}
