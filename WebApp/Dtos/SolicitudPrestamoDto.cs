using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApp.Dtos
{
    public class SolicitudPrestamoDto
    {
        public int Id { get; set; }

        [DisplayName("Clientes")]
        public int ClienteId { get; set; }
        public ClienteDto? ClienteDto { get; set; }

        [DisplayName("Productos")]
        public int ProductoId { get; set; }
        public ProductoDto? ProductoDto { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 10.000,00 y 100.000.000,00")]
        [Range(10000, 1000000000)]
        [DataType(DataType.Currency)]
        [DisplayName("Monto Solicitado")]
        public decimal MontoSolicitado { get; set; }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 600")]
        [Range(0, 600)]
        [DisplayName("Plazo Minimo")]
        public int CantidadCuotas { get; set; }

        [Required]
        [DisplayName("Monto Cuota")]
        [DataType(DataType.Currency)]
        public decimal ValorCuota { get; set; }

        [Required]
        [DisplayName("Costo Total Financiero")]
        [DataType(DataType.Currency)]
        public decimal CostoTotalFinanciero { get; set; }

        public decimal TasaCoberturaDeudaConyuge { get; set; }

        [Required]
        [DisplayName("Fecha de Solicitud")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSolicitud { get; set; }

        public string UrlDocumento { get; set; } = null!;

        [Required]
        [DisplayName("Estado Solicitud")]
        public string Estado { get; set; } = null!;

        [Required]
        [DisplayName("Cantidad de Avales")]
        public int CantidadAvales { get; set; }

        public List<AvalDto>? AvalesDto { get; set; }
    }
}
