using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Models;

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

            var users = _context.Users.Where(x => x.UserName != User.Identity.Name).ToList();
            foreach (var user in users)
            {
                var userDto = new UserDto();
                userDto.Id = user.Id;
                userDto.UserName = user.UserName;
                userDto.Email = user.Email;
                userDto.EmailConfirmed = user.EmailConfirmed;
                //userDto.PhoneNumber = user.PhoneNumber;
                userDto.RoleDto = new RoleDto();
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
                        userDto.RoleId = userRol.RoleId;
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

        public ViewResult Create()
        {
            if (User.IsInRole("ADMINISTRADOR")){
                ViewData["RoleId"] = new SelectList(_context.Roles.Where(c => c.Name != "SUPERUSER"), "Id", "Name");
            }
            else {
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            }            
            return View();
        } 

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,UserName,Email,Password,EmailConfirmed,RoleId")] UserDto userDto)
        {

            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    EmailConfirmed = userDto.EmailConfirmed,
                    PhoneNumber = userDto.PhoneNumber,
                };

                IdentityResult result = await _userManager.CreateAsync(user, userDto.Password);

                if (result.Succeeded)
                {
                    //Constuir un UsuarioRol
                    _context.Add(new IdentityUserRole<string>
                    {
                        RoleId = userDto.RoleId,
                        UserId = user.Id
                    });
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            if (User.IsInRole("ADMINISTRADOR"))
            {
                ViewData["RoleId"] = new SelectList(_context.Roles.Where(c => c.Name != "SUPERUSER"), "Id", "Name");
            }
            else
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            }
            return View(userDto);
        }

        public async Task<IActionResult> Edit(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var userDto = new UserDto();
                userDto.Id = user.Id;
                userDto.UserName = user.UserName;
                userDto.Email = user.Email;
                userDto.EmailConfirmed = user.EmailConfirmed;
                //userDto.PhoneNumber = user.PhoneNumber;
                userDto.RoleDto = new RoleDto();
                var userRol = _context.UserRoles.FirstOrDefault(p => p.UserId == user.Id);
                if (userRol != null)
                {
                    userDto.RoleDto.Id = userRol.RoleId;                    
                }

                if (User.IsInRole("ADMINISTRADOR"))
                {
                    ViewData["RoleId"] = new SelectList(_context.Roles.Where(c => c.Name != "SUPERUSER"), "Id", "Name", userRol.RoleId);
                }
                else
                {
                    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", userRol.RoleId);
                }

                return View(userDto);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id,UserName,Email,Password,EmailConfirmed,RoleId")] UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByIdAsync(userDto.Id);
                if (user != null)
                {
                    user.Email = userDto.Email;
                    user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);
                    user.EmailConfirmed = userDto.EmailConfirmed;

                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var userRol = _context.UserRoles.FirstOrDefault(p => p.UserId == user.Id);
                        if (userRol != null && userRol.RoleId != userDto.RoleId)
                        {
                            //Eliminar Rol de un usuario
                            _context.Remove(userRol);
                            await _context.SaveChangesAsync();

                            //Constuir un UsuarioRol
                            _context.Add(new IdentityUserRole<string>
                            {
                                RoleId = userDto.RoleId,
                                UserId = user.Id
                            });
                            await _context.SaveChangesAsync();
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            }

            if (User.IsInRole("ADMINISTRADOR"))
            {
                ViewData["RoleId"] = new SelectList(_context.Roles.Where(c => c.Name != "SUPERUSER"), "Id", "Name", userDto.RoleId);
            }
            else
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", userDto.RoleId);
            }

            return View(userDto);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {

            IdentityUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var userDto = new UserDto();
                userDto.Id = user.Id;
                userDto.UserName = user.UserName;
                userDto.Email = user.Email;
                userDto.EmailConfirmed = user.EmailConfirmed;
                //userDto.PhoneNumber = user.PhoneNumber;
                userDto.RoleDto = new RoleDto();
                var userRol = _context.UserRoles.FirstOrDefault(p => p.UserId == user.Id);
                if (userRol != null)
                {
                    userDto.RoleDto.Id = userRol.RoleId;
                    var rol = _context.Roles.FirstOrDefault(p => p.Id == userRol.RoleId);
                    if (rol != null)
                    {
                        userDto.RoleDto.Id = rol.Id;
                        userDto.RoleDto.Name = rol.Name;
                    }
                }

                return View(userDto);
            }
            else
            {
                return RedirectToAction("Index");
            }
        } 

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
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
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", _userManager.Users);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
