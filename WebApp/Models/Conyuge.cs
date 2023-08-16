using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Conyuge
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int PersonaId { get; set; }
        public string UrlDocumento { get; set; } = null!;
        [ValidateNever]
        public virtual Cliente Cliente { get; set; } = null!;
        [ValidateNever]
        public virtual Persona Persona { get; set; } = null!;
    }
}
