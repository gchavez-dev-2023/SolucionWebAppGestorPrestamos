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
    public class TiposDocumentoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TiposDocumentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TiposDocumento
        public async Task<IActionResult> Index()
        {
              return _context.TiposDocumento != null ? 
                          View(await _context.TiposDocumento.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TiposDocumento'  is null.");
        }

        // GET: TiposDocumento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TiposDocumento == null)
            {
                return NotFound();
            }

            var tipoDocumento = await _context.TiposDocumento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDocumento == null)
            {
                return NotFound();
            }

            return View(tipoDocumento);
        }

        // GET: TiposDocumento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposDocumento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion")] TipoDocumento tipoDocumento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoDocumento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDocumento);
        }

        // GET: TiposDocumento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TiposDocumento == null)
            {
                return NotFound();
            }

            var tipoDocumento = await _context.TiposDocumento.FindAsync(id);
            if (tipoDocumento == null)
            {
                return NotFound();
            }
            return View(tipoDocumento);
        }

        // POST: TiposDocumento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion")] TipoDocumento tipoDocumento)
        {
            if (id != tipoDocumento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoDocumento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoDocumentoExists(tipoDocumento.Id))
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
            return View(tipoDocumento);
        }

        // GET: TiposDocumento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TiposDocumento == null)
            {
                return NotFound();
            }

            var tipoDocumento = await _context.TiposDocumento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDocumento == null)
            {
                return NotFound();
            }

            return View(tipoDocumento);
        }

        // POST: TiposDocumento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TiposDocumento == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TiposDocumento'  is null.");
            }
            var tipoDocumento = await _context.TiposDocumento.FindAsync(id);
            if (tipoDocumento != null)
            {
                _context.TiposDocumento.Remove(tipoDocumento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoDocumentoExists(int id)
        {
          return (_context.TiposDocumento?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
