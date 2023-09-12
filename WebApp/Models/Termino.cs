using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApp.Models
{
    public partial class Termino
    {
        public Termino()
        {
            Productos = new HashSet<Producto>();
        }

        public int Id { get; set; }
        [DisplayName("Monto Minimo")]
        public decimal MontoMinimo { get; set; }

        [DisplayName("Monto Minimo")]
        public virtual string MontoMinimoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoMinimo); } }
        [DisplayName("Monto Maximo")]
        public decimal MontoMaximo { get; set; }
        [DisplayName("Monto Maximo")]
        public virtual string MontoMaximoDisplay { get { return String.Format(CultureInfo.GetCultureInfo("es-AR"), "{0:C}", MontoMaximo); } }
        [DisplayName("Plazo Minimo")]
        public int PlazoMinimo { get; set; }
        [DisplayName("Plazo Maximo")]
        public int PlazoMaximo { get; set; }

        [DisplayName("Tasa Nominal")]
        [DataType(DataType.Currency)]
        public decimal TasaNominal { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Nominal")]
        public virtual decimal TasaNominalPercent { get { return TasaNominal / 100; } }

        [DisplayName("Tasa Gastos Contratación")]
        public decimal TasaGastosAdministrativos { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Gastos Contratación")]
        public virtual decimal TasaGastosAdministrativosPercent { get { return TasaGastosAdministrativos / 100; } }

        [DisplayName("Tasa Gastos Admin. Mensual")]
        public decimal TasaGastosCobranza { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Gastos Admin. Mensual")]
        public virtual decimal TasaGastosCobranzaPercent { get { return TasaGastosCobranza / 100; } }

        [DisplayName("Tasa Seguros Mensual")]
        public decimal TasaSeguros { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Gastos Admin. Mensual")]
        public virtual decimal TasaSegurosPercent { get { return TasaSeguros / 100; } }

        [DisplayName("Tasa Gastos Admin. Mensual")]
        public decimal TasaInteresMora { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Gastos Admin. Mensual")]
        public virtual decimal TasaInteresMoraPercent { get { return TasaInteresMora / 100; } }

        [DisplayName("¿Se puede Pre-Pagar?")]
        public bool EsPrepagable { get; set; }

        [DisplayName("Tasa Castigo Prepago")]
        public decimal TasaCastigoPrepago { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Castigo Prepago")]
        public virtual decimal TasaCastigoPrepagoPercent { get { return TasaCastigoPrepago / 100; } }

        [DisplayName("Tasa Cobertura Aval")]
        public decimal TasaCoberturaAval { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Cobertura Aval")]
        public virtual decimal TasaCoberturaAvalPercent { get { return TasaCoberturaAval / 100; } }

        [DisplayName("Tasa Cobertura Conyuge")]
        public decimal TasaCoberturaConyuge { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        [DisplayName("Tasa Cobertura Conyuge")]
        public virtual decimal TasaCoberturaConyugePercent { get { return TasaCoberturaConyuge / 100; } }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
