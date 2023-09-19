using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Cuota
    {
        public Cuota()
        {
            CuotasPagos = new HashSet<CuotaPago>();
        }

        public int Id { get; set; }
        public int PrestamoAprobadoId { get; set; }
        public int NumeroCuota { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoCapital { get; set; }
        public decimal MontoInteres { get; set; }
        public decimal MontoGastos { get; set; }
        public decimal MontoSeguros { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoCastigo { get; set; }
        public string UrlDocumento { get; set; } = null!;
        public string Estado { get; set; } = null!;
        [ValidateNever]
        public virtual PrestamoAprobado PrestamoAprobado { get; set; } = null!;
        public virtual ICollection<CuotaPago> CuotasPagos { get; set; }
    }
}
