using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class SolicitudPrestamo
    {
        public SolicitudPrestamo()
        {
            Avales = new HashSet<Aval>();
            Documentos = new HashSet<Documento>();
            PrestamosAprobados = new HashSet<PrestamoAprobado>();
        }

        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ProductoId { get; set; }
        public decimal MontoSolicitado { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal CostoTotalFinanciero { get; set; }
        public decimal TasaCoberturaDeudaConyuge { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string UrlDocumento { get; set; } = null!;
        public string Estado { get; set; } = null!;
        [ValidateNever]
        public virtual Cliente Cliente { get; set; } = null!;
        [ValidateNever]
        public virtual Producto Producto { get; set; } = null!;
        public virtual ICollection<Aval> Avales { get; set; }
        public virtual ICollection<Documento> Documentos { get; set; }
        public virtual ICollection<PrestamoAprobado> PrestamosAprobados { get; set; }
    }
}
