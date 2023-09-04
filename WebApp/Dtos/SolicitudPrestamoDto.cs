namespace WebApp.Dtos
{
    public class SolicitudPrestamoDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public ClienteDto? ClienteDto { get; set; }
        public int ProductoId { get; set; }
        public ProductoDto? ProductoDto { get; set; }
        public decimal MontoSolicitado { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal CostoTotalFinanciero { get; set; }
        public decimal TasaCoberturaDeudaConyuge { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string UrlDocumento { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public int CantidadAvales { get; set; }

        public List<AvalDto>? AvalesDto { get; set; }
    }
}
