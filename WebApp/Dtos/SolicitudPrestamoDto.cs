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

        [Required]
        [DisplayName("Monto Solicitado")]
        [DataType(DataType.Currency)]
        public decimal MontoSolicitado { get; set; }

        [Required]
        [DisplayName("Cantidad Cuotas")]
        public int CantidadCuotas { get; set; }

        [Required]
        [DisplayName("Monto Cuota")]
        [DataType(DataType.Currency)]
        public decimal ValorCuota { get; set; }

        [Required]
        [DisplayName("Costo Total Financiero")]
        [DataType(DataType.Currency)]
        public decimal CostoTotalFinanciero { get; set; }

        [DisplayName("Tasa Cobertura Deuda Conyuge")]
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
