using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Dtos
{
    public class ClienteDto
    {

        [DisplayName("Cliente")]
        public int Id { get; set; }

        //Datos Persona-Cliente
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

        [Required(ErrorMessage = "Debe ingresar fecha valida, menor a la fecha del día.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }
        public int Edad { get; set; }

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
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Numero no valido")]
        [DisplayName("Telefono")]
        public string Telefono { get; set; } = null!;

        [Required(ErrorMessage = "Debe seleccionar Nacionalidad.")]
        [ForeignKey("Nacionalidad")]
        [DisplayName("Nacionalidad")]
        [Column(Order = 1)]
        public int NacionalidadId { get; set; }
        //

        [Required(ErrorMessage = "Debe ingresar Domicilio Alternativo valido de 3 a 200 caracteres.")]
        [StringLength(200, MinimumLength = 3)]
        [DisplayName("Domicilio Alternativo")]
        public string DomicilioAlternativo { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Numero Telefono.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Numero no valido")]
        [DisplayName("Telefono Laboral")]
        public string TelefonoLaboral { get; set; }

        [Required(ErrorMessage = "Debe indicar si es PEP.")]
        [Column(TypeName = "bool")]
        [DisplayName("Persona Politicamente Expuesta")]
        public bool PersonaPoliticamenteExpuesta { get; set; }

        //Origen de ingresos
        public int OrigenIngresoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar Tipo Actividad Laboral.")]
        [ForeignKey("TipoActividad")]
        [DisplayName("Tipo Actividad Laboral")]
        [Column(Order = 1)]
        public int TipoActividadId { get; set; }

        [Required(ErrorMessage = "Debe ingresar fecha valida, menor a la fecha del día.")]
        [DisplayName("Fecha de Inicio Actividad Laboral")]
        [DataType(DataType.Date)]
        public DateTime FechaInicioActividad { get; set; }

        [Required(ErrorMessage = "Debe ingresar fecha valida, menor a la Fecha de Inicio Actividad Laboral.")]
        [DisplayName("Fecha de Fin Actividad Laboral")]
        [DataType(DataType.Date)]
        public DateTime FechaFinActividad { get; set; }

        [Required(ErrorMessage = "Debe ingresar monto, valores permitidos entre 10.000,00 y 10.000.000,00")]
        [Range(10000, 100000000)]
        [DisplayName("Monto Liquido Percibido Actividad Laboral")]
        [DataType(DataType.Currency)]
        public decimal MontoLiquidoPercibido { get; set; }

        [Required(ErrorMessage = "Debe ingresar cantidad, valores permitidos entre 0 y 100")]
        [Range(0, 100)]
        public int Scoring { get; set; }
        ///
        [Required(ErrorMessage = "Debe seleccionar Estado Civil.")]
        [ForeignKey("EstadoCivil")]
        [DisplayName("Estado Civil")]
        public int EstadoCivilId { get; set; }
        public bool RequiereDatosConyuge { get; set; }

        //Datos Persona-Conyuge
        public int ConyugeId { get; set; }
        public int ConyugePersonaId { get; set; }

        [Required(ErrorMessage = "Debe ingresar un DNI valido de 7 a 10 caracteres.")]
        [StringLength(10, MinimumLength = 7)]
        [DisplayName("DNI")]
        public string ConyugeCedulaIdentidad { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Nombres validos de 3 a 100 caracteres.")]
        [StringLength(100, MinimumLength = 3)]
        [DisplayName("Nombres")]
        public string ConyugeNombre { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Apellidos validos de 3 a 100 caracteres.")]
        [StringLength(100, MinimumLength = 3)]
        [DisplayName("Apellidos")]
        public string ConyugeApellido { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar Fecha Valida, menor a la fecha del día.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha de Nacimiento")]
        public DateTime ConyugeFechaNacimiento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar Genero.")]
        [ForeignKey("Genero")]
        [DisplayName("Sexo")]
        public int ConyugeGeneroId { get; set; }

        [Required(ErrorMessage = "Debe ingresar Domicilio valido de 3 a 200 caracteres.")]
        [StringLength(200, MinimumLength = 3)]
        [DisplayName("Domicilio")]
        public string ConyugeDomicilio { get; set; } = null!;

        [Required(ErrorMessage = "Debe ingresar E-Mail.")]
        [StringLength(200, MinimumLength = 3)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Dirección de Correo electrónico incorrecta.")]
        [DisplayName("E-Mail")]
        public string ConyugeCorreoElectronico { get; set; } = null!;

        //cambiar a String
        [Required(ErrorMessage = "Debe ingresar Numero Telefono.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Numero no valido")]
        [DisplayName("Telefono")]
        public string ConyugeTelefono { get; set; } = null!;

        [Required(ErrorMessage = "Debe seleccionar Nacionalidad.")]
        [ForeignKey("Nacionalidad")]
        [DisplayName("Nacionalidad")]
        [Column(Order = 1)]
        public int ConyugeNacionalidadId { get; set; }
        //
    }
}
