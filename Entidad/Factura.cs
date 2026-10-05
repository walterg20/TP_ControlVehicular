using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP_ControlVehicular.Entidad
{
    [Table("Factura")]
    [Microsoft.EntityFrameworkCore.Index(nameof(RegistroServicioId), IsUnique = true)]
    public class Factura
    {
        [Key]
        public int Id { get; set; }

        public int RegistroServicioId { get; set; }

        public int Numero { get; set; }

        public DateTime Fecha { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [ForeignKey("RegistroServicioId")]
        public RegistroServicio RegistroServicio { get; set; } = null!;

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
