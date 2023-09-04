using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApp.Dtos
{
    public class AvalDto
    {
        public int Id { get; set; }
        //Datos Persona-Aval
        public int PersonaId { get; set; }
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

        [StringLength(200, MinimumLength = 3)]
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
        public int Telefono { get; set; }

        [ForeignKey("Nacionalidad")]
        [DisplayName("Nacionalidad")]
        [Column(Order = 1)]
        public int NacionalidadId { get; set; }
        //
        public decimal TasaCoberturaDeuda { get; set; }
        public string UrlDocumento { get; set; } = null!;
    }
}
