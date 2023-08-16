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
    public class TiposActividadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TiposActividadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TiposActividad
        public async Task<IActionResult> Index()
        {
              return _context.TiposActividad != null ? 
                          View(await _context.TiposActividad.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TiposActividad'  is null.");
        }

        // GET: TiposActividad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TiposActividad == null)
            {
                return NotFound();
            }

            var tipoActividad = await _context.TiposActividad
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoActividad == null)
            {
                return NotFound();
            }

            return View(tipoActividad);
        }

        // GET: TiposActividad/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposActividad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,VerificarUif")] TipoActividad tipoActividad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoActividad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoActividad);
        }

        // GET: TiposActividad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TiposActividad == null)
            {
                return NotFound();
            }

            var tipoActividad = await _context.TiposActividad.FindAsync(id);
            if (tipoActividad == null)
            {
                return NotFound();
            }
            return View(tipoActividad);
        }

        // POST: TiposActividad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,VerificarUif")] TipoActividad tipoActividad)
        {
            if (id != tipoActividad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoActividad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoActividadExists(tipoActividad.Id))
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
            return View(tipoActividad);
        }

        // GET: TiposActividad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TiposActividad == null)
            {
                return NotFound();
            }

            var tipoActividad = await _context.TiposActividad
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoActividad == null)
            {
                return NotFound();
            }

            return View(tipoActividad);
        }

        // POST: TiposActividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TiposActividad == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TiposActividad'  is null.");
            }
            var tipoActividad = await _context.TiposActividad.FindAsync(id);
            if (tipoActividad != null)
            {
                _context.TiposActividad.Remove(tipoActividad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoActividadExists(int id)
        {
          return (_context.TiposActividad?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
