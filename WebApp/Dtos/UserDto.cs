using System.ComponentModel.DataAnnotations;

namespace WebApp.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public RoleDto RoleDto { get; set; }

    }
}
