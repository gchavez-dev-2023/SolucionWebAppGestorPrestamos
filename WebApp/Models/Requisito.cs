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

        [DisplayName("Edad Minima")]
        public int EdadMinima { get; set; }

        [DisplayName("Edad Limite")]
        public int EdadMaxima { get; set; }

        [DisplayName("Scoring Minimo")]
        public int ScoringMinimo { get; set; }

        [DisplayName("Cantidad de Avales")]
        public int CantidadAvales { get; set; }

        [DisplayName("Cantidad Recibos Sueldo")]
        public int CantidadRecibosSueldo { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
