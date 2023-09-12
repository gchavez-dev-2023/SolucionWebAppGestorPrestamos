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

        [Required(ErrorMessage = "Debe ingresar un DNI valido de 7 a 10 caracteres.")]
        [StringLength(10, MinimumLength = 7)]
        [DisplayName("DNI")]
        public string CedulaIdentidad { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Nombres validos de 3 a 100 caracteres.")]
        [StringLength(100, MinimumLength = 3)]
        [DisplayName("Nombres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Apellidos validos de 3 a 100 caracteres.")]
        [StringLength(100, MinimumLength = 3)]
        [DisplayName("Apellidos")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Fecha Valida, menor a la fecha del día.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Fecha de Nacimiento")]
        public virtual DateTime FechaNacimientoDisplay { get { return FechaNacimiento; } }

        [Required(ErrorMessage = "Debe seleccionar Genero.")]
        [ForeignKey("Genero")]
        [DisplayName("Sexo")]
        public int GeneroId { get; set; }

        [Required(ErrorMessage = "Debe ingresar Domicilio valido de 3 a 200 caracteres.")]
        [StringLength(200, MinimumLength = 3)]
        [DisplayName("Domicilio")]
        public string Domicilio { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar E-Mail.")]
        [StringLength(200, MinimumLength = 3)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Dirección de Correo electrónico incorrecta.")]
        [DisplayName("E-Mail")]
        public string CorreoElectronico { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Numero Telefono.")]
        [RegularExpression(@"^(\+)?[9876543210]\d{10}$", ErrorMessage = "Numero no valido, formato +56998761234")]
        [DisplayName("Telefono")]
        public string Telefono { get; set; } = null!;

        [Required(ErrorMessage = "Debe seleccionar Nacionalidad.")]
        [ForeignKey("Nacionalidad")]
        [DisplayName("Nacionalidad")]
        [Column(Order = 1)]
        public int NacionalidadId { get; set; }
        //
        public decimal TasaCoberturaDeuda { get; set; }
        public string UrlDocumento { get; set; } = null!;
    }
}
