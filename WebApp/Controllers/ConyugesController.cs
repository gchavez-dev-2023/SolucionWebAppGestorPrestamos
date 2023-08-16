using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ConyugesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConyugesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Conyuges
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Conyuges.Include(c => c.Cliente).Include(c => c.Persona);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Conyuges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Conyuges == null)
            {
                return NotFound();
            }

            var conyuge = await _context.Conyuges
                .Include(c => c.Cliente)
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conyuge == null)
            {
                return NotFound();
            }

            return View(conyuge);
        }

        // GET: Conyuges/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Id");
            return View();
        }

        // POST: Conyuges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,PersonaId,UrlDocumento")] Conyuge conyuge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conyuge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", conyuge.ClienteId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Id", conyuge.PersonaId);
            return View(conyuge);
        }

        // GET: Conyuges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Conyuges == null)
            {
                return NotFound();
            }

            var conyuge = await _context.Conyuges.FindAsync(id);
            if (conyuge == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", conyuge.ClienteId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Id", conyuge.PersonaId);
            return View(conyuge);
        }

        // POST: Conyuges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,PersonaId,UrlDocumento")] Conyuge conyuge)
        {
            if (id != conyuge.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conyuge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConyugeExists(conyuge.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", conyuge.ClienteId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "Id", conyuge.PersonaId);
            return View(conyuge);
        }

        // GET: Conyuges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Conyuges == null)
            {
                return NotFound();
            }

            var conyuge = await _context.Conyuges
                .Include(c => c.Cliente)
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conyuge == null)
            {
                return NotFound();
            }

            return View(conyuge);
        }

        // POST: Conyuges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Conyuges == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Conyuges'  is null.");
            }
            var conyuge = await _context.Conyuges.FindAsync(id);
            if (conyuge != null)
            {
                _context.Conyuges.Remove(conyuge);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConyugeExists(int id)
        {
          return (_context.Conyuges?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
