using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WebApp.Models
{
    public partial class Termino
    {
        public Termino()
        {
            Productos = new HashSet<Producto>();
        }

        public int Id { get; set; }
        public decimal MontoMinimo { get; set; }
        public decimal MontoMaximo { get; set; }
        public int PlazoMinimo { get; set; }
        public int PlazoMaximo { get; set; }
        [DisplayName("Tasa Nominal")]
        public decimal TasaNominal { get; set; }
        public decimal TasaGastosAdministrativos { get; set; }
        public decimal TasaGastosCobranza { get; set; }
        public decimal TasaSeguros { get; set; }
        public decimal TasaInteresMora { get; set; }
        public bool EsPrepagable { get; set; }
        public decimal TasaCastigoPrepago { get; set; }
        public decimal TasaCoberturaAval { get; set; }
        public decimal TasaCoberturaConyuge { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
