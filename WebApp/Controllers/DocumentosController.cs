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
    public class DocumentosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocumentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Documentos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Documentos.Include(d => d.SolicitudPrestamo).Include(d => d.TipoDocumento);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Documentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Documentos == null)
            {
                return NotFound();
            }

            var documento = await _context.Documentos
                .Include(d => d.SolicitudPrestamo)
                .Include(d => d.TipoDocumento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        // GET: Documentos/Create
        public IActionResult Create()
        {
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id");
            ViewData["TipoDocumentoId"] = new SelectList(_context.TiposDocumento, "Id", "Descripcion");
            return View();
        }

        // POST: Documentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SolicitudPrestamoId,TipoDocumentoId,UrlDocumento")] Documento documento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(documento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", documento.SolicitudPrestamoId);
            ViewData["TipoDocumentoId"] = new SelectList(_context.TiposDocumento, "Id", "Descripcion", documento.TipoDocumentoId);
            return View(documento);
        }

        // GET: Documentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Documentos == null)
            {
                return NotFound();
            }

            var documento = await _context.Documentos.FindAsync(id);
            if (documento == null)
            {
                return NotFound();
            }
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", documento.SolicitudPrestamoId);
            ViewData["TipoDocumentoId"] = new SelectList(_context.TiposDocumento, "Id", "Descripcion", documento.TipoDocumentoId);
            return View(documento);
        }

        // POST: Documentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SolicitudPrestamoId,TipoDocumentoId,UrlDocumento")] Documento documento)
        {
            if (id != documento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentoExists(documento.Id))
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
            ViewData["SolicitudPrestamoId"] = new SelectList(_context.SolicitudesPrestamo, "Id", "Id", documento.SolicitudPrestamoId);
            ViewData["TipoDocumentoId"] = new SelectList(_context.TiposDocumento, "Id", "Descripcion", documento.TipoDocumentoId);
            return View(documento);
        }

        // GET: Documentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Documentos == null)
            {
                return NotFound();
            }

            var documento = await _context.Documentos
                .Include(d => d.SolicitudPrestamo)
                .Include(d => d.TipoDocumento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        // POST: Documentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Documentos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Documentos'  is null.");
            }
            var documento = await _context.Documentos.FindAsync(id);
            if (documento != null)
            {
                _context.Documentos.Remove(documento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentoExists(int id)
        {
          return (_context.Documentos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
