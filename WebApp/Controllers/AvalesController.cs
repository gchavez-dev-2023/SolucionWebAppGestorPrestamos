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
    public class AvalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Avales
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Avales.Include(a => a.Persona).Include(a => a.SolicitudPrestamo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Avales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Avales == null)
            {
                return NotFound();
            }

            var aval = await _context.Avales
                .Include(a => a.Persona)
                .Include(a => a.SolicitudPrestamo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aval == null)
            {
                return NotFound();
            }

            return View(aval);
        }

        // GET: Avales/Create
        public IActionResult Create()
        {
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad");
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id");
            return View();
        }

        // POST: Avales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SolicitudPrestamoId,PersonaId,TasaCoberturaDeuda,UrlDocumento")] Aval aval)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aval);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", aval.PersonaId);
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", aval.SolicitudPrestamoId);
            return View(aval);
        }

        // GET: Avales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Avales == null)
            {
                return NotFound();
            }

            var aval = await _context.Avales.FindAsync(id);
            if (aval == null)
            {
                return NotFound();
            }
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", aval.PersonaId);
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", aval.SolicitudPrestamoId);
            return View(aval);
        }

        // POST: Avales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SolicitudPrestamoId,PersonaId,TasaCoberturaDeuda,UrlDocumento")] Aval aval)
        {
            if (id != aval.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aval);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvalExists(aval.Id))
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
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", aval.PersonaId);
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", aval.SolicitudPrestamoId);
            return View(aval);
        }

        // GET: Avales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Avales == null)
            {
                return NotFound();
            }

            var aval = await _context.Avales
                .Include(a => a.Persona)
                .Include(a => a.SolicitudPrestamo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aval == null)
            {
                return NotFound();
            }

            return View(aval);
        }

        // POST: Avales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Avales == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Avales'  is null.");
            }
            var aval = await _context.Avales.FindAsync(id);
            if (aval != null)
            {
                _context.Avales.Remove(aval);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvalExists(int id)
        {
          return (_context.Avales?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
