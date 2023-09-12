using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

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

        [DisplayName("Monto Minimo")]
        public virtual string MontoMinimoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoMinimo); } }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 10.000,00 y 100.000.000,00")]
        [Range(10000, 1000000000)]
        [DataType(DataType.Currency)]
        [DisplayName("Monto Maximo")]
        public decimal MontoMaximo { get; set; }

        [DisplayName("Monto Maximo")]
        public virtual string MontoMaximoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoMaximo); } }

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

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Nominal")]
        public virtual decimal TasaNominalPercent { get { return TasaNominal / 100; } }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DisplayName("Tasa Gastos Contratación")]
        public decimal TasaGastosAdministrativos { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Gastos Contratación")]
        public virtual decimal TasaGastosAdministrativosPercent { get { return TasaGastosAdministrativos / 100; } }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DisplayName("Tasa Gastos Admin. Mensual")]
        public decimal TasaGastosCobranza { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Gastos Admin. Mensual")]
        public virtual decimal TasaGastosCobranzaPercent { get { return TasaGastosCobranza / 100; } }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DisplayName("Tasa Seguros Mensual")]
        public decimal TasaSeguros { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Seguros Mensual")]
        public virtual decimal TasaSegurosPercent { get { return TasaSeguros / 100; } }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DisplayName("Tasa Interes Mora Diario")]
        public decimal TasaInteresMora { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Interes Mora Diario")]
        public virtual decimal TasaInteresMoraPercent { get { return TasaInteresMora / 100; } }

        [Required(ErrorMessage = "Debe seleccionar valor")]
        [Column(TypeName = "bool")]
        [DisplayName("¿Se puede Pre-Pagar?")]
        public bool EsPrepagable { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 1000,000")]
        [Range(0, 1000)]
        [DisplayName("Tasa Castigo Prepago")]
        public decimal TasaCastigoPrepago { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Castigo Prepago")]
        public virtual decimal TasaCastigoPrepagoPercent { get { return TasaCastigoPrepago / 100; } }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 100,000")]
        [Range(0, 100)]
        [DisplayName("Tasa Cobertura Aval")]
        public decimal TasaCoberturaAval { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Cobertura Aval")]
        public virtual decimal TasaCoberturaAvalPercent { get { return TasaCoberturaAval / 100; } }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 0,000 y 100,000")]
        [Range(0, 100)]
        [DisplayName("Tasa Cobertura Conyuge")]
        public decimal TasaCoberturaConyuge { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Cobertura Conyuge")]
        public virtual decimal TasaCoberturaConyugePercent { get { return TasaCoberturaConyuge / 100; } }

        [Required(ErrorMessage = "Debe ingresar fecha valida.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha Inicio Vigencia")]
        public DateTime FechaInicioVigencia { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha Inicio Vigencia")]
        public virtual DateTime FechaInicioVigenciaDisplay { get { return FechaInicioVigencia; } }

        [Required(ErrorMessage = "Debe ingresar fecha valida, menor a la Fecha Inicio Vigencia.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha Fin Vigencia")]
        public DateTime FechaFinVigencia { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha Fin Vigencia")]
        public virtual DateTime FechaFinVigenciaDisplay { get { return FechaFinVigencia; } }
    }
}
