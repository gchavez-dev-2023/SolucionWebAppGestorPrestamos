using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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

        [DisplayName("Solicitud")]
        public int Id { get; set; }
        [DisplayName("Cedula Identidad Cliente")]
        public int ClienteId { get; set; }

        [DisplayName("Nombre Producto")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 10.000,00 y 100.000.000,00")]
        [Range(10000, 1000000000)]
        [DataType(DataType.Currency)]
        [DisplayName("Monto Solicitado")]
        public decimal MontoSolicitado { get; set; }
        [DisplayName("Monto Solicitado")]
        public virtual string MontoSolicitadoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoSolicitado); } }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 600")]
        [Range(0, 600)]
        [DisplayName("Cantidad Cuotas")]
        public int CantidadCuotas { get; set; }

        [DisplayName("Monto Cuota")]
        [DataType(DataType.Currency)]
        public decimal ValorCuota { get; set; }
        [DisplayName("Monto Cuota")]
        public virtual string ValorCuotaDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", ValorCuota); } }


        [DisplayName("Costo Total Financiero")]
        public decimal CostoTotalFinanciero { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Costo Total Financiero")]
        public virtual decimal CostoTotalFinancieroPercent { get { return CostoTotalFinanciero / 100; } }
        public decimal TasaCoberturaDeudaConyuge { get; set; }

        [DisplayName("Fecha de Solicitud")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSolicitud { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de Solicitud")]
        public virtual DateTime FechaSolicitudDisplay { get { return FechaSolicitud; } }
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
