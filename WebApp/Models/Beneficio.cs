using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WebApp.Models
{
    public partial class Beneficio
    {
        public Beneficio()
        {
            Productos = new HashSet<Producto>();
        }

        public int Id { get; set; }

        [DisplayName("¿Posee Aprobación 24 Horas?")]
        public bool Aprobacion24Horas { get; set; }

        [DisplayName("¿Se puede solicitar en linea?")]
        public bool SolicitudEnLinea { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
