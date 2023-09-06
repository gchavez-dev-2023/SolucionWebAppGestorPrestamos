using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApp.Data;
using WebApp.Dtos;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SUPERUSER,ADMINISTRADOR")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPasswordHasher<IdentityUser> _passwordHasher;
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IPasswordHasher<IdentityUser> passwordHasher)
        {
            _userManager = userManager;
            _context = context;
            _passwordHasher = passwordHasher;
        }
        public IActionResult Index()
        {
            var usersDto = new List<UserDto>();

            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                var userDto = new UserDto();
                userDto.Id = user.Id;
                userDto.UserName = user.UserName;
                userDto.Email = user.Email;
                userDto.EmailConfirmed = user.EmailConfirmed;
                userDto.PhoneNumber = user.PhoneNumber;
                var userRol = _context.UserRoles.FirstOrDefault(p => p.UserId == user.Id);
                bool usuarioPermitido = true;
                if (userRol != null)
                {
                    var rol = _context.Roles.FirstOrDefault(p => p.Id == userRol.RoleId);
                    if (User.IsInRole("ADMINISTRADOR") && rol.Name == "SUPERUSER")
                    {
                        usuarioPermitido = false;
                    }
                    else
                    {
                        userDto.RoleDto = new RoleDto();
                        userDto.RoleDto.Id = rol.Id;
                        userDto.RoleDto.Name = rol.Name;
                    }
                }
                if (usuarioPermitido)
                {
                    usersDto.Add(userDto);
                }
            }

            return View(usersDto);
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserDto user)
        {
            if (ModelState.IsValid)
            {
                IdentityUser appUser = new IdentityUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password, bool emailConfirmed)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                {
                    user.Email = email;
                }
                else
                {
                    ModelState.AddModelError("", "Email cannot be empty");
                }

                if (!string.IsNullOrEmpty(password))
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, password);
                }
                else
                {
                    ModelState.AddModelError("", "Password cannot be empty");
                }

                user.EmailConfirmed = emailConfirmed;

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", _userManager.Users);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
