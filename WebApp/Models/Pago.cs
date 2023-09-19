using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Pago
    {
        public Pago()
        {
            CuotasPagos = new HashSet<CuotaPago>();
        }
        public int Id { get; set; }
        public int PrestamoAprobadoId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoPago { get; set; }
        public int FormaPagoId { get; set; }
        public int PersonaId { get; set; }
        public string UrlDocumento { get; set; } = null!;
        [ValidateNever]
        public virtual PrestamoAprobado PrestamoAprobado { get; set; } = null!;
        [ValidateNever]
        public virtual FormaPago FormaPago { get; set; } = null!;
        [ValidateNever]
        public virtual Persona Persona { get; set; } = null!;
        public virtual ICollection<CuotaPago> CuotasPagos { get; set; }
    }
}
