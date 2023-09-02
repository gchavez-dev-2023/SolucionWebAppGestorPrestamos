using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ProductosDtoController : Controller
    {

        private readonly ApplicationDbContext _context;
        public ProductosDtoController(ApplicationDbContext context)
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

            var productoDto = new ProductoDto();
            productoModelToDto(producto, productoDto);

            return View(productoDto);
        }


        // GET: Productos/Create
        public IActionResult Create()
        {
            //ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id");
            //ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id");
            //ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,RequisitosId,EdadMinima,EdadMaxima,ScoringMinimo,CantidadAvales,CantidadRecibosSueldo,BeneficiosId,Aprobacion24Horas,SolicitudEnLinea,MontoMinimo,MontoMaximo,PlazoMinimo,PlazoMaximo,TasaNominal,TasaGastosAdministrativos,TasaGastosCobranza,TasaSeguros,TasaInteresMora,EsPrepagable,TasaCastigoPrepago,TasaCoberturaAval,TasaCoberturaConyuge,TerminosId,FechaInicioVigencia,FechaFinVigencia")] ProductoDto productoDto)
        {
            if (ModelState.IsValid)
            {

                //evaluar si existe Producto
                var existeProducto = await _context.Productos
                    .FirstOrDefaultAsync(m => m.Descripcion == productoDto.Descripcion) == null ? false : true;
                if (existeProducto)
                {
                    ModelState.AddModelError(string.Empty,
                    "Ya existe un Producto con misma Descripcion. " + productoDto.Descripcion);
                }
                else
                {
                    var requisito = new Requisito();
                    requisitoDtoToModel(productoDto, requisito);
                    _context.Add(requisito);
                    await _context.SaveChangesAsync();

                    var beneficio = new Beneficio();
                    beneficioDtoToModel(productoDto, beneficio);
                    _context.Add(beneficio);
                    await _context.SaveChangesAsync();

                    var termino = new Termino();
                    terminoDtoToModel(productoDto, termino);
                    _context.Add(termino);
                    await _context.SaveChangesAsync();

                    var producto = new Producto();
                    productoDtoToModel(productoDto, producto, requisito, beneficio, termino);
                    _context.Add(producto);
                    await _context.SaveChangesAsync();


                    return RedirectToAction(nameof(Index));
                }
            }
            //ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id", producto.BeneficiosId);
            //ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id", producto.RequisitosId);
            //ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id", producto.TerminosId);
            return View(productoDto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var productoDto = new ProductoDto();
            productoModelToDto(producto, productoDto);

            //ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id", producto.BeneficiosId);
            //ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id", producto.RequisitosId);
            //ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id", producto.TerminosId);
            return View(productoDto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,RequisitosId,EdadMinima,EdadMaxima,ScoringMinimo,CantidadAvales,CantidadRecibosSueldo,BeneficiosId,Aprobacion24Horas,SolicitudEnLinea,MontoMinimo,MontoMaximo,PlazoMinimo,PlazoMaximo,TasaNominal,TasaGastosAdministrativos,TasaGastosCobranza,TasaSeguros,TasaInteresMora,EsPrepagable,TasaCastigoPrepago,TasaCoberturaAval,TasaCoberturaConyuge,TerminosId,FechaInicioVigencia,FechaFinVigencia")] ProductoDto productoDto)
        {
            if (id != productoDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                //evaluar si existe Producto
                var existeProducto = await _context.Productos
                    .FirstOrDefaultAsync(m => m.Descripcion == productoDto.Descripcion) == null ? false : true;
                if (existeProducto)
                {
                    ModelState.AddModelError(string.Empty,
                    "Ya existe un Producto con misma Descripcion. " + productoDto.Descripcion);
                }
                else
                {
                    try
                    {
                        //Requisito
                        var requisito = await _context.Requisitos
                        .FirstOrDefaultAsync(m => m.Id == productoDto.RequisitosId);

                        //Evaluar si trajo datos la consulta de la BD
                        if (requisito != null)
                        {
                            //Actualizar Requisito
                            requisitoDtoToModel(productoDto, requisito);
                            _context.Update(requisito);
                        }
                        else
                        {
                            //Crear Requisito
                            requisito = new Requisito();
                            requisitoDtoToModel(productoDto, requisito);
                            _context.Add(requisito);
                        }
                        await _context.SaveChangesAsync();

                        //Beneficio
                        var beneficio = await _context.Beneficios
                        .FirstOrDefaultAsync(m => m.Id == productoDto.BeneficiosId);

                        //Evaluar si trajo datos la consulta de la BD
                        if (beneficio != null)
                        {
                            //Actualizar Beneficio
                            beneficioDtoToModel(productoDto, beneficio);
                            _context.Update(beneficio);
                        }
                        else
                        {
                            //Crear Beneficio
                            beneficio = new Beneficio();
                            beneficioDtoToModel(productoDto, beneficio);
                            _context.Add(beneficio);
                        }
                        await _context.SaveChangesAsync();

                        //Termino
                        var termino = await _context.Terminos
                        .FirstOrDefaultAsync(m => m.Id == productoDto.TerminosId);

                        //Evaluar si trajo datos la consulta de la BD
                        if (termino != null)
                        {
                            //Actualizar Termino
                            terminoDtoToModel(productoDto, termino);
                            _context.Update(termino);
                        }
                        else
                        {
                            //Crear Termino
                            termino = new Termino();
                            terminoDtoToModel(productoDto, termino);
                            _context.Add(termino);
                        }
                        await _context.SaveChangesAsync();

                        //Producto
                        var producto = await _context.Productos
                        .FirstOrDefaultAsync(m => m.Id == productoDto.Id);
                        //Actualizar Producto
                        productoDtoToModel(productoDto, producto, requisito, beneficio, termino);
                        _context.Update(producto);
                        await _context.SaveChangesAsync();

                        //Termino OK
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductoExists(productoDto.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            //ViewData["BeneficiosId"] = new SelectList(_context.Beneficios, "Id", "Id", producto.BeneficiosId);
            //ViewData["RequisitosId"] = new SelectList(_context.Requisitos, "Id", "Id", producto.RequisitosId);
            //ViewData["TerminosId"] = new SelectList(_context.Terminos, "Id", "Id", producto.TerminosId);
            return View(productoDto);
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

            var productoDto = new ProductoDto();
            productoModelToDto(producto, productoDto);

            return View(productoDto);
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
                await _context.SaveChangesAsync();
            }

            var beneficio = await _context.Beneficios.FindAsync(producto.BeneficiosId);
            if (beneficio != null)
            {
                _context.Beneficios.Remove(beneficio);
                await _context.SaveChangesAsync();
            }

            var requisito = await _context.Requisitos.FindAsync(producto.RequisitosId);
            if (requisito != null)
            {
                _context.Requisitos.Remove(requisito);
                await _context.SaveChangesAsync();
            }

            var termino = await _context.Terminos.FindAsync(producto.TerminosId);
            if (termino != null)
            {
                _context.Terminos.Remove(termino);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return (_context.Productos?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static void productoModelToDto(Producto? producto, ProductoDto productoDto)
        {
            productoDto.Id = producto.Id;
            productoDto.Descripcion = producto.Descripcion;
            productoDto.RequisitosId = producto.RequisitosId;
            productoDto.EdadMinima = producto.Requisitos.EdadMinima;
            productoDto.EdadMaxima = producto.Requisitos.EdadMaxima;
            productoDto.ScoringMinimo = producto.Requisitos.ScoringMinimo;
            productoDto.CantidadAvales = producto.Requisitos.CantidadAvales;
            productoDto.CantidadRecibosSueldo = producto.Requisitos.CantidadRecibosSueldo;
            productoDto.BeneficiosId = producto.BeneficiosId;
            productoDto.Aprobacion24Horas = producto.Beneficios.Aprobacion24Horas;
            productoDto.SolicitudEnLinea = producto.Beneficios.SolicitudEnLinea;
            productoDto.TerminosId = producto.TerminosId;
            productoDto.MontoMinimo = producto.Terminos.MontoMinimo;
            productoDto.MontoMaximo = producto.Terminos.MontoMaximo;
            productoDto.PlazoMinimo = producto.Terminos.PlazoMinimo;
            productoDto.PlazoMaximo = producto.Terminos.PlazoMaximo;
            productoDto.TasaNominal = producto.Terminos.TasaNominal;
            productoDto.TasaGastosAdministrativos = producto.Terminos.TasaGastosAdministrativos;
            productoDto.TasaGastosCobranza = producto.Terminos.TasaGastosCobranza;
            productoDto.TasaSeguros = producto.Terminos.TasaSeguros;
            productoDto.TasaInteresMora = producto.Terminos.TasaInteresMora;
            productoDto.EsPrepagable = producto.Terminos.EsPrepagable;
            productoDto.TasaCastigoPrepago = producto.Terminos.TasaCastigoPrepago;
            productoDto.TasaCoberturaAval = producto.Terminos.TasaCoberturaAval;
            productoDto.TasaCoberturaConyuge = producto.Terminos.TasaCoberturaConyuge;
            productoDto.FechaInicioVigencia = producto.FechaInicioVigencia;
            productoDto.FechaFinVigencia = producto.FechaFinVigencia;
        }

        private static void productoDtoToModel(ProductoDto productoDto, Producto producto, Requisito requisito, Beneficio beneficio, Termino termino)
        {
            producto.Id = productoDto.Id;
            producto.Descripcion = productoDto.Descripcion;
            producto.RequisitosId = requisito.Id;
            producto.BeneficiosId = beneficio.Id;
            producto.TerminosId = termino.Id;
            producto.FechaInicioVigencia = productoDto.FechaInicioVigencia;
            producto.FechaFinVigencia = productoDto.FechaFinVigencia;
        }

        private static void requisitoDtoToModel(ProductoDto productoDto, Requisito requisito)
        {
            requisito.Id = productoDto.RequisitosId;
            requisito.EdadMinima = productoDto.EdadMinima;
            requisito.EdadMaxima = productoDto.EdadMaxima;
            requisito.ScoringMinimo = productoDto.ScoringMinimo;
            requisito.CantidadAvales = productoDto.CantidadAvales;
            requisito.CantidadRecibosSueldo = productoDto.CantidadRecibosSueldo;
        }

        private static void beneficioDtoToModel(ProductoDto productoDto, Beneficio beneficio)
        {
            beneficio.Id = productoDto.BeneficiosId;
            beneficio.Aprobacion24Horas = productoDto.Aprobacion24Horas;
            beneficio.SolicitudEnLinea = productoDto.SolicitudEnLinea;
        }

        private static void terminoDtoToModel(ProductoDto productoDto, Termino termino)
        {
            termino.Id = productoDto.TerminosId;
            termino.MontoMinimo = productoDto.MontoMinimo;
            termino.MontoMaximo = productoDto.MontoMaximo;
            termino.PlazoMinimo = productoDto.PlazoMinimo;
            termino.PlazoMaximo = productoDto.PlazoMaximo;
            termino.TasaNominal = productoDto.TasaNominal;
            termino.TasaGastosAdministrativos = productoDto.TasaGastosAdministrativos;
            termino.TasaGastosCobranza = productoDto.TasaGastosCobranza;
            termino.TasaSeguros = productoDto.TasaSeguros;
            termino.TasaInteresMora = productoDto.TasaInteresMora;
            termino.EsPrepagable = productoDto.EsPrepagable;
            termino.TasaCastigoPrepago = productoDto.TasaCastigoPrepago;
            termino.TasaCoberturaAval = productoDto.TasaCoberturaAval;
            termino.TasaCoberturaConyuge = productoDto.TasaCoberturaConyuge;
        }
    }
}
