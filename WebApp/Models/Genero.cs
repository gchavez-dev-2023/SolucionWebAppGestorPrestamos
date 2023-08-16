using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Genero
    {
        public Genero()
        {
            Personas = new HashSet<Persona>();
        }
        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<Persona> Personas { get; set; }
    }
}
