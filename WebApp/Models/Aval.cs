using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Aval
    {
        public int Id { get; set; }
        public int SolicitudPrestamoId { get; set; }
        public int PersonaId { get; set; }
        public decimal TasaCoberturaDeuda { get; set; }
        public string UrlDocumento { get; set; } = null!;
        [ValidateNever]
        public virtual Persona Persona { get; set; } = null!;
        [ValidateNever]
        public virtual SolicitudPrestamo SolicitudPrestamo { get; set; } = null!;
    }
}
