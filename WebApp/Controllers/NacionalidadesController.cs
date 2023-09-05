using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{

    [Authorize(Roles = "SUPERUSER")]
    public class NacionalidadesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NacionalidadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Nacionalidades
        public async Task<IActionResult> Index()
        {
              return _context.Nacionalidades != null ? 
                          View(await _context.Nacionalidades.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Nacionalidades'  is null.");
        }

        // GET: Nacionalidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Nacionalidades == null)
            {
                return NotFound();
            }

            var nacionalidad = await _context.Nacionalidades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nacionalidad == null)
            {
                return NotFound();
            }

            return View(nacionalidad);
        }

        // GET: Nacionalidades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nacionalidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion")] Nacionalidad nacionalidad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nacionalidad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nacionalidad);
        }

        // GET: Nacionalidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Nacionalidades == null)
            {
                return NotFound();
            }

            var nacionalidad = await _context.Nacionalidades.FindAsync(id);
            if (nacionalidad == null)
            {
                return NotFound();
            }
            return View(nacionalidad);
        }

        // POST: Nacionalidades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion")] Nacionalidad nacionalidad)
        {
            if (id != nacionalidad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nacionalidad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NacionalidadExists(nacionalidad.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nacionalidad);
        }

        // GET: Nacionalidades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Nacionalidades == null)
            {
                return NotFound();
            }

            var nacionalidad = await _context.Nacionalidades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nacionalidad == null)
            {
                return NotFound();
            }

            return View(nacionalidad);
        }

        // POST: Nacionalidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Nacionalidades == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Nacionalidades'  is null.");
            }
            var nacionalidad = await _context.Nacionalidades.FindAsync(id);
            if (nacionalidad != null)
            {
                _context.Nacionalidades.Remove(nacionalidad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NacionalidadExists(int id)
        {
          return (_context.Nacionalidades?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
