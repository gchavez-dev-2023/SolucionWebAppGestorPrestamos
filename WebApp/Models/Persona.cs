using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.Models
{
    public partial class Persona
    {
        public Persona()
        {
            Avales = new HashSet<Aval>();
            Clientes = new HashSet<Cliente>();
            Conyuges = new HashSet<Conyuge>();
        }

        public int Id { get; set; }
        public string CedulaIdentidad { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public int GeneroId { get; set; }
        public string Domicilio { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public int Telefono { get; set; }
        public int NacionalidadId { get; set; }
        public string UrlImagen { get; set; } = null!;
        public string UrlDocumento { get; set; } = null!;
        public bool DatosVerificados { get; set; }
        [ValidateNever]
        public virtual Genero Genero { get; set; } = null!;
        [ValidateNever]
        public virtual Nacionalidad Nacionalidad { get; set; } = null!;
        public virtual ICollection<Aval> Avales { get; set; }
        public virtual ICollection<Cliente> Clientes { get; set; }
        public virtual ICollection<Conyuge> Conyuges { get; set; }

        public static implicit operator string?(Persona? v)
        {
            throw new NotImplementedException();
        }
    }
}
