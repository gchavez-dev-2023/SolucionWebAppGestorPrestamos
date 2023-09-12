using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [DisplayName("Domicilio Alternativo")]
        public string DomicilioAlternativo { get; set; } = null!;

        [DisplayName("Telefono Laboral")]
        public string TelefonoLaboral { get; set; }

        [DisplayName("Persona Politicamente Expuesta")]
        public bool PersonaPoliticamenteExpuesta { get; set; }

        [DisplayName("Estado Civil")]
        public int EstadoCivilId { get; set; }

        [DisplayName("Scoring")]
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
