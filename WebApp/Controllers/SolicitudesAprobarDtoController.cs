using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE")]
    public class SolicitudesAprobarDtoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesAprobarDtoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SolicitudesPrestamo
        public async Task<IActionResult> Index()
        {
            var solicitudesPrestamo = await _context.SolicitudesPrestamo
                .Include(s => s.Cliente)
                .Include(s => s.Producto)
                .Where(x => x.Estado == "Analisis")
                .OrderByDescending(s => s.Id).ToListAsync();

            foreach (var item in solicitudesPrestamo)
            {
                item.Cliente.Persona = await _context.Personas.FirstOrDefaultAsync(x => x.Id == item.Cliente.PersonaId);
            }

            var solicitudesPrestamoDto = new List<SolicitudPrestamoDto>();

            SolicitudesPrestamoModelToDto(solicitudesPrestamo, solicitudesPrestamoDto);

            return View(solicitudesPrestamoDto);
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

        // GET: SolicitudesAprobar/Aprove/5
        public async Task<IActionResult> Aprove(int? id)
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

        // POST: SolicitudesAprobar/Aprove/5
        [HttpPost, ActionName("Aprove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AproveConfirmed(int id)
        {
            if (_context.SolicitudesPrestamo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SolicitudPrestamo'  is null.");
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .FirstOrDefaultAsync(m => m.Id == id);

            if (solicitudPrestamo != null)
            {
                //Recuperar Beneficios/Requisitos/Terminos
                var producto = await _context.Productos
                    .Include(c => c.Terminos)
                    .FirstOrDefaultAsync(m => m.Id == solicitudPrestamo.ProductoId);

                //Crear PrestamoAprobado
                var prestamoAprobado = new PrestamoAprobado();
                solicitudPrestamoModel(solicitudPrestamo, producto, prestamoAprobado);
                _context.PrestamosAprobado.Add(prestamoAprobado);
                await _context.SaveChangesAsync();

                //Crear Cuotas
                var cuotas = new List<Cuota>();
                cuotasModel(cuotas, producto, prestamoAprobado);
                foreach (var cuota in cuotas)
                {
                    _context.Cuotas.Add(cuota);
                    await _context.SaveChangesAsync();                    
                }               

                //Actualizar SolicitudPrestamo
                solicitudPrestamo.Estado = "Aprobado";
                _context.SolicitudesPrestamo.Update(solicitudPrestamo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: SolicitudesAprobar/Reject/5
        public async Task<IActionResult> Reject(int? id)
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

        // POST: SolicitudesAprobar/Reject/5
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectConfirmed(int id)
        {
            if (_context.SolicitudesPrestamo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SolicitudPrestamo'  is null.");
            }

            var solicitudPrestamo = await _context.SolicitudesPrestamo
                .FirstOrDefaultAsync(m => m.Id == id);

            if (solicitudPrestamo != null)
            {
                solicitudPrestamo.Estado = "Rechazado";
                //Actualizar SolicitudPrestamo
                _context.SolicitudesPrestamo.Update(solicitudPrestamo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private static void SolicitudesPrestamoModelToDto(List<SolicitudPrestamo>? solicitudesPrestamo, List<SolicitudPrestamoDto> solicitudesPrestamoDto)
        {
            foreach (var item in solicitudesPrestamo)
            {
                var solicitudPrestamoDto = new SolicitudPrestamoDto();
                solicitudPrestamoDto.Id = item.Id;
                solicitudPrestamoDto.ClienteDto = new ClienteDto();
                solicitudPrestamoDto.ClienteDto.Id = item.ClienteId;
                solicitudPrestamoDto.ClienteDto.CedulaIdentidad = item.Cliente.Persona.CedulaIdentidad;
                solicitudPrestamoDto.ProductoDto = new ProductoDto();   
                solicitudPrestamoDto.ProductoDto.Id = item.ProductoId;
                solicitudPrestamoDto.ProductoDto.Descripcion = item.Producto.Descripcion;
                solicitudPrestamoDto.MontoSolicitado = item.MontoSolicitado;
                solicitudPrestamoDto.CantidadCuotas = item.CantidadCuotas;
                solicitudPrestamoDto.ValorCuota = item.ValorCuota;
                solicitudPrestamoDto.CostoTotalFinanciero = item.CostoTotalFinanciero;
                solicitudPrestamoDto.FechaSolicitud = item.FechaSolicitud;
                solicitudPrestamoDto.Estado = item.Estado;
                solicitudesPrestamoDto.Add(solicitudPrestamoDto);
            }
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
            solicitudPrestamoDto.CantidadAvales = solicitudPrestamo.Avales.Count();
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

        private static void solicitudPrestamoModel(SolicitudPrestamo? solicitudPrestamo, Producto? producto, PrestamoAprobado prestamoAprobado)
        {
            prestamoAprobado.SolicitudPrestamoId = solicitudPrestamo.Id;
            prestamoAprobado.MontoAprobado = solicitudPrestamo.MontoSolicitado;
            prestamoAprobado.CantidadCuotas = solicitudPrestamo.CantidadCuotas;
            prestamoAprobado.ValorCuota = solicitudPrestamo.ValorCuota;
            prestamoAprobado.CostoTotalFinanciero = solicitudPrestamo.CostoTotalFinanciero;
            prestamoAprobado.MontoInteres = (producto.Terminos.TasaNominal * solicitudPrestamo.MontoSolicitado) / 100;
            prestamoAprobado.MontoGastosContratacion = (producto.Terminos.TasaGastosAdministrativos * solicitudPrestamo.MontoSolicitado) / 100;
            prestamoAprobado.MontoGastosMantencion = ((producto.Terminos.TasaGastosCobranza * solicitudPrestamo.MontoSolicitado) / 100) * solicitudPrestamo.CantidadCuotas;
            prestamoAprobado.MontoSeguros = ((producto.Terminos.TasaGastosCobranza * solicitudPrestamo.MontoSolicitado) / 100) * solicitudPrestamo.CantidadCuotas;
            prestamoAprobado.FechaAprobacion = DateTime.Today;
            prestamoAprobado.FechaDesembolso = DateTime.Today;
            prestamoAprobado.FechaPrimerVencimiento = DateTime.Today.AddMonths(1);
            prestamoAprobado.UrlDocumento = "-";
            prestamoAprobado.Estado = "Aprobado";
        }

        private static void cuotasModel(List<Cuota>? cuotas, Producto? producto, PrestamoAprobado prestamoAprobado)
        {
            for (int i = 0; i < prestamoAprobado.CantidadCuotas; i++)
            {
                var cuota = new Cuota();
                cuota.PrestamoAprobadoId = prestamoAprobado.Id;
                cuota.NumeroCuota = i+1;
                cuota.FechaVencimiento = prestamoAprobado.FechaPrimerVencimiento.AddMonths(i+1);
                cuota.MontoCapital = prestamoAprobado.MontoAprobado / prestamoAprobado.CantidadCuotas;
                cuota.MontoInteres = prestamoAprobado.MontoInteres / prestamoAprobado.CantidadCuotas;
                cuota.MontoGastos = (prestamoAprobado.MontoGastosContratacion + prestamoAprobado.MontoGastosMantencion) / prestamoAprobado.CantidadCuotas;
                cuota.MontoSeguros = prestamoAprobado.MontoSeguros / prestamoAprobado.CantidadCuotas;
                cuota.UrlDocumento = "-";
                cuota.Estado = "Pendiente";
                cuotas.Add(cuota);
            }

        }
    }
}
