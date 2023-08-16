using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class PrestamoAprobado
    {
        public PrestamoAprobado()
        {
            Cuota = new HashSet<Cuota>();
        }

        public int Id { get; set; }
        public int SolicitudPrestamoId { get; set; }
        public decimal MontoAprobado { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal CostoTotalFinanciero { get; set; }
        public decimal MontoGastosContratacion { get; set; }
        public decimal MontoGastosMantencionMensual { get; set; }
        public decimal MontoSegurosMensual { get; set; }
        public DateTime FechaAprobacion { get; set; }
        public DateTime FechaDesembolso { get; set; }
        public DateTime FechaPrimerVencimiento { get; set; }
        public string UrlDocumento { get; set; } = null!;
        public string Estado { get; set; } = null!;
        [ValidateNever]
        public virtual SolicitudPrestamo SolicitudPrestamo { get; set; } = null!;
        public virtual ICollection<Cuota> Cuota { get; set; }
    }
}
