using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar nombre valido de 3 a 50 caracteres.")]
        [StringLength(50, MinimumLength = 3)]
        [DisplayName("Nombre de usuario")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Debe ingresar E-Mail.")]
        [StringLength(200, MinimumLength = 3)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Dirección de Correo electrónico incorrecta.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar contraseña valida")]
        [StringLength(50, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$",
            ErrorMessage = "Contraseña debe tener al menos 8 caracteres. Al menos 1 letra mayuscula, 1 letra minuscula, 1 numero y 1 caracter especial.")]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [Required]
        [DisplayName("¿E-Mail Confirmado?")]
        public bool EmailConfirmed { get; set; }

        public string? PhoneNumber { get; set; }

        [DisplayName("Rol")]
        public string? RoleId { get; set; }

        [ValidateNever]
        public virtual RoleDto? RoleDto { get; set; }

    }
}
