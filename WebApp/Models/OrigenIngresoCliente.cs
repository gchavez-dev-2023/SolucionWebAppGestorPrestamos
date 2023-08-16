using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class OrigenIngresoCliente
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int TipoActividadId { get; set; }
        public DateTime FechaInicioActividad { get; set; }
        public DateTime FechaFinActividad { get; set; }
        public decimal MontoLiquidoPercibido { get; set; }
        public string UrlDocumento { get; set; } = null!;
        [ValidateNever]
        public virtual Cliente Cliente { get; set; } = null!;
        [ValidateNever]
        public virtual TipoActividad TipoActividad { get; set; } = null!;
    }
}
