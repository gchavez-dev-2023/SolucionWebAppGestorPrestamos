using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class EstadoCivil
    {
        public EstadoCivil()
        {
            Clientes = new HashSet<Cliente>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
        public bool RequiereDatosConyuge { get; set; }

        public virtual ICollection<Cliente> Clientes { get; set; }
    }
}
