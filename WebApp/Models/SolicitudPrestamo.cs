using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        [DisplayName("Monto Solicitado")]
        [DataType(DataType.Currency)]
        public decimal MontoSolicitado { get; set; }

        [DisplayName("Cantidad Cuotas")]
        public int CantidadCuotas { get; set; }

        [DisplayName("Monto Cuota")]
        [DataType(DataType.Currency)]
        public decimal ValorCuota { get; set; }

        [DisplayName("Costo Total Financiero")]
        [DataType(DataType.Currency)]
        public decimal CostoTotalFinanciero { get; set; }
        public decimal TasaCoberturaDeudaConyuge { get; set; }

        [DisplayName("Fecha de Solicitud")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSolicitud { get; set; }
        public string UrlDocumento { get; set; } = null!;
        [DisplayName("Estado Solicitud")]
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
