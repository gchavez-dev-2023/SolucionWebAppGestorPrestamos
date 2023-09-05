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
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SUPERUSER")]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clientes.Include(c => c.EstadoCivil).Include(c => c.Persona);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.EstadoCivil)
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion");
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad");
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PersonaId,DomicilioAlternativo,TelefonoLaboral,PersonaPoliticamenteExpuesta,EstadoCivilId,Scoring,UrlDocumento")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion", cliente.EstadoCivilId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", cliente.PersonaId);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion", cliente.EstadoCivilId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", cliente.PersonaId);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PersonaId,DomicilioAlternativo,TelefonoLaboral,PersonaPoliticamenteExpuesta,EstadoCivilId,Scoring,UrlDocumento")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion", cliente.EstadoCivilId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", cliente.PersonaId);
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.EstadoCivil)
                .Include(c => c.Persona)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clientes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Clientes'  is null.");
            }
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
          return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public JsonResult EvalCliente(int clienteId)
        {
            int edad = 0;
            int scoring = 0;

            var cliente = _context.Clientes
                .Include(p => p.Persona)
                .Where(x => x.Id == clienteId)
                .FirstOrDefault();

            if (cliente != null)
            {
                edad = DateTime.Today.Year - cliente.Persona.FechaNacimiento.Year;
                scoring = cliente.Scoring;
            }

            ClienteDto clienteDto = new ClienteDto
            {
                Edad = edad,
                Scoring = scoring
            };

            return Json(clienteDto);
        }
    }
}
