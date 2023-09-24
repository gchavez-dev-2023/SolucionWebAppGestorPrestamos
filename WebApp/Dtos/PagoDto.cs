using ExpressiveAnnotations.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;

namespace WebApp.Dtos
{
    public class PagoDto
    {
        public int Id { get; set; }

        [DisplayName("Prestamo")]
        public int PrestamoAprobadoId { get; set; }
        public int? CuotaId { get; set; }

        [DisplayName("Numero de Cuota")]
        public int NumeroCuota { get; set; }

        [DisplayName("Fecha de Pago")]
        public DateTime FechaPago { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de Pago")]
        public virtual DateTime FechaPagoDisplay { get { return FechaPago; } }

        //[Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 100,00 y 100.000.000,00")]
        //[Range(100, 100000000)]
        [DataType(DataType.Currency)]
        [DisplayName("Monto Pago")]
        [AssertThat("(MontoPago >= MontoMinimoPago && MontoPago <= MontoMaximoPago)", ErrorMessage = "Debe ingresar monto, valores permitidos Monto Minimo y Monto Minimo")]
        public decimal MontoPago { get; set; }

        [DisplayName("Monto Pago")]
        public virtual string MontoPagoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoPago); } }

        [DisplayName("Forma de Pago")]
        public int FormaPagoId { get; set; }
        [DisplayName("Forma de Pago")]
        public string? FormaPagoDescripcion { get; set; }

        [DisplayName("Persona que Paga")]
        public int PersonaId { get; set; }

        [DisplayName("Monto Minimo Pago")]
        public decimal MontoMinimoPago { get; set; }

        [DisplayName("Monto Maximo Pago")]
        public decimal MontoMaximoPago { get; set; }

    }
}
