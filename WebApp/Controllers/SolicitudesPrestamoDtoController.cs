using DinkToPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE,COLABORADOR")]
    public class SolicitudesPrestamoDtoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesPrestamoDtoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SolicitudesPrestamo
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = await _context.SolicitudesPrestamo.Include(s => s.Cliente)
                .Include(s => s.Producto)
                .Where(x => x.Estado == "Analisis")
                .OrderByDescending(s => s.Id).ToListAsync();

            foreach (var item in applicationDbContext)
            {
                item.Cliente.Persona = await _context.Personas.FirstOrDefaultAsync(x => x.Id == item.Cliente.PersonaId);
            }

            return View( applicationDbContext);
        }

        // GET: SolicitudesPrestamo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SolicitudesPrestamo == null)
            {
                return NotFound();
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .Include(s => s.Avales)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solicitudPrestamo == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.
                Include(c => c.Persona).
                Include(c => c.OrigenesIngresoClientes).
                Include(c => c.EstadoCivil).
                Include(c => c.Conyuges).
                FirstOrDefaultAsync(m => m.Id == solicitudPrestamo.ClienteId);

            var personaConyuge = new Persona();

            if (cliente.EstadoCivil.RequiereDatosConyuge)
            {
                var conyuge = cliente.Conyuges.FirstOrDefault();
                personaConyuge = await _context.Personas.FirstOrDefaultAsync(c => c.Id == conyuge.PersonaId);
            }


            var producto = await _context.Productos
                .Include(p => p.Beneficios)
                .Include(p => p.Requisitos)
                .Include(p => p.Terminos)
                .FirstOrDefaultAsync(m => m.Id == solicitudPrestamo.ProductoId);

            var solicitudPrestamoDto = new SolicitudPrestamoDto();
            //Inicializar campos necesarios
            solicitudPrestamoDto.ClienteDto = new ClienteDto();
            solicitudPrestamoDto.ProductoDto = new ProductoDto();
            solicitudPrestamoDto.AvalesDto = new List<AvalDto>();

            foreach (var aval in solicitudPrestamo.Avales)
            {
                aval.Persona = await _context.Personas
                .FirstOrDefaultAsync(m => m.Id == aval.PersonaId);
            }

            SolicitudPrestamoModelToDto(solicitudPrestamo, solicitudPrestamoDto, cliente, personaConyuge, producto);

            return View(solicitudPrestamoDto);
        }

        // GET: SolicitudesPrestamo/CreateStep1
        public IActionResult CreateStep1()
        {
            //Solo cargar clientes, que sean personas validas
            ViewData["ClienteId"] = new SelectList(_context.Clientes
                .Include(c => c.Persona)
                .Where(x => x.Persona.DatosVerificados == true)
                , "Id", "Persona.CedulaIdentidad");

            //Solo cargar productos, que esten vigentes
            ViewData["ProductoId"] = new SelectList(_context.Productos
                .Where(x => x.FechaInicioVigencia <= DateTime.Today
                         && x.FechaFinVigencia >= DateTime.Today)
                , "Id", "Descripcion");

            var solicitudPrestamoDto = new SolicitudPrestamoDto();
            //Inicializar campos necesarios
            solicitudPrestamoDto.ClienteDto = new ClienteDto();
            solicitudPrestamoDto.ProductoDto = new ProductoDto();
            solicitudPrestamoDto.AvalesDto = new List<AvalDto>();
            return View(solicitudPrestamoDto);
        }

        // GET: SolicitudesPrestamo/CreateStep2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStep2([Bind("Id,ClienteId,ClienteDto,ProductoId,ProductoDto,MontoSolicitado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,TasaCoberturaDeudaConyuge,FechaSolicitud,UrlDocumento,Estado,CantidadAvales,AvalesDto")] SolicitudPrestamoDto solicitudPrestamoDto)
        {

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Include(c => c.Persona), "Id", "Persona.CedulaIdentidad", solicitudPrestamoDto.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", solicitudPrestamoDto.ProductoId);

            solicitudPrestamoDto.FechaSolicitud = DateTime.Today;

            if (solicitudPrestamoDto.ProductoDto.EdadMinima > solicitudPrestamoDto.ClienteDto.Edad ||
                solicitudPrestamoDto.ProductoDto.EdadMaxima < solicitudPrestamoDto.ClienteDto.Edad)
            {
                ModelState.AddModelError(string.Empty,
                "Edad del Cliente fuera de los rango del Producto seleccionado. " + solicitudPrestamoDto.ClienteDto.Edad);
                return View(nameof(CreateStep1), solicitudPrestamoDto);
            }

            if (solicitudPrestamoDto.ProductoDto.ScoringMinimo > solicitudPrestamoDto.ClienteDto.Scoring)
            {
                ModelState.AddModelError(string.Empty,
                "Scoring del Cliente fuera de los rango del Producto seleccionado. " + solicitudPrestamoDto.ClienteDto.Scoring);
                return View(nameof(CreateStep1), solicitudPrestamoDto);
            }

            if (solicitudPrestamoDto.ProductoDto.MontoMinimo > solicitudPrestamoDto.MontoSolicitado ||
                solicitudPrestamoDto.ProductoDto.MontoMaximo < solicitudPrestamoDto.MontoSolicitado)
            {
                ModelState.AddModelError(string.Empty,
                "Monto Solicitado fuera de los rango del Producto seleccionado. " + solicitudPrestamoDto.MontoSolicitado);
                return View(nameof(CreateStep1), solicitudPrestamoDto);
            }

            if (solicitudPrestamoDto.ProductoDto.PlazoMinimo > solicitudPrestamoDto.CantidadCuotas ||
                solicitudPrestamoDto.ProductoDto.PlazoMaximo < solicitudPrestamoDto.CantidadCuotas)
            {
                ModelState.AddModelError(string.Empty,
                "Cantidad Cuotas fuera de los rango del Producto seleccionado. " + solicitudPrestamoDto.CantidadCuotas);
                return View(nameof(CreateStep1),solicitudPrestamoDto);
            }
            //Inicializar
            solicitudPrestamoDto.AvalesDto = new List<AvalDto>();

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");

            for (int i = 0; i < solicitudPrestamoDto.CantidadAvales; i++)
            {
                var avalDto = new AvalDto();
                solicitudPrestamoDto.AvalesDto.Add(avalDto);
            }

            solicitudPrestamoDto.ClienteDto.Id = solicitudPrestamoDto.ClienteId;
            solicitudPrestamoDto.ProductoDto.Id = solicitudPrestamoDto.ProductoId;
            return View(solicitudPrestamoDto);
        }

        // POST: SolicitudesPrestamo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,ClienteDto,ProductoId,ProductoDto,MontoSolicitado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,TasaCoberturaDeudaConyuge,FechaSolicitud,UrlDocumento,Estado,CantidadAvales,AvalesDto")] SolicitudPrestamoDto solicitudPrestamoDto)
        {

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Include(c => c.Persona), "Id", "Persona.CedulaIdentidad", solicitudPrestamoDto.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", solicitudPrestamoDto.ProductoId);

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");

            try
            {
                string clienteCedulaIdentidad = "";
                string conyugeCedulaIdentidad = "";
                var cliente = await _context.Clientes
                    .Include(c => c.Persona)
                    .Include(c => c.EstadoCivil)
                    .Include(c => c.Conyuges)
                    .FirstOrDefaultAsync(x => x.Id == solicitudPrestamoDto.ClienteId);

                if (cliente != null)
                {
                    clienteCedulaIdentidad = cliente.Persona.CedulaIdentidad;
                    if (cliente.EstadoCivil.RequiereDatosConyuge)
                    {
                        var conyuge = cliente.Conyuges.FirstOrDefault();
                        if (conyuge != null)
                        {
                            var personaConyuge = await _context.Personas.FirstOrDefaultAsync(x => x.Id == conyuge.PersonaId);
                            if (personaConyuge != null)
                            {
                                conyugeCedulaIdentidad = personaConyuge.CedulaIdentidad;
                            }
                        }

                    }
                }
                //Lista para recolectar Cedula Identidad de Avales
                List<string> listaCedulaIdentidadAvales = new List<string>();
                for (int i = 0; i < solicitudPrestamoDto.AvalesDto.Count; i++)
                {
                    listaCedulaIdentidadAvales.Add(solicitudPrestamoDto.AvalesDto[i].CedulaIdentidad);
                    if (solicitudPrestamoDto.AvalesDto[i].CedulaIdentidad == clienteCedulaIdentidad)
                    {
                        ModelState.AddModelError(string.Empty,
                        "Cedula Identidad no puede ser el mismo del Cliente, error en Aval N° " + i + 1);
                        return View(nameof(CreateStep2), solicitudPrestamoDto);
                    }
                    if (solicitudPrestamoDto.AvalesDto[i].CedulaIdentidad == conyugeCedulaIdentidad)
                    {
                        ModelState.AddModelError(string.Empty,
                        "Cedula Identidad no puede ser el mismo del Conyuge, error en Aval N° " + i + 1);
                        return View(nameof(CreateStep2), solicitudPrestamoDto);
                    }
                    if (solicitudPrestamoDto.AvalesDto[i].FechaNacimiento >= DateTime.Today)
                    {
                        ModelState.AddModelError(string.Empty,
                        "Fecha de Nacimiento del Aval no puede ser mayor o igual a la fecha de día, error en Aval N° " + i + 1);
                        return View(nameof(CreateStep2), solicitudPrestamoDto);
                    }
                }

                //Comprobar si existen duplicados las Cedula Identidad de Avales.
                var hasDuplicates = listaCedulaIdentidadAvales.GroupBy(x => x).Any(g => g.Count() > 1);
                if (hasDuplicates)
                {
                    ModelState.AddModelError(string.Empty,
                    "Existe Cedula de Identidad duplicada entre los avales.");
                    return View(nameof(CreateStep2), solicitudPrestamoDto);
                }

                //Alta de Solicitud
                SolicitudPrestamo solicitudPrestamo = new SolicitudPrestamo();
                SolicitudPrestamoDtoToModel(solicitudPrestamoDto, solicitudPrestamo);
                _context.Add(solicitudPrestamo);
                await _context.SaveChangesAsync();

                //Alta Avales
                for (int i = 0; i < solicitudPrestamoDto.AvalesDto.Count; i++)
                {
                    //Alta Persona
                    Persona persona = new Persona();
                    PersonaDtoToModel(solicitudPrestamoDto.AvalesDto[i], persona);
                    _context.Add(persona);
                    await _context.SaveChangesAsync();

                    //Alta Aval
                    Aval aval = new Aval();
                    aval.SolicitudPrestamoId = solicitudPrestamo.Id;
                    aval.PersonaId = persona.Id;
                    aval.TasaCoberturaDeuda = 0;//TODO enlazar al producto.terminos
                    aval.UrlDocumento = "-";

                    _context.Add(aval);
                    await _context.SaveChangesAsync();
                }

                //Termino OK
                return RedirectToAction(nameof(Index));

                //if (ModelState.IsValid)
                //{
                //    _context.Add(solicitudPrestamoDto);
                //    await _context.SaveChangesAsync();
                //    return RedirectToAction(nameof(Index));
                //}
            }
            catch (Exception e) {
                ModelState.AddModelError(string.Empty,
                        "Error: " + e.Message);
            }

            return View(nameof(CreateStep2), solicitudPrestamoDto);
            //return RedirectToAction(nameof(Index));
        }

        // GET: SolicitudesPrestamo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SolicitudesPrestamo == null)
            {
                return NotFound();
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .Include(s => s.Avales)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (solicitudPrestamo == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.
                Include(c => c.Persona).
                Include(c => c.OrigenesIngresoClientes).
                Include(c => c.EstadoCivil).
                Include(c => c.Conyuges).
                FirstOrDefaultAsync(m => m.Id == solicitudPrestamo.ClienteId);

            var personaConyuge = new Persona();

            if (cliente.EstadoCivil.RequiereDatosConyuge)
            {
                var conyuge = cliente.Conyuges.FirstOrDefault();
                personaConyuge = await _context.Personas.FirstOrDefaultAsync(c => c.Id == conyuge.PersonaId);
            }


            var producto = await _context.Productos
                .Include(p => p.Beneficios)
                .Include(p => p.Requisitos)
                .Include(p => p.Terminos)
                .FirstOrDefaultAsync(m => m.Id == solicitudPrestamo.ProductoId);

            var solicitudPrestamoDto = new SolicitudPrestamoDto();
            //Inicializar campos necesarios
            solicitudPrestamoDto.ClienteDto = new ClienteDto();
            solicitudPrestamoDto.ProductoDto = new ProductoDto();
            solicitudPrestamoDto.AvalesDto = new List<AvalDto>();

            foreach (var aval in solicitudPrestamo.Avales)
            {
                aval.Persona = await _context.Personas
                .FirstOrDefaultAsync(m => m.Id == aval.PersonaId);
            }

            SolicitudPrestamoModelToDto(solicitudPrestamo, solicitudPrestamoDto, cliente, personaConyuge, producto);

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Include(c => c.Persona), "Id", "Persona.CedulaIdentidad", solicitudPrestamoDto.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", solicitudPrestamoDto.ProductoId);

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");

            return View(solicitudPrestamoDto);

        }

        // POST: SolicitudesPrestamo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,ClienteDto,ProductoId,ProductoDto,MontoSolicitado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,TasaCoberturaDeudaConyuge,FechaSolicitud,UrlDocumento,Estado,CantidadAvales,AvalesDto")] SolicitudPrestamoDto solicitudPrestamoDto)
        {
            if (id != solicitudPrestamoDto.Id)
            {
                return NotFound();
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Include(c => c.Persona), "Id", "Persona.CedulaIdentidad", solicitudPrestamoDto.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", solicitudPrestamoDto.ProductoId);

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");

            if (ModelState.IsValid)
            {
                try
                {
                    string clienteCedulaIdentidad = "";
                    string conyugeCedulaIdentidad = "";
                    var cliente = await _context.Clientes
                        .Include(c => c.Persona)
                        .Include(c => c.EstadoCivil)
                        .Include(c => c.Conyuges)
                        .FirstOrDefaultAsync(x => x.Id == solicitudPrestamoDto.ClienteId);

                    if (cliente != null)
                    {
                        clienteCedulaIdentidad = cliente.Persona.CedulaIdentidad;
                        if (cliente.EstadoCivil.RequiereDatosConyuge)
                        {
                            var conyuge = cliente.Conyuges.FirstOrDefault();
                            if (conyuge != null)
                            {
                                var personaConyuge = await _context.Personas.FirstOrDefaultAsync(x => x.Id == conyuge.PersonaId);
                                if (personaConyuge != null)
                                {
                                    conyugeCedulaIdentidad = personaConyuge.CedulaIdentidad;
                                }
                            }
                        }
                    }
                    //Lista para recolectar Cedula Identidad de Avales
                    List<string> listaCedulaIdentidadAvales = new List<string>();
                    for (int i = 0; i < solicitudPrestamoDto.AvalesDto.Count; i++)
                    {
                        listaCedulaIdentidadAvales.Add(solicitudPrestamoDto.AvalesDto[i].CedulaIdentidad);
                        if (solicitudPrestamoDto.AvalesDto[i].CedulaIdentidad == clienteCedulaIdentidad)
                        {
                            ModelState.AddModelError(string.Empty,
                            "Cedula Identidad no puede ser el mismo del Cliente, error en Aval N° " + i + 1);
                            return View(solicitudPrestamoDto);
                        }
                        if (solicitudPrestamoDto.AvalesDto[i].CedulaIdentidad == conyugeCedulaIdentidad)
                        {
                            ModelState.AddModelError(string.Empty,
                            "Cedula Identidad no puede ser el mismo del Conyuge, error en Aval N° " + i + 1);
                            return View(solicitudPrestamoDto);
                        }
                        if (solicitudPrestamoDto.AvalesDto[i].FechaNacimiento >= DateTime.Today)
                        {
                            ModelState.AddModelError(string.Empty,
                            "Fecha de Nacimiento del Aval no puede ser mayor o igual a la fecha de día, error en Aval N° " + i + 1);
                            return View(nameof(CreateStep2), solicitudPrestamoDto);
                        }
                    }

                    //Comprobar si existen duplicados las Cedula Identidad de Avales.
                    var hasDuplicates = listaCedulaIdentidadAvales.GroupBy(x => x).Any(g => g.Count() > 1);
                    if (hasDuplicates)
                    {
                        ModelState.AddModelError(string.Empty,
                        "Existe Cedula de Identidad duplicada entre los avales.");
                        return View(solicitudPrestamoDto);
                    }

                    //Tratar Avales
                    for (int i = 0; i < solicitudPrestamoDto.AvalesDto.Count; i++)
                    {
                        //Persona Aval
                        var persona = await _context.Personas
                        .FirstOrDefaultAsync(m => m.Id == solicitudPrestamoDto.AvalesDto[i].PersonaId);

                        //Evaluar si trajo datos la consulta de la BD
                        if (persona != null)
                        {
                            //Actualizar Persona Aval
                            PersonaDtoToModel(solicitudPrestamoDto.AvalesDto[i], persona);
                            _context.Update(persona);
                        }
                        else
                        {
                            //Crear Persona Aval
                            persona = new Persona();
                            PersonaDtoToModel(solicitudPrestamoDto.AvalesDto[i], persona);
                            _context.Add(persona);
                        }
                        await _context.SaveChangesAsync();

                        //Aval
                        var aval = await _context.Avales
                        .FirstOrDefaultAsync(m => m.Id == solicitudPrestamoDto.AvalesDto[i].Id);

                        //Evaluar si trajo datos la consulta de la BD
                        if (aval != null)
                        {
                            //Actualizar Aval
                            aval.SolicitudPrestamoId = solicitudPrestamoDto.Id;
                            aval.PersonaId = persona.Id;
                            _context.Update(aval);
                        }
                        else
                        {
                            //Crear Aval
                            aval = new Aval();
                            aval.SolicitudPrestamoId = solicitudPrestamoDto.Id;
                            aval.PersonaId = persona.Id;
                            aval.TasaCoberturaDeuda = 0;//TODO enlazar al producto.terminos
                            aval.UrlDocumento = "-";
                            _context.Add(aval);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolicitudPrestamoExists(solicitudPrestamoDto.Id))
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
            return View(solicitudPrestamoDto);
        }

        // GET: SolicitudesPrestamo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SolicitudesPrestamo == null)
            {
                return NotFound();
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .Include(s => s.Avales)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (solicitudPrestamo == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.
                Include(c => c.Persona).
                Include(c => c.OrigenesIngresoClientes).
                Include(c => c.EstadoCivil).
                Include(c => c.Conyuges).
                FirstOrDefaultAsync(m => m.Id == solicitudPrestamo.ClienteId);

            var personaConyuge = new Persona();

            if (cliente.EstadoCivil.RequiereDatosConyuge)
            {
                var conyuge = cliente.Conyuges.FirstOrDefault();
                personaConyuge = await _context.Personas.FirstOrDefaultAsync(c => c.Id == conyuge.PersonaId);
            }


            var producto = await _context.Productos
                .Include(p => p.Beneficios)
                .Include(p => p.Requisitos)
                .Include(p => p.Terminos)
                .FirstOrDefaultAsync(m => m.Id == solicitudPrestamo.ProductoId);

            var solicitudPrestamoDto = new SolicitudPrestamoDto();
            //Inicializar campos necesarios
            solicitudPrestamoDto.ClienteDto = new ClienteDto();
            solicitudPrestamoDto.ProductoDto = new ProductoDto();
            solicitudPrestamoDto.AvalesDto = new List<AvalDto>();

            foreach (var aval in solicitudPrestamo.Avales)
            {
                aval.Persona = await _context.Personas
                .FirstOrDefaultAsync(m => m.Id == aval.PersonaId);
            }

            SolicitudPrestamoModelToDto(solicitudPrestamo, solicitudPrestamoDto, cliente, personaConyuge, producto);

            return View(solicitudPrestamoDto);
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

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .Include(c => c.Avales)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (solicitudPrestamo != null)
            {
                //Eliminar SolicitudPrestamo
                _context.SolicitudesPrestamo.Remove(solicitudPrestamo);
                await _context.SaveChangesAsync();
                //Recorrer los avales asociados
                foreach (var aval in solicitudPrestamo.Avales)
                {
                    //Recuperar Persona asociada al Aval
                    var persona = await _context.Personas
                    .FirstOrDefaultAsync(m => m.Id == aval.PersonaId);
                    if (persona != null)
                    {
                        //Eliminar Persona
                        _context.Personas.Remove(persona);
                        await _context.SaveChangesAsync();
                    }

                    //Eliminar Aval
                    //_context.Avales.Remove(aval);
                    //await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SolicitudPrestamoExists(int id)
        {
            return (_context.SolicitudesPrestamo?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static void SolicitudPrestamoModelToDto(SolicitudPrestamo? solicitudPrestamo, SolicitudPrestamoDto solicitudPrestamoDto, Cliente? cliente, Persona? personaConyuge, Producto? producto)
        {
            solicitudPrestamoDto.Id = solicitudPrestamo.Id;
            solicitudPrestamoDto.ClienteId = solicitudPrestamo.Cliente.Id;
            ClienteModelToDto(cliente, solicitudPrestamoDto.ClienteDto, personaConyuge);
            solicitudPrestamoDto.ProductoId = solicitudPrestamo.Producto.Id;
            ProductoModelToDto(producto, solicitudPrestamoDto.ProductoDto);
            solicitudPrestamoDto.MontoSolicitado = solicitudPrestamo.MontoSolicitado;
            solicitudPrestamoDto.CantidadCuotas = solicitudPrestamo.CantidadCuotas;
            solicitudPrestamoDto.ValorCuota = solicitudPrestamo.ValorCuota;
            solicitudPrestamoDto.CostoTotalFinanciero = solicitudPrestamo.CostoTotalFinanciero;
            solicitudPrestamoDto.TasaCoberturaDeudaConyuge = solicitudPrestamo.TasaCoberturaDeudaConyuge;
            solicitudPrestamoDto.FechaSolicitud = solicitudPrestamo.FechaSolicitud;
            solicitudPrestamoDto.UrlDocumento = solicitudPrestamo.UrlDocumento;
            solicitudPrestamoDto.Estado = solicitudPrestamo.Estado;
            foreach (var aval in solicitudPrestamo.Avales)
            {
                var avalDto = new AvalDto();
                avalDto.Id = aval.Id;
                avalDto.PersonaId = aval.PersonaId;
                avalDto.CedulaIdentidad = aval.Persona.CedulaIdentidad;
                avalDto.Nombre = aval.Persona.Nombre;
                avalDto.Apellido = aval.Persona.Apellido;
                avalDto.FechaNacimiento = aval.Persona.FechaNacimiento;
                avalDto.GeneroId = aval.Persona.GeneroId;
                avalDto.Domicilio = aval.Persona.Domicilio;
                avalDto.CorreoElectronico = aval.Persona.CorreoElectronico;
                avalDto.Telefono = aval.Persona.Telefono;
                avalDto.NacionalidadId = aval.Persona.NacionalidadId;
                avalDto.TasaCoberturaDeuda = aval.TasaCoberturaDeuda;
                avalDto.UrlDocumento = aval.UrlDocumento;
                solicitudPrestamoDto.AvalesDto.Add(avalDto);
            }
        }

        private static void ClienteModelToDto(Cliente? cliente, ClienteDto clienteDto, Persona? personaConyuge)
        {
            clienteDto.Id = cliente.Id;
            clienteDto.DomicilioAlternativo = cliente.DomicilioAlternativo;
            clienteDto.TelefonoLaboral = cliente.TelefonoLaboral;
            clienteDto.PersonaPoliticamenteExpuesta = cliente.PersonaPoliticamenteExpuesta;
            clienteDto.EstadoCivilId = cliente.EstadoCivilId;
            clienteDto.Scoring = cliente.Scoring;
            //PersonaCliente
            clienteDto.PersonaId = cliente.Persona.Id;
            clienteDto.CedulaIdentidad = cliente.Persona.CedulaIdentidad;
            clienteDto.Nombre = cliente.Persona.Nombre;
            clienteDto.Apellido = cliente.Persona.Apellido;
            clienteDto.FechaNacimiento = cliente.Persona.FechaNacimiento;
            clienteDto.GeneroId = cliente.Persona.GeneroId;
            clienteDto.Domicilio = cliente.Persona.Domicilio;
            clienteDto.CorreoElectronico = cliente.Persona.CorreoElectronico;
            clienteDto.Telefono = cliente.Persona.Telefono;
            clienteDto.NacionalidadId = cliente.Persona.NacionalidadId;
            //PersonaConyugeConyugeId
            if (cliente.EstadoCivil.RequiereDatosConyuge)
            {
                clienteDto.ConyugeId = cliente.Conyuges.FirstOrDefault().Id;
                clienteDto.ConyugePersonaId = personaConyuge.Id;
                clienteDto.ConyugeCedulaIdentidad = personaConyuge.CedulaIdentidad;
                clienteDto.ConyugeNombre = personaConyuge.Nombre;
                clienteDto.ConyugeApellido = personaConyuge.Apellido;
                clienteDto.ConyugeFechaNacimiento = personaConyuge.FechaNacimiento;
                clienteDto.ConyugeGeneroId = personaConyuge.GeneroId;
                clienteDto.ConyugeDomicilio = personaConyuge.Domicilio;
                clienteDto.ConyugeCorreoElectronico = personaConyuge.CorreoElectronico;
                clienteDto.ConyugeTelefono = personaConyuge.Telefono;
                clienteDto.ConyugeNacionalidadId = personaConyuge.NacionalidadId;
            }
            //OrigenIngresoCliente
            var origenIngresoCliente = cliente.OrigenesIngresoClientes.FirstOrDefault();
            if (origenIngresoCliente != null)
            {
                clienteDto.OrigenIngresoId = origenIngresoCliente.Id;
                clienteDto.TipoActividadId = origenIngresoCliente.TipoActividadId;
                clienteDto.FechaInicioActividad = origenIngresoCliente.FechaInicioActividad;
                clienteDto.FechaFinActividad = origenIngresoCliente.FechaFinActividad;
                clienteDto.MontoLiquidoPercibido = origenIngresoCliente.MontoLiquidoPercibido;
            }
        }

        private static void ProductoModelToDto(Producto? producto, ProductoDto productoDto)
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
        private static void SolicitudPrestamoDtoToModel(SolicitudPrestamoDto solicitudPrestamoDto, SolicitudPrestamo solicitudPrestamo)
        {
            solicitudPrestamo.ClienteId = solicitudPrestamoDto.ClienteId;
            solicitudPrestamo.ProductoId = solicitudPrestamoDto.ProductoId;
            solicitudPrestamo.MontoSolicitado = solicitudPrestamoDto.MontoSolicitado;
            solicitudPrestamo.CantidadCuotas = solicitudPrestamoDto.CantidadCuotas;
            solicitudPrestamo.ValorCuota = solicitudPrestamoDto.ValorCuota;
            solicitudPrestamo.CostoTotalFinanciero = solicitudPrestamoDto.CostoTotalFinanciero;
            solicitudPrestamo.TasaCoberturaDeudaConyuge = solicitudPrestamoDto.TasaCoberturaDeudaConyuge;
            solicitudPrestamo.FechaSolicitud = solicitudPrestamoDto.FechaSolicitud;
            solicitudPrestamo.UrlDocumento = "-";
            solicitudPrestamo.Estado = "Analisis";
        }

        private static void PersonaDtoToModel(AvalDto avalDto, Persona persona)
        {
            persona.CedulaIdentidad = avalDto.CedulaIdentidad;
            persona.Nombre = avalDto.Nombre;
            persona.Apellido = avalDto.Apellido;
            persona.FechaNacimiento = avalDto.FechaNacimiento;
            persona.GeneroId = avalDto.GeneroId;
            persona.Domicilio = avalDto.Domicilio;
            persona.CorreoElectronico = avalDto.CorreoElectronico;
            persona.Telefono = avalDto.Telefono;
            persona.NacionalidadId = avalDto.NacionalidadId;
            persona.UrlImagen = "-";
            persona.UrlDocumento = "-";
            persona.DatosVerificados = true;
        }
    }
}
