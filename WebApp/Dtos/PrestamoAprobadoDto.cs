using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApp.Dtos
{
    public class PrestamoAprobadoDto
    {
        [DisplayName("Prestamo")]
        public int Id { get; set; }

        [DisplayName("Solicitud")]
        public int SolicitudPrestamoId { get; set; }

        [DisplayName("Cedula Identidad Cliente")]
        public int ClienteId { get; set; }
        public ClienteDto? ClienteDto { get; set; }

        [DisplayName("Nombre Producto")]
        public int ProductoId { get; set; }
        public ProductoDto? ProductoDto { get; set; }

        [DisplayName("Monto Aprobado")]
        public decimal MontoAprobado { get; set; }
        [DisplayName("Monto Aprobado")]
        public virtual string MontoAprobadoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoAprobado); } }

        [DisplayName("Cantidad de Cuotas")]
        public int CantidadCuotas { get; set; }

        [DisplayName("Valor Cuota")]
        public decimal ValorCuota { get; set; }
        [DisplayName("Valor Cuota")]
        public virtual string ValorCuotaDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", ValorCuota); } }

        [DisplayName("Costo Total Financiero")]
        public decimal CostoTotalFinanciero { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Costo Total Financiero")]
        public virtual decimal CostoTotalFinancieroPercent { get { return CostoTotalFinanciero / 100; } }

        [DisplayName("Monto Interes")]
        public decimal MontoInteres { get; set; }
        [DisplayName("Monto Interes")]
        public virtual string MontoInteresDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoInteres); } }

        [DisplayName("Monto Gastos Contratacion")]
        public decimal MontoGastosContratacion { get; set; }
        [DisplayName("Monto Gastos Contratacion")]
        public virtual string MontoGastosContratacionDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoGastosContratacion); } }

        [DisplayName("Monto Gastos Mantencion Mensual")]
        public decimal MontoGastosMantencionMensual { get; set; }
        [DisplayName("Monto Gastos Mantencion Mensual")]
        public virtual string MontoGastosMantencionMensualDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoGastosMantencionMensual); } }

        [DisplayName("Monto Seguros Mensual")]
        public decimal MontoSegurosMensual { get; set; }
        [DisplayName("Monto Seguros Mensual")]
        public virtual string MontoSegurosMensualDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoSegurosMensual); } }

        [DisplayName("Fecha de Aprobacion")]
        public DateTime FechaAprobacion { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de Aprobacion")]
        public virtual DateTime FechaAprobacionDisplay { get { return FechaAprobacion; } }

        [DisplayName("Fecha Desembolso")]
        public DateTime FechaDesembolso { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Desembolso")]
        public virtual DateTime FechaDesembolsonDisplay { get { return FechaDesembolso; } }

        [DisplayName("Fecha Primer Vencimiento")]
        public DateTime FechaPrimerVencimiento { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha Primer Vencimiento")]
        public virtual DateTime FechaPrimerVencimientoDisplay { get { return FechaPrimerVencimiento; } }
        public string UrlDocumento { get; set; } = null!;
        public string Estado { get; set; } = null!;

        [DisplayName("Monto Total Prestamo")]
        public decimal PrestamoTotal { get; set; }

        [DisplayName("Monto Total Prestamo")]
        public virtual string PrestamoTotalDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", PrestamoTotal); } }

        [DisplayName("Monto Total Pagado")]
        public decimal PrestamoTotalPagado { get; set; }

        [DisplayName("Monto Total Pagado")]
        public virtual string PrestamoTotalPagadoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", PrestamoTotalPagado); } }

        [DisplayName("Monto Total Adeudado")]
        public decimal PrestamoTotalDeuda { get; set; }

        [DisplayName("Monto Total Adeudado")]
        public virtual string PrestamoTotalDeudaDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", PrestamoTotalDeuda); } }

        public List<PagoDto>? CuotaPagosDto { get; set; }
        public List<CuotaDto>? PrestamoCuotasDto { get; set; }

    }
}
