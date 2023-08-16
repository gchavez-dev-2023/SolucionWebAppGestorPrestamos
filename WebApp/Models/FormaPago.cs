using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class FormaPago
    {
        public FormaPago()
        {
            Pagos = new HashSet<Pago>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<Pago> Pagos { get; set; }
    }
}
