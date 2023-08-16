using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Producto
    {
        public Producto()
        {
            SolicitudesPrestamos = new HashSet<SolicitudPrestamo>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
        public int RequisitosId { get; set; }
        public int BeneficiosId { get; set; }
        public int TerminosId { get; set; }
        public DateTime FechaInicioVigencia { get; set; }
        public DateTime FechaFinVigencia { get; set; }
        [ValidateNever]
        public virtual Beneficio Beneficios { get; set; } = null!;
        [ValidateNever]
        public virtual Requisito Requisitos { get; set; } = null!;
        [ValidateNever]
        public virtual Termino Terminos { get; set; } = null!;
        public virtual ICollection<SolicitudPrestamo> SolicitudesPrestamos { get; set; }
    }
}
