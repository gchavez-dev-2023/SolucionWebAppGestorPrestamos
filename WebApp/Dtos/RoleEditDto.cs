using Microsoft.AspNetCore.Identity;

namespace WebApp.Dtos
{
    public class RoleEditDto
    {

        public IdentityRole Role { get; set; }
        public IEnumerable<IdentityUser> Members { get; set; }
        public IEnumerable<IdentityUser> NonMembers { get; set; }
    }
}
