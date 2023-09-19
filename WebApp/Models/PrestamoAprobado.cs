using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class PrestamoAprobado
    {
        public PrestamoAprobado()
        {
            Cuotas = new HashSet<Cuota>();
            Pagos = new HashSet<Pago>();
        }

        public int Id { get; set; }
        public int SolicitudPrestamoId { get; set; }
        public decimal MontoAprobado { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal CostoTotalFinanciero { get; set; }
        public decimal MontoInteres { get; set; }
        public decimal MontoGastosContratacion { get; set; }
        public decimal MontoGastosMantencion { get; set; }
        public decimal MontoSeguros { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoCastigo { get; set; }
        public DateTime FechaAprobacion { get; set; }
        public DateTime FechaDesembolso { get; set; }
        public DateTime FechaPrimerVencimiento { get; set; }
        public string UrlDocumento { get; set; } = null!;
        public string Estado { get; set; } = null!;
        [ValidateNever]
        public virtual SolicitudPrestamo SolicitudPrestamo { get; set; } = null!;
        public virtual ICollection<Cuota> Cuotas { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }
}
