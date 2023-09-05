using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Dtos
{
    public class ClienteDto
    {
        public int Id { get; set; }

        //Datos Persona-Cliente
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
        public int Edad { get; set; }

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

        [StringLength(200, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("Domicilio Alternativo")]
        public string DomicilioAlternativo { get; set; } = null!;
        [Required]
        [DisplayName("Telefono Laboral")]
        public int TelefonoLaboral { get; set; }
        [Required]
        [Column(TypeName = "bool")]
        [DisplayName("Persona Politicamente Expuesta")]
        public bool PersonaPoliticamenteExpuesta { get; set; }

        //Origen de ingresos
        public int OrigenIngresoId { get; set; }
        [ForeignKey("TipoActividad")]
        [DisplayName("Tipo Actividad Laboral")]
        [Column(Order = 1)]
        public int TipoActividadId { get; set; }
        [Required]
        [DisplayName("Fecha de Inicio Actividad Laboral")]
        [DataType(DataType.Date)]
        public DateTime FechaInicioActividad { get; set; }
        [Required]
        [DisplayName("Fecha de Fin Actividad Laboral")]
        [DataType(DataType.Date)]
        public DateTime FechaFinActividad { get; set; }
        [Required]
        [DisplayName("Monto Liquido Percibido Actividad Laboral")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal MontoLiquidoPercibido { get; set; }
        //

        [ForeignKey("EstadoCivil")]
        [DisplayName("Estado Civil")]
        public int EstadoCivilId { get; set; }

        //Datos Persona-Conyuge
        public int ConyugeId { get; set; }
        public int ConyugePersonaId { get; set; }
        [Required]
        [DisplayName("DNI")]
        public string ConyugeCedulaIdentidad { get; set; } = null!;

        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Nombres")]
        public string ConyugeNombre { get; set; } = null!;
        [StringLength(100, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Apellidos")]
        public string ConyugeApellido { get; set; } = null!;

        [Required]
        [DisplayName("Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime ConyugeFechaNacimiento { get; set; }
        [ForeignKey("Genero")]
        [DisplayName("Sexo")]
        public int ConyugeGeneroId { get; set; }
        [StringLength(200, MinimumLength = 3)]
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("Domicilio")]
        public string ConyugeDomicilio { get; set; } = null!;
        [StringLength(200, MinimumLength = 3)]
        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Dirección de Correo electrónico incorrecta.")]
        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("E-Mail")]
        public string ConyugeCorreoElectronico { get; set; } = null!;
        [Required]
        [DisplayName("Telefono")]
        public int ConyugeTelefono { get; set; }
        [ForeignKey("Nacionalidad")]
        [DisplayName("Nacionalidad")]
        [Column(Order = 1)]
        public int ConyugeNacionalidadId { get; set; }
        //
        public int Scoring { get; set; }
    }
}
