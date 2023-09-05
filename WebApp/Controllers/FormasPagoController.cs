using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SUPERUSER")]
    public class FormasPagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormasPagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FormasPago
        public async Task<IActionResult> Index()
        {
              return _context.FormasPago != null ? 
                          View(await _context.FormasPago.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.FormasPago'  is null.");
        }

        // GET: FormasPago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FormasPago == null)
            {
                return NotFound();
            }

            var formaPago = await _context.FormasPago
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formaPago == null)
            {
                return NotFound();
            }

            return View(formaPago);
        }

        // GET: FormasPago/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FormasPago/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion")] FormaPago formaPago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formaPago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(formaPago);
        }

        // GET: FormasPago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FormasPago == null)
            {
                return NotFound();
            }

            var formaPago = await _context.FormasPago.FindAsync(id);
            if (formaPago == null)
            {
                return NotFound();
            }
            return View(formaPago);
        }

        // POST: FormasPago/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion")] FormaPago formaPago)
        {
            if (id != formaPago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formaPago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormaPagoExists(formaPago.Id))
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
            return View(formaPago);
        }

        // GET: FormasPago/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FormasPago == null)
            {
                return NotFound();
            }

            var formaPago = await _context.FormasPago
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formaPago == null)
            {
                return NotFound();
            }

            return View(formaPago);
        }

        // POST: FormasPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FormasPago == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FormasPago'  is null.");
            }
            var formaPago = await _context.FormasPago.FindAsync(id);
            if (formaPago != null)
            {
                _context.FormasPago.Remove(formaPago);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormaPagoExists(int id)
        {
          return (_context.FormasPago?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
