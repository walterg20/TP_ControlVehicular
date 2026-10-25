using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP_ControlVehicular.Entidad
{
    [Table("Pago")]
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        public int FacturaId { get; set; }

        public int MetodoPagoId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        public DateTime FechaPago { get; set; }

        [ForeignKey("FacturaId")]
        public Factura Factura { get; set; } = null!;

        [ForeignKey("MetodoPagoId")]
        public MetodoPago MetodoPago { get; set; } = null!;
    }
}
