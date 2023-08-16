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
    public class TerminosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerminosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Terminos
        public async Task<IActionResult> Index()
        {
              return _context.Terminos != null ? 
                          View(await _context.Terminos.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Terminos'  is null.");
        }

        // GET: Terminos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Terminos == null)
            {
                return NotFound();
            }

            var termino = await _context.Terminos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (termino == null)
            {
                return NotFound();
            }

            return View(termino);
        }

        // GET: Terminos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Terminos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MontoMinimo,MontoMaximo,PlazoMinimo,PlazoMaximo,TasaNominal,TasaGastosAdministrativos,TasaGastosCobranza,TasaSeguros,TasaInteresMora,EsPrepagable,TasaCastigoPrepago,TasaCoberturaAval,TasaCoberturaConyuge")] Termino termino)
        {
            if (ModelState.IsValid)
            {
                _context.Add(termino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(termino);
        }

        // GET: Terminos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Terminos == null)
            {
                return NotFound();
            }

            var termino = await _context.Terminos.FindAsync(id);
            if (termino == null)
            {
                return NotFound();
            }
            return View(termino);
        }

        // POST: Terminos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MontoMinimo,MontoMaximo,PlazoMinimo,PlazoMaximo,TasaNominal,TasaGastosAdministrativos,TasaGastosCobranza,TasaSeguros,TasaInteresMora,EsPrepagable,TasaCastigoPrepago,TasaCoberturaAval,TasaCoberturaConyuge")] Termino termino)
        {
            if (id != termino.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(termino);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerminoExists(termino.Id))
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
            return View(termino);
        }

        // GET: Terminos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Terminos == null)
            {
                return NotFound();
            }

            var termino = await _context.Terminos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (termino == null)
            {
                return NotFound();
            }

            return View(termino);
        }

        // POST: Terminos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Terminos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Terminos'  is null.");
            }
            var termino = await _context.Terminos.FindAsync(id);
            if (termino != null)
            {
                _context.Terminos.Remove(termino);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TerminoExists(int id)
        {
          return (_context.Terminos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
