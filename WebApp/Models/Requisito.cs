using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WebApp.Models
{
    public partial class Requisito
    {
        public Requisito()
        {
            Productos = new HashSet<Producto>();
        }

        public int Id { get; set; }
        public int EdadMinima { get; set; }
        public int EdadMaxima { get; set; }

        [DisplayName("Scoring Minimo")]
        public int ScoringMinimo { get; set; }
        public int CantidadAvales { get; set; }
        public int CantidadRecibosSueldo { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
