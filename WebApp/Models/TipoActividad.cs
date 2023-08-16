using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class TipoActividad
    {
        public TipoActividad()
        {
            OrigenesIngresoClientes = new HashSet<OrigenIngresoCliente>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
        public bool VerificarUif { get; set; }

        public virtual ICollection<OrigenIngresoCliente> OrigenesIngresoClientes { get; set; }
    }
}
