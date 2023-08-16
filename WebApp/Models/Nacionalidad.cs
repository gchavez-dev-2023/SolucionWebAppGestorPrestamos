namespace WebApp.Models
{
    public partial class Nacionalidad
    {

        public Nacionalidad()
        {
            Personas = new HashSet<Persona>();
        }
        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<Persona> Personas { get; set; }
    }        
}
