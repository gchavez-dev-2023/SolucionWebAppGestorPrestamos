using System.ComponentModel.DataAnnotations;

namespace WebApp.Dtos
{
    public class RoleModificationDto
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }

        public string[]? AddIds { get; set; }

        public string[]? DeleteIds { get; set; }
    }
}
