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
    public class RequisitosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequisitosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requisitos
        public async Task<IActionResult> Index()
        {
              return _context.Requisitos != null ? 
                          View(await _context.Requisitos.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Requisitos'  is null.");
        }

        // GET: Requisitos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Requisitos == null)
            {
                return NotFound();
            }

            var requisito = await _context.Requisitos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requisito == null)
            {
                return NotFound();
            }

            return View(requisito);
        }

        // GET: Requisitos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Requisitos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EdadMinima,EdadMaxima,ScoringMinimo,CantidadAvales,CantidadRecibosSueldo")] Requisito requisito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requisito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requisito);
        }

        // GET: Requisitos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Requisitos == null)
            {
                return NotFound();
            }

            var requisito = await _context.Requisitos.FindAsync(id);
            if (requisito == null)
            {
                return NotFound();
            }
            return View(requisito);
        }

        // POST: Requisitos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EdadMinima,EdadMaxima,ScoringMinimo,CantidadAvales,CantidadRecibosSueldo")] Requisito requisito)
        {
            if (id != requisito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requisito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequisitoExists(requisito.Id))
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
            return View(requisito);
        }

        // GET: Requisitos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Requisitos == null)
            {
                return NotFound();
            }

            var requisito = await _context.Requisitos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requisito == null)
            {
                return NotFound();
            }

            return View(requisito);
        }

        // POST: Requisitos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Requisitos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Requisitos'  is null.");
            }
            var requisito = await _context.Requisitos.FindAsync(id);
            if (requisito != null)
            {
                _context.Requisitos.Remove(requisito);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequisitoExists(int id)
        {
          return (_context.Requisitos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
