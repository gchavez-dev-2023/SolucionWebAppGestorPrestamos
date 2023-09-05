using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Linq;
using WebApp.Data;
using WebApp.Dtos;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SUPERUSER,ADMINISTRADOR")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public RoleController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //public ViewResult Index() => View(roleManager.Roles);
        public ViewResult Index() => View(_context.Roles);

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }                
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                //Cambiar por modelo nuevo, con patron singelton
                _context.Add(new IdentityRole(name));
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

                //IdentityResult result = await _context.Roles.CreateAsync(new IdentityRole(name));
                /*if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }                    
                else
                {
                    Errors(result);
                }*/                    
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            
            //Se reemplaza por nuevo forma
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "No role found");
            }
            return View("Index", _roleManager.Roles);

        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            var users = await _context.Users.ToListAsync();

            List<IdentityUser> members = new List<IdentityUser>();
            List<IdentityUser> nonMembers = new List<IdentityUser>();

            foreach (IdentityUser user in users)
            {
                var list = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEditDto
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleModificationDto model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    bool existeUsuarioRol = await _context.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == userId 
                                          && ur.RoleId == model.RoleId) == null ? false : true;
                    if (!existeUsuarioRol)
                    {
                        var user = await _context.Users.FirstOrDefaultAsync(ur => ur.Id == userId);
                        if (user != null)
                        {
                            var rol = await _context.Roles.FirstOrDefaultAsync(ur => ur.Id == model.RoleId);
                            if (rol != null)
                            {
                                //Constuir un UsuarioRol
                                _context.Add(new IdentityUserRole<string>
                                {
                                    RoleId = rol.Id,
                                    UserId = user.Id
                                });

                                await _context.SaveChangesAsync();

                            }
                        }

                    }
                }

                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    IdentityUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            Errors(result);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return await Edit(model.RoleId);
            }
        }
    }
}
