using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Dtos
{
    public class ProductoDto
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; } = null!;
        public int RequisitosId { get; set; }

        [Required]
        [DisplayName("Edad Minima")]
        public int EdadMinima { get; set; }

        [Required]
        [DisplayName("Edad Limite")]
        public int EdadMaxima { get; set; }

        [Required]
        [DisplayName("Scoring Minimo")]
        public int ScoringMinimo { get; set; }

        [Required]
        [DisplayName("Cantidad Avales")]
        public int CantidadAvales { get; set; }

        [Required]
        [DisplayName("Cantidad Recibos Sueldo")]
        public int CantidadRecibosSueldo { get; set; }
        public int BeneficiosId { get; set; }
        [Required]
        [Column(TypeName = "bool")]
        [DisplayName("¿Posee Aprobación 24 Horas?")]
        public bool Aprobacion24Horas { get; set; }
        [Required]
        [Column(TypeName = "bool")]
        [DisplayName("¿Se puede solicitar en linea?")]
        public bool SolicitudEnLinea { get; set; }
        public int TerminosId { get; set; }

        [Required]
        [DisplayName("Monto Minimo")]
        [Column(TypeName = "decimal(14, 2)")]
        public decimal MontoMinimo { get; set; }
        [Required]
        [DisplayName("Monto Maximo")]
        [Column(TypeName = "decimal(14, 2)")]
        public decimal MontoMaximo { get; set; }

        [Required]
        [DisplayName("Plazo Minimo")]

        public int PlazoMinimo { get; set; }
        [Required]
        [DisplayName("Plazo Maximo")]
        public int PlazoMaximo { get; set; }

        [Required]
        [DisplayName("Tasa Nominal")]
        [Column(TypeName = "decimal(6, 3)")]
        public decimal TasaNominal { get; set; }

        [Required]
        [DisplayName("Tasa Gastos Contratación")]
        [Column(TypeName = "decimal(6, 3)")]
        public decimal TasaGastosAdministrativos { get; set; }

        [Required]
        [DisplayName("Tasa Gastos Admin. Mensual")]
        [Column(TypeName = "decimal(6, 3)")]
        public decimal TasaGastosCobranza { get; set; }

        [Required]
        [DisplayName("Tasa Seguros Mensual")]
        [Column(TypeName = "decimal(6, 3)")]
        public decimal TasaSeguros { get; set; }

        [Required]
        [DisplayName("Tasa Interes Mora Diario")]
        [Column(TypeName = "decimal(6, 3)")]
        public decimal TasaInteresMora { get; set; }

        [Required]
        [Column(TypeName = "bool")]
        [DisplayName("¿Se puede Pre-Pagar?")]
        public bool EsPrepagable { get; set; }

        [Required]
        [DisplayName("Tasa Castigo Prepago")]
        [Column(TypeName = "decimal(6, 3)")]
        public decimal TasaCastigoPrepago { get; set; }

        [Required]
        [DisplayName("Tasa Cobertura Aval")]
        [Column(TypeName = "decimal(6, 3)")]
        public decimal TasaCoberturaAval { get; set; }
        [Required]
        [DisplayName("Tasa Cobertura Conyuge")]
        public decimal TasaCoberturaConyuge { get; set; }
        [Required]
        [DisplayName("Fecha Inicio Vigencia")]
        [DataType(DataType.Date)]
        public DateTime FechaInicioVigencia { get; set; }
        [Required]
        [DisplayName("Fecha Fin Vigencia")]
        [DataType(DataType.Date)]
        public DateTime FechaFinVigencia { get; set; }
    }
}
