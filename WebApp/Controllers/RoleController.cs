using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private RoleManager<IdentityRole> roleManager;
        private UserManager<IdentityUser> userManager;
        public RoleController(ApplicationDbContext context, RoleManager<IdentityRole> roleMgr, UserManager<IdentityUser> userMrg)
        {
            _context = context;
            roleManager = roleMgr;
            userManager = userMrg;
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
            return View("Index", roleManager.Roles);

            /*IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }                    
                else
                {
                    Errors(result);
                }                    
            }
            else
            {
                ModelState.AddModelError("", "No role found");
            }

            return View("Index", roleManager.Roles);*/
        }

        public async Task<IActionResult> Update(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            var users = await _context.Users.ToListAsync();
            //IdentityRole role = await roleManager.FindByIdAsync(id);
            List<IdentityUser> members = new List<IdentityUser>();
            List<IdentityUser> nonMembers = new List<IdentityUser>();
            //foreach (IdentityUser user in userManager.Users)
            foreach (IdentityUser user in users)
            {
                //var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                var list = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModification model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.AddIds ?? new string[] { })
                {
                    //var user = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
                    IdentityUser user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        /*user.RoleId = model.RoleName;

                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        */
                        
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            Errors(result);
                        }       
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { })
                {
                    IdentityUser user = await userManager.FindByIdAsync(userId);
                    //var user = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
                    if (user != null)
                    {
                        /*_context.UserRoles.Remove(user);
                         await _context.SaveChangesAsync();
                        */
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
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
                return await Update(model.RoleId);
            }                
        }
    }
}
