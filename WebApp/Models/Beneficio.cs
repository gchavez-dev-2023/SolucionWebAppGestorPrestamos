using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Beneficio
    {
        public Beneficio()
        {
            Productos = new HashSet<Producto>();
        }

        public int Id { get; set; }
        public bool Aprobacion24Horas { get; set; }
        public bool SolicitudEnLinea { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
