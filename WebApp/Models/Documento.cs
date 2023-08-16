using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Documento
    {
        public int Id { get; set; }
        public int SolicitudPrestamoId { get; set; }
        public int TipoDocumentoId { get; set; }
        public string UrlDocumento { get; set; } = null!;
        [ValidateNever]
        public virtual SolicitudPrestamo SolicitudPrestamo { get; set; } = null!;
        [ValidateNever]
        public virtual TipoDocumento TipoDocumento { get; set; } = null!;
    }
}
