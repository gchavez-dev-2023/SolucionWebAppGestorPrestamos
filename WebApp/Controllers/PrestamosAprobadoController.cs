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
    public class PrestamosAprobadoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrestamosAprobadoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrestamosAprobado
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PrestamosAprobado.Include(p => p.SolicitudPrestamo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PrestamosAprobado/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PrestamosAprobado == null)
            {
                return NotFound();
            }

            var prestamoAprobado = await _context.PrestamosAprobado
                .Include(p => p.SolicitudPrestamo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prestamoAprobado == null)
            {
                return NotFound();
            }

            return View(prestamoAprobado);
        }

        // GET: PrestamosAprobado/Create
        public IActionResult Create()
        {
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id");
            return View();
        }

        // POST: PrestamosAprobado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SolicitudPrestamoId,MontoAprobado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,MontoGastosContratacion,MontoGastosMantencionMensual,MontoSegurosMensual,FechaAprobacion,FechaDesembolso,FechaPrimerVencimiento,UrlDocumento,Estado")] PrestamoAprobado prestamoAprobado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prestamoAprobado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", prestamoAprobado.SolicitudPrestamoId);
            return View(prestamoAprobado);
        }

        // GET: PrestamosAprobado/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PrestamosAprobado == null)
            {
                return NotFound();
            }

            var prestamoAprobado = await _context.PrestamosAprobado.FindAsync(id);
            if (prestamoAprobado == null)
            {
                return NotFound();
            }
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", prestamoAprobado.SolicitudPrestamoId);
            return View(prestamoAprobado);
        }

        // POST: PrestamosAprobado/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SolicitudPrestamoId,MontoAprobado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,MontoGastosContratacion,MontoGastosMantencionMensual,MontoSegurosMensual,FechaAprobacion,FechaDesembolso,FechaPrimerVencimiento,UrlDocumento,Estado")] PrestamoAprobado prestamoAprobado)
        {
            if (id != prestamoAprobado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prestamoAprobado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrestamoAprobadoExists(prestamoAprobado.Id))
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
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", prestamoAprobado.SolicitudPrestamoId);
            return View(prestamoAprobado);
        }

        // GET: PrestamosAprobado/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PrestamosAprobado == null)
            {
                return NotFound();
            }

            var prestamoAprobado = await _context.PrestamosAprobado
                .Include(p => p.SolicitudPrestamo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prestamoAprobado == null)
            {
                return NotFound();
            }

            return View(prestamoAprobado);
        }

        // POST: PrestamosAprobado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PrestamosAprobado == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PrestamosAprobado'  is null.");
            }
            var prestamoAprobado = await _context.PrestamosAprobado.FindAsync(id);
            if (prestamoAprobado != null)
            {
                _context.PrestamosAprobado.Remove(prestamoAprobado);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrestamoAprobadoExists(int id)
        {
          return (_context.PrestamosAprobado?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
