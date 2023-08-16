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
    public class OrigenesIngresoClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrigenesIngresoClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrigenesIngresoCliente
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrigenesIngresoCliente.Include(o => o.Cliente).Include(o => o.TipoActividad);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrigenesIngresoCliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrigenesIngresoCliente == null)
            {
                return NotFound();
            }

            var origenIngresoCliente = await _context.OrigenesIngresoCliente
                .Include(o => o.Cliente)
                .Include(o => o.TipoActividad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (origenIngresoCliente == null)
            {
                return NotFound();
            }

            return View(origenIngresoCliente);
        }

        // GET: OrigenesIngresoCliente/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Id");
            return View();
        }

        // POST: OrigenesIngresoCliente/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,TipoActividadId,FechaInicioActividad,FechaFinActividad,MontoLiquidoPercibido,UrlDocumento")] OrigenIngresoCliente origenIngresoCliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(origenIngresoCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", origenIngresoCliente.ClienteId);
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Id", origenIngresoCliente.TipoActividadId);
            return View(origenIngresoCliente);
        }

        // GET: OrigenesIngresoCliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrigenesIngresoCliente == null)
            {
                return NotFound();
            }

            var origenIngresoCliente = await _context.OrigenesIngresoCliente.FindAsync(id);
            if (origenIngresoCliente == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", origenIngresoCliente.ClienteId);
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Id", origenIngresoCliente.TipoActividadId);
            return View(origenIngresoCliente);
        }

        // POST: OrigenesIngresoCliente/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,TipoActividadId,FechaInicioActividad,FechaFinActividad,MontoLiquidoPercibido,UrlDocumento")] OrigenIngresoCliente origenIngresoCliente)
        {
            if (id != origenIngresoCliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(origenIngresoCliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrigenIngresoClienteExists(origenIngresoCliente.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", origenIngresoCliente.ClienteId);
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Id", origenIngresoCliente.TipoActividadId);
            return View(origenIngresoCliente);
        }

        // GET: OrigenesIngresoCliente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrigenesIngresoCliente == null)
            {
                return NotFound();
            }

            var origenIngresoCliente = await _context.OrigenesIngresoCliente
                .Include(o => o.Cliente)
                .Include(o => o.TipoActividad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (origenIngresoCliente == null)
            {
                return NotFound();
            }

            return View(origenIngresoCliente);
        }

        // POST: OrigenesIngresoCliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrigenesIngresoCliente == null)
            {
                return Problem("Entity set 'ApplicationDbContext.OrigenesIngresoCliente'  is null.");
            }
            var origenIngresoCliente = await _context.OrigenesIngresoCliente.FindAsync(id);
            if (origenIngresoCliente != null)
            {
                _context.OrigenesIngresoCliente.Remove(origenIngresoCliente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrigenIngresoClienteExists(int id)
        {
          return (_context.OrigenesIngresoCliente?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
