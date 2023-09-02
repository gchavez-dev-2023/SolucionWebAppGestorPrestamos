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
        public int EdadMinima { get; set; }
        public int EdadMaxima { get; set; }
        public int ScoringMinimo { get; set; }
        public int CantidadAvales { get; set; }
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
        public decimal MontoMinimo { get; set; }
        public decimal MontoMaximo { get; set; }
        public int PlazoMinimo { get; set; }
        public int PlazoMaximo { get; set; }
        public decimal TasaNominal { get; set; }
        public decimal TasaGastosAdministrativos { get; set; }
        public decimal TasaGastosCobranza { get; set; }
        public decimal TasaSeguros { get; set; }
        public decimal TasaInteresMora { get; set; }
        [Required]
        [Column(TypeName = "bool")]
        [DisplayName("¿Se puede Pre-Pagar?")]
        public bool EsPrepagable { get; set; }
        public decimal TasaCastigoPrepago { get; set; }
        public decimal TasaCoberturaAval { get; set; }
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
