using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApp.Models
{
    public partial class Producto
    {
        public Producto()
        {
            SolicitudesPrestamos = new HashSet<SolicitudPrestamo>();
        }

        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; } = null!;
        public int RequisitosId { get; set; }
        public int BeneficiosId { get; set; }
        public int TerminosId { get; set; }
        [Required]
        [DisplayName("Fecha Inicio Vigencia")]
        [DataType(DataType.Date)]
        public DateTime FechaInicioVigencia { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha Inicio Vigencia")]
        public virtual DateTime FechaInicioVigenciaDisplay { get { return FechaInicioVigencia; } }

        [Required]
        [DisplayName("Fecha Fin Vigencia")]
        [DataType(DataType.Date)]
        public DateTime FechaFinVigencia { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha Fin Vigencia")]
        public virtual DateTime FechaFinVigenciaDisplay { get { return FechaFinVigencia; } }

        [ValidateNever]
        public virtual Beneficio Beneficios { get; set; } = null!;
        [ValidateNever]
        public virtual Requisito Requisitos { get; set; } = null!;
        [ValidateNever]
        public virtual Termino Terminos { get; set; } = null!;
        public virtual ICollection<SolicitudPrestamo> SolicitudesPrestamos { get; set; }
    }
}
