using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.Models
{
    public class CuotaPago
    {
        public int Id { get; set; }
        public int CuotaId { get; set; }
        public int PagoId { get; set; }
        public decimal MontoCapital { get; set; }
        public decimal MontoInteres { get; set; }
        public decimal MontoGastos { get; set; }
        public decimal MontoSeguros { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoCastigo { get; set; }
        [ValidateNever]
        public virtual Cuota Cuota { get; set; } = null!;
        [ValidateNever]
        public virtual Pago Pago { get; set; } = null!;
    }
}
