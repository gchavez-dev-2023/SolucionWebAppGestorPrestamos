using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class TipoDocumento
    {
        public TipoDocumento()
        {
            Documentos = new HashSet<Documento>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<Documento> Documentos { get; set; }
    }
}
