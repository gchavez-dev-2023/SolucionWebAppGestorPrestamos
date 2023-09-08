using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        [DisplayName("DNI")]
        public string CedulaIdentidad { get; set; } = null!;
        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Nombres")]
        public string Nombre { get; set; } = null!;
        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Apellidos")]
        public string Apellido { get; set; } = null!;

        [Required]
        [DisplayName("Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [ForeignKey("Genero")]
        [DisplayName("Sexo")]
        public int GeneroId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("Domicilio")]
        public string Domicilio { get; set; } = null!;
        [StringLength(200, MinimumLength = 3)]
        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Dirección de Correo electrónico incorrecta.")]
        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("E-Mail")]
        public string CorreoElectronico { get; set; } = null!;
        [Required]
        [DisplayName("Telefono")]
        public string Telefono { get; set; } = null!;

        [ForeignKey("Nacionalidad")]
        [DisplayName("Nacionalidad")]
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
