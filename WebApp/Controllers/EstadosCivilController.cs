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
    public class EstadosCivilController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstadosCivilController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EstadosCivil
        public async Task<IActionResult> Index()
        {
              return _context.EstadosCivil != null ? 
                          View(await _context.EstadosCivil.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.EstadosCivils'  is null.");
        }

        // GET: EstadosCivil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EstadosCivil == null)
            {
                return NotFound();
            }

            var estadoCivil = await _context.EstadosCivil
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadoCivil == null)
            {
                return NotFound();
            }

            return View(estadoCivil);
        }

        // GET: EstadosCivil/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EstadosCivil/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,RequiereDatosConyuge")] EstadoCivil estadoCivil)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estadoCivil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estadoCivil);
        }

        // GET: EstadosCivil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EstadosCivil == null)
            {
                return NotFound();
            }

            var estadoCivil = await _context.EstadosCivil.FindAsync(id);
            if (estadoCivil == null)
            {
                return NotFound();
            }
            return View(estadoCivil);
        }

        // POST: EstadosCivil/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,RequiereDatosConyuge")] EstadoCivil estadoCivil)
        {
            if (id != estadoCivil.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estadoCivil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadoCivilExists(estadoCivil.Id))
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
            return View(estadoCivil);
        }

        // GET: EstadosCivil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EstadosCivil == null)
            {
                return NotFound();
            }

            var estadoCivil = await _context.EstadosCivil
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estadoCivil == null)
            {
                return NotFound();
            }

            return View(estadoCivil);
        }

        // POST: EstadosCivil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EstadosCivil == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EstadosCivils'  is null.");
            }
            var estadoCivil = await _context.EstadosCivil.FindAsync(id);
            if (estadoCivil != null)
            {
                _context.EstadosCivil.Remove(estadoCivil);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadoCivilExists(int id)
        {
          return (_context.EstadosCivil?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public JsonResult RequiereDatosConyuge(int estadoCivilId)
        {
            bool requiereDatosConyuge = false;

            if (estadoCivilId > 0)
            {
                var estadosCivil = _context.EstadosCivil.Where(x => x.Id == estadoCivilId).FirstOrDefault();
                requiereDatosConyuge = estadosCivil.RequiereDatosConyuge;

            }

            EstadoCivil estadoCivil = new EstadoCivil
            {
                Id = estadoCivilId,
                RequiereDatosConyuge = requiereDatosConyuge
            };

            return Json(estadoCivil);
        }
    }
}
