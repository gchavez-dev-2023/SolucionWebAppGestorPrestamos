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
    public class SolicitudesPrestamoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesPrestamoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SolicitudesPrestamo
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SolicitudesPrestamo.Include(s => s.Cliente).Include(s => s.Producto);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SolicitudesPrestamo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SolicitudesPrestamo == null)
            {
                return NotFound();
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .Include(s => s.Cliente)
                .Include(s => s.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solicitudPrestamo == null)
            {
                return NotFound();
            }

            return View(solicitudPrestamo);
        }

        // GET: SolicitudesPrestamo/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Id");
            return View();
        }

        // POST: SolicitudesPrestamo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,ProductoId,MontoSolicitado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,TasaCoberturaDeudaConyuge,FechaSolicitud,UrlDocumento,Estado")] SolicitudPrestamo solicitudPrestamo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solicitudPrestamo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", solicitudPrestamo.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Id", solicitudPrestamo.ProductoId);
            return View(solicitudPrestamo);
        }

        // GET: SolicitudesPrestamo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SolicitudesPrestamo == null)
            {
                return NotFound();
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo.FindAsync(id);
            if (solicitudPrestamo == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", solicitudPrestamo.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Id", solicitudPrestamo.ProductoId);
            return View(solicitudPrestamo);
        }

        // POST: SolicitudesPrestamo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,ProductoId,MontoSolicitado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,TasaCoberturaDeudaConyuge,FechaSolicitud,UrlDocumento,Estado")] SolicitudPrestamo solicitudPrestamo)
        {
            if (id != solicitudPrestamo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solicitudPrestamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolicitudPrestamoExists(solicitudPrestamo.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", solicitudPrestamo.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Id", solicitudPrestamo.ProductoId);
            return View(solicitudPrestamo);
        }

        // GET: SolicitudesPrestamo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SolicitudesPrestamo == null)
            {
                return NotFound();
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .Include(s => s.Cliente)
                .Include(s => s.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solicitudPrestamo == null)
            {
                return NotFound();
            }

            return View(solicitudPrestamo);
        }

        // POST: SolicitudesPrestamo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SolicitudesPrestamo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SolicitudesPrestamo'  is null.");
            }
            var solicitudPrestamo = await _context.SolicitudesPrestamo.FindAsync(id);
            if (solicitudPrestamo != null)
            {
                _context.SolicitudesPrestamo.Remove(solicitudPrestamo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SolicitudPrestamoExists(int id)
        {
          return (_context.SolicitudesPrestamo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
