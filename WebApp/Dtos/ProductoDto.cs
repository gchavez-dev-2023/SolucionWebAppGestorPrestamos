using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Dtos
{
    public class ProductoDto
    {

        [DisplayName("Producto")]
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; } = null!;
        public int RequisitosId { get; set; }

        [Required(ErrorMessage = "Debe ingresar edad, valores permitidos entre 18 y 100.")]
        [Range(18, 100)]
        [DisplayName("Edad Minima")]
        public int EdadMinima { get; set; }
        
        [Required(ErrorMessage = "Debe ingresar edad, valores permitidos entre 18 y 100.")]
        [Range(18, 100)]
        [DisplayName("Edad Limite")]
        public int EdadMaxima { get; set; }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 100")]
        [Range(0, 100)]
        [DisplayName("Scoring Minimo")]
        public int ScoringMinimo { get; set; }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 10")]
        [Range(0, 10)]
        [DisplayName("Cantidad de Avales")]
        public int CantidadAvales { get; set; }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 10")]
        [Range(0, 10)]
        [DisplayName("Cantidad Recibos Sueldo")]
        public int CantidadRecibosSueldo { get; set; }

        public int BeneficiosId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar valor")]
        [Column(TypeName = "bool")]
        [DisplayName("¿Posee Aprobación 24 Horas?")]
        public bool Aprobacion24Horas { get; set; }

        [Required(ErrorMessage = "Debe seleccionar valor")]
        [Column(TypeName = "bool")]
        [DisplayName("¿Se puede solicitar en linea?")]
        public bool SolicitudEnLinea { get; set; }
        public int TerminosId { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 10.000,00 y 100.000.000,00")]
        [Range(10000, 1000000000)]
        [DataType(DataType.Currency)]
        [DisplayName("Monto Minimo")]
        public decimal MontoMinimo { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 10.000,00 y 100.000.000,00")]
        [Range(10000, 1000000000)]
        [DataType(DataType.Currency)]
        [DisplayName("Monto Maximo")]
        public decimal MontoMaximo { get; set; }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 600")]
        [Range(0, 600)]
        [DisplayName("Plazo Minimo")]
        public int PlazoMinimo { get; set; }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 600")]
        [Range(0, 600)]
        [DisplayName("Plazo Maximo")]
        public int PlazoMaximo { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Nominal")]
        public decimal TasaNominal { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Gastos Contratación")]
        public decimal TasaGastosAdministrativos { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Gastos Admin. Mensual")]
        public decimal TasaGastosCobranza { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Seguros Mensual")]
        public decimal TasaSeguros { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Interes Mora Diario")]
        public decimal TasaInteresMora { get; set; }

        [Required(ErrorMessage = "Debe seleccionar valor")]
        [Column(TypeName = "bool")]
        [DisplayName("¿Se puede Pre-Pagar?")]
        public bool EsPrepagable { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Castigo Prepago")]
        public decimal TasaCastigoPrepago { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 100,000")]
        [Range(0, 100)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Cobertura Aval")]
        public decimal TasaCoberturaAval { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 100,000")]
        [Range(0, 100)]
        [DataType(DataType.Currency)]
        [DisplayName("Tasa Cobertura Conyuge")]
        public decimal TasaCoberturaConyuge { get; set; }

        [Required(ErrorMessage = "Debe ingresar fecha valida.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha Inicio Vigencia")]
        public DateTime FechaInicioVigencia { get; set; }

        [Required(ErrorMessage = "Debe ingresar fecha valida, menor a la Fecha Inicio Vigencia.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha Fin Vigencia")]
        public DateTime FechaFinVigencia { get; set; }
    }
}
