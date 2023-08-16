using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            Conyuges = new HashSet<Conyuge>();
            OrigenesIngresoClientes = new HashSet<OrigenIngresoCliente>();
            SolicitudesPrestamos = new HashSet<SolicitudPrestamo>();
        }

        public int Id { get; set; }
       
        public int PersonaId { get; set; }

        public string DomicilioAlternativo { get; set; } = null!;

        public int TelefonoLaboral { get; set; }
        public bool PersonaPoliticamenteExpuesta { get; set; }
        
        public int EstadoCivilId { get; set; }
        public int Scoring { get; set; }
        public string UrlDocumento { get; set; } = null!;
        [ValidateNever]
        public virtual EstadoCivil EstadoCivil { get; set; } = null!;
        [ValidateNever]
        public virtual Persona Persona { get; set; } = null!;
        public virtual ICollection<Conyuge> Conyuges { get; set; }
        public virtual ICollection<OrigenIngresoCliente> OrigenesIngresoClientes { get; set; }
        public virtual ICollection<SolicitudPrestamo> SolicitudesPrestamos { get; set; }
    }
}
