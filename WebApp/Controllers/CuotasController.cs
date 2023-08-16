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
    public class CuotasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuotasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cuotas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Cuotas.Include(c => c.PrestamoAprobado);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Cuotas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cuotas == null)
            {
                return NotFound();
            }

            var cuota = await _context.Cuotas
                .Include(c => c.PrestamoAprobado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuota == null)
            {
                return NotFound();
            }

            return View(cuota);
        }

        // GET: Cuotas/Create
        public IActionResult Create()
        {
            ViewData["PrestamoAprobadoId"] = new SelectList(_context.PrestamosAprobado, "Id", "Id");
            return View();
        }

        // POST: Cuotas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrestamoAprobadoId,NumeroCuota,FechaVencimiento,MontoCapital,MontoInteres,MontoGastos,MontoSeguros,MontoTotalCuota,UrlDocumento,Estado")] Cuota cuota)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cuota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PrestamoAprobadoId"] = new SelectList(_context.PrestamosAprobado, "Id", "Id", cuota.PrestamoAprobadoId);
            return View(cuota);
        }

        // GET: Cuotas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cuotas == null)
            {
                return NotFound();
            }

            var cuota = await _context.Cuotas.FindAsync(id);
            if (cuota == null)
            {
                return NotFound();
            }
            ViewData["PrestamoAprobadoId"] = new SelectList(_context.PrestamosAprobado, "Id", "Id", cuota.PrestamoAprobadoId);
            return View(cuota);
        }

        // POST: Cuotas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrestamoAprobadoId,NumeroCuota,FechaVencimiento,MontoCapital,MontoInteres,MontoGastos,MontoSeguros,MontoTotalCuota,UrlDocumento,Estado")] Cuota cuota)
        {
            if (id != cuota.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuota);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuotaExists(cuota.Id))
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
            ViewData["PrestamoAprobadoId"] = new SelectList(_context.PrestamosAprobado, "Id", "Id", cuota.PrestamoAprobadoId);
            return View(cuota);
        }

        // GET: Cuotas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cuotas == null)
            {
                return NotFound();
            }

            var cuota = await _context.Cuotas
                .Include(c => c.PrestamoAprobado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuota == null)
            {
                return NotFound();
            }

            return View(cuota);
        }

        // POST: Cuotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cuotas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Cuotas'  is null.");
            }
            var cuota = await _context.Cuotas.FindAsync(id);
            if (cuota != null)
            {
                _context.Cuotas.Remove(cuota);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuotaExists(int id)
        {
          return (_context.Cuotas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
