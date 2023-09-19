using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Globalization;

namespace WebApp.Dtos
{
    public class CuotaDto
    {

        public int Id { get; set; }

        [DisplayName("Prestamo")]
        public int PrestamoAprobadoId { get; set; }

        [DisplayName("Numero de Cuota")]
        public int NumeroCuota { get; set; }

        [DisplayName("Fecha de Vencimiento")]
        public DateTime FechaVencimiento { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha de Vencimiento")]
        public virtual DateTime FechaVencimientoDisplay { get { return FechaVencimiento; } }

        [DisplayName("Monto Capital")]
        public decimal MontoCapital { get; set; }
        [DisplayName("Monto Capital")]
        public virtual string MontoCapitalDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoCapital); } }
        public decimal MontoInteres { get; set; }
        public decimal MontoGastos { get; set; }
        public decimal MontoSeguros { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoCastigo { get; set; }
        public string UrlDocumento { get; set; } = null!;
        public string Estado { get; set; } = null!;

        [DisplayName("Monto Total Cuota")]
        public decimal MontoTotalCuota { get; set; }
        [DisplayName("Monto Total Cuota")]
        public virtual string MontoTotalCuotaDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoTotalCuota); } }

        [DisplayName("Monto Total Pagado")]
        public decimal CuotaTotalPagado { get; set; }
        [DisplayName("Monto Total Pagado")]
        public virtual string CuotaTotalPagadoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", CuotaTotalPagado); } }

        [DisplayName("Monto Total Deuda")]
        public decimal CuotaTotalDeuda { get; set; }
        [DisplayName("Monto Total Deuda")]
        public virtual string CuotaTotalDeudaDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", CuotaTotalDeuda); } }
        public List<PagoDto>? CuotaPagosDto { get; set; }

    }
}
