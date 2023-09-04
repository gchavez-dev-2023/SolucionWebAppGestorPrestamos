using DinkToPdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Helpers;
using WebApp.Models;

namespace WebApp.Controllers
{
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

            solicitudPrestamoModelToDto(solicitudPrestamo, solicitudPrestamoDto, cliente, personaConyuge, producto);

            return View(solicitudPrestamoDto);
        }

        // GET: SolicitudesPrestamo/CreateStep1
        public IActionResult CreateStep1()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes.Include(c => c.Persona), "Id", "Persona.CedulaIdentidad");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion");
            return View();
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

            for (int i = 0; i < solicitudPrestamoDto.CantidadAvales; i++)
            {
                var avalDto = new AvalDto();
                solicitudPrestamoDto.AvalesDto.Add(avalDto);
            }

            //if (ModelState.IsValid)
            //{
            //    return View(solicitudPrestamoDto);
            //}
            //else
            //{
            //    return RedirectToAction(nameof(CreateStep1));
            //}
            return View(solicitudPrestamoDto);
        }

        // POST: SolicitudesPrestamo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,ClienteDto,ProductoId,ProductoDto,MontoSolicitado,CantidadCuotas,ValorCuota,CostoTotalFinanciero,TasaCoberturaDeudaConyuge,FechaSolicitud,UrlDocumento,Estado,CantidadAvales,AvalesDto")] SolicitudPrestamoDto solicitudPrestamoDto)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(solicitudPrestamoDto);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", solicitudPrestamoDto.ClienteId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", solicitudPrestamoDto.ProductoId);
            //return View(nameof(CreateStep2), solicitudPrestamoDto);
            for (int i = 0; i < solicitudPrestamoDto.AvalesDto.Count; i++)
            {
                if (solicitudPrestamoDto.AvalesDto[i].CedulaIdentidad == "1234")
                {
                    ModelState.AddModelError(string.Empty,
                    "No se completo la Cedula Identidad del Aval N° " + i+1);
                    return View(nameof(CreateStep2), solicitudPrestamoDto);
                }
            }
            return RedirectToAction(nameof(Index));
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", solicitudPrestamo.ProductoId);
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Descripcion", solicitudPrestamo.ProductoId);
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

            solicitudPrestamoModelToDto(solicitudPrestamo, solicitudPrestamoDto, cliente, personaConyuge, producto);

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
            var solicitudPrestamo = await _context.SolicitudesPrestamo.FindAsync(id);
            if (solicitudPrestamo != null)
            {
                _context.SolicitudesPrestamo.Remove(solicitudPrestamo);
                await _context.SaveChangesAsync();
            }

            var avales = await _context.Avales.FindAsync(solicitudPrestamo.Id);
            if (avales != null)
            {
                foreach (var aval in solicitudPrestamo.Avales)
                {
                    _context.Avales.Remove(aval);
                    await _context.SaveChangesAsync();

                    var persona = await _context.Personas
                    .FirstOrDefaultAsync(m => m.Id == aval.PersonaId);
                    if (persona != null)
                    {
                        _context.Personas.Remove(persona);
                        await _context.SaveChangesAsync();
                    }                        
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SolicitudPrestamoExists(int id)
        {
            return (_context.SolicitudesPrestamo?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static void solicitudPrestamoModelToDto(SolicitudPrestamo? solicitudPrestamo, SolicitudPrestamoDto solicitudPrestamoDto, Cliente? cliente, Persona? personaConyuge, Producto? producto)
        {
            solicitudPrestamoDto.Id = solicitudPrestamo.Id;
            solicitudPrestamoDto.ClienteId = solicitudPrestamo.Cliente.Id;
            clienteModelToDto(cliente, solicitudPrestamoDto.ClienteDto, personaConyuge);
            solicitudPrestamoDto.ProductoId = solicitudPrestamo.Producto.Id;
            productoModelToDto(producto, solicitudPrestamoDto.ProductoDto);
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

        private static void clienteModelToDto(Cliente? cliente, ClienteDto clienteDto, Persona? personaConyuge)
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

    }
}
