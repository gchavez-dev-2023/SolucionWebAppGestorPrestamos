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
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Productos.Include(p => p.Beneficios).Include(p => p.Requisitos).Include(p => p.Terminos);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Beneficios)
                .Include(p => p.Requisitos)
                .Include(p => p.Terminos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id");
            ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id");
            ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,RequisitosId,BeneficiosId,TerminosId,FechaInicioVigencia,FechaFinVigencia")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id", producto.BeneficiosId);
            ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id", producto.RequisitosId);
            ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id", producto.TerminosId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id", producto.BeneficiosId);
            ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id", producto.RequisitosId);
            ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id", producto.TerminosId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,RequisitosId,BeneficiosId,TerminosId,FechaInicioVigencia,FechaFinVigencia")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id", producto.BeneficiosId);
            ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id", producto.RequisitosId);
            ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id", producto.TerminosId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Beneficios)
                .Include(p => p.Requisitos)
                .Include(p => p.Terminos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Productos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Productos'  is null.");
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.Productos?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public JsonResult GetCTFyValorCuota(int productoId, decimal montoSolicitado, int cantidadCuotas)
        {
            decimal ctf = 0;
            decimal montoCuota = 0;

            if (montoSolicitado > 0 && cantidadCuotas > 0)
            {
                var producto = _context.Productos
                    .Where(x => x.Id == productoId)
                    .Include(p => p.Terminos)
                    .FirstOrDefault();
                //var terminos = _context.Terminos.Where(x => x.Id == producto.TerminosId).FirstOrDefault();
                if (producto != null)
                {
                    //Se cambia la tasa efectiva de Decimal a Double, porque la funcion que calcula potencia no soporta decimal
                    var mensual = (montoSolicitado * ((producto.Terminos.TasaNominal + producto.Terminos.TasaGastosCobranza + producto.Terminos.TasaSeguros) / 100) * cantidadCuotas);
                    var unico = (montoSolicitado * (producto.Terminos.TasaGastosAdministrativos / 100));
                    //montoCuota = Math.Round((((montoSolicitado * interesEfectivoMensual) + (montoSolicitado * terminos.TasaGastosAdministrativos) + (montoSolicitado * terminos.TasaGastosAdministrativos * cantidadCuotas) + (montoSolicitado * terminos.TasaSeguros * cantidadCuotas)) / cantidadCuotas), 2);
                    montoCuota = Math.Round(((montoSolicitado + unico + mensual) / cantidadCuotas), 2);
                    //ctf = Math.Round((((montoCuota*cantidadCuotas) - montoSolicitado)/cantidadCuotas)/100, 3);
                    ctf = Math.Round(((unico + mensual) / montoSolicitado) * 100, 3);
                }
            }

            SolicitudPrestamo credito = new SolicitudPrestamo
            {
                CostoTotalFinanciero = ctf,
                ValorCuota = montoCuota
            };

            return Json(credito);
        }


        [HttpPost]
        public JsonResult EvalProducto(int productoId)
        {
            int edadMinima = 0;
            int edadMaxima = 0;
            int scoringMinimo = 0;
            int cantidadAvales = 0;
            decimal montoMinimo = 0;
            decimal montoMaximo = 0;
            int plazoMinimo = 0;
            int plazoMaximo = 0;

            var producto = _context.Productos
                .Include(p => p.Beneficios)
                .Include(p => p.Requisitos)
                .Include(p => p.Terminos)
                .Where(x => x.Id == productoId)
                .FirstOrDefault();

            if (producto != null)
            {
                edadMinima = producto.Requisitos.EdadMinima;
                edadMaxima = producto.Requisitos.EdadMaxima;
                scoringMinimo = producto.Requisitos.ScoringMinimo;
                cantidadAvales = producto.Requisitos.CantidadAvales;
                montoMinimo = producto.Terminos.MontoMinimo;
                montoMaximo = producto.Terminos.MontoMaximo;
                plazoMinimo = producto.Terminos.PlazoMinimo;
                plazoMaximo = producto.Terminos.PlazoMaximo;
                
            }

            ProductoDto productoDto = new ProductoDto
            {
                EdadMinima = edadMinima,
                EdadMaxima = edadMaxima,
                ScoringMinimo = scoringMinimo,
                CantidadAvales = cantidadAvales,
                MontoMinimo = montoMinimo,
                MontoMaximo = montoMaximo,
                PlazoMinimo = plazoMinimo,
                PlazoMaximo = plazoMaximo
            };

            return Json(productoDto);
        }
    }
}
