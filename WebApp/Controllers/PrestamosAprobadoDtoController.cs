using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE,COLABORADOR")]
    public class PrestamosAprobadoDtoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrestamosAprobadoDtoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrestamosAprobadoDto
        public async Task<IActionResult> Index()
        {
            var prestamosAprobado = await _context.PrestamosAprobado
                .Include(s => s.SolicitudPrestamo)
                .Where(x => x.Estado == "Aprobado")
                .OrderByDescending(s => s.Id).ToListAsync();

            var prestamosAprobadoDto = new List<PrestamoAprobadoDto>();
            await PrestamosAprobadoModelToDto(prestamosAprobado, prestamosAprobadoDto);

            return View(prestamosAprobadoDto);
        }

        // GET: PrestamosAprobadoDto/CuotasIndex/5
        public async Task<IActionResult> CuotasIndex(int? id)
        {
            var cuotas = await _context.Cuotas
                .Include(c => c.CuotasPagos)
                .Where(x => x.PrestamoAprobadoId == id)
                .OrderBy(s => s.NumeroCuota).ToListAsync();

            var cuotasDto = new List<CuotaDto>();
            CuotasModelToDto(cuotas, cuotasDto);

            return View(cuotasDto);

        }

        // GET: PrestamosAprobadoDto/CuotaPagosIndex/5
        public async Task<IActionResult> CuotaPagosIndex(int? id)
        {
            var cuotaPagos = await _context.CuotasPagos
                .Include(c => c.Cuota)
                .Include(c => c.Pago)
                .Where(x => x.CuotaId == id)
                .OrderBy(s => s.Pago.FechaPago)
                .ToListAsync();

            var pagosDto = new List<PagoDto>();
            PagosModelToDto(cuotaPagos, pagosDto);

            //Enviar el Id del Prestamo
            var cuota = await _context.Cuotas
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (cuota != null)
            {
                ViewData["CuotaId"] = cuota.Id;
                ViewData["PrestamosAprobadoId"] = cuota.PrestamoAprobadoId;
                ViewData["NumeroCuota"] = cuota.NumeroCuota;
            }

            return View(pagosDto);

        }

        // GET: PrestamosAprobadoDto/CuotaPagoCreate
        public async Task<IActionResult> CuotaPagoCreate(int? id)
        {
            var pagoDto = new PagoDto();
            pagoDto.CuotaId = id;
            pagoDto.MontoMinimoPago = 1;
            pagoDto.MontoMaximoPago = 0;

            if (id != null)
            {
                var cuota = await _context.Cuotas
                    .Include(c => c.PrestamoAprobado)
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (cuota != null)
                {
                    pagoDto.MontoMaximoPago = cuota.MontoCapital + cuota.MontoInteres + cuota.MontoGastos + cuota.MontoSeguros + cuota.MontoMora + cuota.MontoCastigo;
                    pagoDto.PrestamoAprobadoId = cuota.PrestamoAprobadoId;
                    var persona = await _context.SolicitudesPrestamo
                        .Include(c => c.Cliente)
                        .Where(x => x.Id == cuota.PrestamoAprobado.SolicitudPrestamoId)
                        .FirstOrDefaultAsync();
                    pagoDto.PersonaId = persona.Cliente.PersonaId;
                }

                var cuotasPagos = await _context.CuotasPagos
                    .Where(x => x.CuotaId == id)
                    .ToListAsync();
                if (cuotasPagos != null)
                {
                    pagoDto.MontoMaximoPago = pagoDto.MontoMaximoPago - (cuotasPagos.Sum(c => c.MontoCapital) + cuotasPagos.Sum(c => c.MontoInteres) + cuotasPagos.Sum(c => c.MontoGastos) + cuotasPagos.Sum(c => c.MontoSeguros) + cuotasPagos.Sum(c => c.MontoMora) + cuotasPagos.Sum(c => c.MontoCastigo));
                }
            }

            ViewData["FormaPagoId"] = new SelectList(_context.FormasPago, "Id", "Descripcion");
            //ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad");
            return View(pagoDto);
        }

        // POST: PrestamosAprobadoDto/CuotaPagoCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CuotaPagoCreate([Bind("Id, CuotaId, NumeroCuota, FechaPago, MontoPago, FormaPagoId, PersonaId, PrestamoAprobadoId, MontoMinimoPago, MontoMaximoPago")] PagoDto pagoDto)
        {
            if (ModelState.IsValid)
            {
                //Crear nuevo Pago
                var pago = new Pago();
                PagoDtoToModel(pagoDto, pago);
                _context.Add(pago);
                await _context.SaveChangesAsync();

                //Crear nuevo CuotaPago
                var cuotaPago = new CuotaPago();
                cuotaPago.PagoId = pago.Id;
                await CuotaPagoCalculo(pagoDto, cuotaPago);
                _context.Add(cuotaPago);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(CuotaPagosIndex), new { id = pagoDto.CuotaId });
            }
            ViewData["FormaPagoId"] = new SelectList(_context.FormasPago, "Id", "Descripcion", pagoDto.FormaPagoId);
            //ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", pagoDto.PersonaId);
            return View(pagoDto);
        }


        // GET: PrestamosAprobadoDto/CuotaPagoDetails/5
        public async Task<IActionResult> CuotaPagoDetails(int? id)
        {
            if (id == null || _context.CuotasPagos == null)
            {
                return NotFound();
            }

            //Recuperar Cuota Pago
            var cuotaPago = await _context.CuotasPagos
                .Include(p => p.Cuota)
                .Include(p => p.Pago)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            if (cuotaPago == null)
            {
                return NotFound();
            }
            var formaDePago = await _context.FormasPago.Where(x => x.Id == cuotaPago.Pago.FormaPagoId).FirstOrDefaultAsync();
            var pagoDto = new PagoDto();
            PagoModelToDto(cuotaPago, pagoDto, formaDePago);

            return View(pagoDto);
        }

        // GET: PrestamosAprobadoDto/CuotaPagoEdit/5
        public async Task<IActionResult> CuotaPagoEdit(int? id)
        {
            if (id == null || _context.CuotasPagos == null)
            {
                return NotFound();
            }

            //Recuperar Cuota Pago
            var cuotaPago = await _context.CuotasPagos
                .Include(p => p.Cuota)
                .Include(p => p.Pago)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            var pagoDto = new PagoDto();

            if (cuotaPago == null)
            {
                return NotFound();
            }
            else
            {
                pagoDto.MontoMaximoPago = cuotaPago.Cuota.MontoCapital + cuotaPago.Cuota.MontoInteres + cuotaPago.Cuota.MontoGastos + cuotaPago.Cuota.MontoSeguros + cuotaPago.Cuota.MontoMora + cuotaPago.Cuota.MontoCastigo;
                pagoDto.PrestamoAprobadoId = cuotaPago.Cuota.PrestamoAprobadoId;

                var cuotasPagos = await _context.CuotasPagos
                    .Where(x => x.CuotaId == cuotaPago.CuotaId
                             && x.Id != cuotaPago.Id)
                    .ToListAsync();
                if (cuotasPagos != null)
                {
                    pagoDto.MontoMaximoPago = pagoDto.MontoMaximoPago - (cuotasPagos.Sum(c => c.MontoCapital) + cuotasPagos.Sum(c => c.MontoInteres) + cuotasPagos.Sum(c => c.MontoGastos) + cuotasPagos.Sum(c => c.MontoSeguros) + cuotasPagos.Sum(c => c.MontoMora) + cuotasPagos.Sum(c => c.MontoCastigo));
                }
            }

            var formaDePago = await _context.FormasPago.Where(x => x.Id == cuotaPago.Pago.FormaPagoId).FirstOrDefaultAsync();
            PagoModelToDto(cuotaPago, pagoDto, formaDePago);

            ViewData["FormaPagoId"] = new SelectList(_context.FormasPago, "Id", "Descripcion", pagoDto.FormaPagoId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", pagoDto.PersonaId);

            return View(pagoDto);
        }

        // POST: PrestamosAprobadoDto/CuotaPagoEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CuotaPagoEdit(int id, [Bind("Id, CuotaId, NumeroCuota, FechaPago, MontoPago, FormaPagoId, PersonaId, PrestamoAprobadoId, MontoMinimoPago, MontoMaximoPago")] PagoDto pagoDto)
        {
            if (id != pagoDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Recuperar Cuota Pago
                    var cuotaPago = await _context.CuotasPagos
                        .Include(p => p.Cuota)
                        .Include(p => p.Pago)
                        .Where(m => m.Id == id)
                        .FirstOrDefaultAsync();

                    //Evaluar si trajo datos la consulta de la BD
                    if (cuotaPago.Pago != null)
                    {
                        //Actualizar Pago
                        PagoDtoToModel(pagoDto, cuotaPago.Pago);
                        _context.Update(cuotaPago.Pago);
                    }
                    else
                    {
                        //Crear Pago
                        var pago = new Pago();
                        PagoDtoToModel(pagoDto, pago);
                        _context.Add(pago);
                    }
                    await _context.SaveChangesAsync();

                    //Eliminar CuotaPago
                    if (cuotaPago != null)
                    {
                        _context.CuotasPagos.Remove(cuotaPago);
                        await _context.SaveChangesAsync();
                    }

                    //Crear nuevo CuotaPago
                    var cuotaPagoCreate = new CuotaPago();
                    cuotaPagoCreate.PagoId = cuotaPago.PagoId;
                    await CuotaPagoCalculo(pagoDto, cuotaPagoCreate);
                    _context.Add(cuotaPagoCreate);
                    await _context.SaveChangesAsync();

                    //Termino OK
                    return RedirectToAction(nameof(CuotaPagosIndex), new { id = pagoDto.CuotaId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pagoDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["FormaPagoId"] = new SelectList(_context.FormasPago, "Id", "Descripcion", pagoDto.FormaPagoId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", pagoDto.PersonaId);

            return View(pagoDto);
        }

        // GET: PrestamosAprobadoDto/CuotaPagoDelete/5
        public async Task<IActionResult> CuotaPagoDelete(int? id)
        {
            if (id == null || _context.CuotasPagos == null)
            {
                return NotFound();
            }

            var cuotaPago = await _context.CuotasPagos
                .Include(p => p.Cuota)
                .Include(p => p.Pago)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            if (cuotaPago == null)
            {
                return NotFound();
            }

            var formaDePago = await _context.FormasPago.Where(x => x.Id == cuotaPago.Pago.FormaPagoId).FirstOrDefaultAsync();
            var pagoDto = new PagoDto();
            PagoModelToDto(cuotaPago, pagoDto, formaDePago);

            return View(pagoDto);
        }

        // POST: PrestamosAprobadoDto/CuotaPagoDelete/5
        [HttpPost, ActionName("CuotaPagoDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CuotasPagos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CuotasPagos'  is null.");
            }

            var cuotaPago = await _context.CuotasPagos
                .Include(c => c.Pago)
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            if (cuotaPago != null)
            {
                _context.Pagos.Remove(cuotaPago.Pago);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CuotaPagosIndex), new { id = cuotaPago.CuotaId });
        }

        // GET: PrestamosAprobadoDto/PrestamoPagoCreate
        public async Task<IActionResult> PrestamoPagoCreate(int? id)
        {
            var pagoDto = new PagoDto();

            if (id != null)
            {
                pagoDto.PrestamoAprobadoId = (int)id;

                bool esPrepagable = false;
                decimal montoAdeudado = 0;

                var prestamoAprobadoDb = _context.PrestamosAprobado;
                var solicitudPrestamoDb = _context.SolicitudesPrestamo;
                var productoDb = _context.Productos;
                var terminoDb = _context.Terminos;
                var cuotaDb = _context.Cuotas;
                var pagoDb = _context.Pagos;

                var esPrepagableDb = await (from a in prestamoAprobadoDb
                                            join s in solicitudPrestamoDb on a.SolicitudPrestamoId equals s.Id
                                            join p in productoDb on s.ProductoId equals p.Id
                                            join t in terminoDb on p.TerminosId equals t.Id
                                            where a.Id == id
                                            select new
                                            {
                                                t.EsPrepagable
                                            }).FirstOrDefaultAsync();

                if (esPrepagableDb != null)
                {
                    esPrepagable = esPrepagableDb.EsPrepagable;
                }

                //EsPrepagable
                if (esPrepagable)
                {
                    //var montoAdeudadoDb = (from a in prestamoAprobadoDb
                    //                             join c in cuotaDb on a.Id equals c.PrestamoAprobadoId
                    //                             join p in pagoDb on c.Id equals p.PrestamoAprobadoId into acp
                    //                             where a.Id == id && c.FechaVencimiento <= DateTime.Now.AddDays(15)
                    //                             group c by new { a.Id } into g
                    //                             select new
                    //                             {
                    //                                 montoTotalCuota = g.Sum(c => c.MontoTotalCuota),
                    //                                 //montoTotalPago = g.Sum()
                    //                             });
                }

                //var montoAdeudado = await _context.Cuotas.where
            }
            pagoDto.MontoMinimoPago = 1;
            pagoDto.MontoMaximoPago = 100;

            ViewData["FormaPagoId"] = new SelectList(_context.FormasPago, "Id", "Descripcion");
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad");
            return View(pagoDto);
        }

        // POST: PrestamosAprobadoDto/PrestamoPagoCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrestamoPagoCreate([Bind("Id, CuotaId, NumeroCuota, FechaPago, MontoPago, FormaPagoId, PersonaId, PrestamoAprobadoId, MontoMinimoPago, MontoMaximoPago")] PagoDto pagoDto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pagoDto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FormaPagoId"] = new SelectList(_context.FormasPago, "Id", "Descripcion", pagoDto.FormaPagoId);
            ViewData["PersonaId"] = new SelectList(_context.Personas, "Id", "CedulaIdentidad", pagoDto.PersonaId);
            return View(pagoDto);
        }


        private async Task PrestamosAprobadoModelToDto(List<PrestamoAprobado>? prestamosAprobado, List<PrestamoAprobadoDto> prestamosAprobadoDto)
        {
            foreach (var item in prestamosAprobado)
            {
                var prestamoAprobadoDto = new PrestamoAprobadoDto();
                prestamoAprobadoDto.Id = item.Id;
                prestamoAprobadoDto.ProductoId = item.SolicitudPrestamo.ProductoId;
                //completar llamar a BD y pasar del Model to DTO
                var productoDto = new ProductoDto();
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(x => x.Id == item.SolicitudPrestamo.ProductoId);
                if (producto != null)
                {
                    productoDto.Id = producto.Id;
                    productoDto.Descripcion = producto.Descripcion;
                }
                prestamoAprobadoDto.ProductoDto = productoDto;
                //
                prestamoAprobadoDto.ClienteId = item.SolicitudPrestamo.ClienteId;
                //completar llamar a BD y pasar del Model to DTO
                var clienteDto = new ClienteDto();
                var cliente = await _context.Clientes
                    .Include(c => c.Persona)
                    .FirstOrDefaultAsync(x => x.Id == item.SolicitudPrestamo.ClienteId);
                if (cliente != null)
                {
                    clienteDto.Id = cliente.Id;
                    clienteDto.CedulaIdentidad = cliente.Persona.CedulaIdentidad;
                }
                prestamoAprobadoDto.ClienteDto = clienteDto;
                //
                prestamoAprobadoDto.PrestamoTotal = item.MontoAprobado + item.MontoInteres + item.MontoGastosContratacion + item.MontoGastosMantencion + item.MontoSeguros + item.MontoMora + item.MontoCastigo;
                //Calcular pago
                decimal totalPagado = 0;
                var montoTotalPagos = await _context.Pagos.Where(x => x.PrestamoAprobadoId == prestamoAprobadoDto.Id).SumAsync(s => s.MontoPago);
                if (montoTotalPagos != null)
                {
                    totalPagado += montoTotalPagos;
                }
                //foreach (var cuota in item.Cuotas)
                //{
                //    //var montoTotalPagos = await _context.Pagos.Where(x => x.CuotaId == cuota.Id).SumAsync(s => s.MontoPago);
                //    //totalPagado += montoTotalPagos;
                //}

                prestamoAprobadoDto.PrestamoTotalPagado = totalPagado;
                prestamoAprobadoDto.PrestamoTotalDeuda = prestamoAprobadoDto.PrestamoTotal - prestamoAprobadoDto.PrestamoTotalPagado;
                prestamoAprobadoDto.MontoAprobado = item.MontoAprobado;
                prestamoAprobadoDto.CantidadCuotas = item.CantidadCuotas;
                prestamoAprobadoDto.ValorCuota = item.ValorCuota;
                prestamoAprobadoDto.CostoTotalFinanciero = item.CostoTotalFinanciero;
                prestamoAprobadoDto.FechaAprobacion = item.FechaAprobacion;
                prestamoAprobadoDto.Estado = item.Estado;
                prestamosAprobadoDto.Add(prestamoAprobadoDto);
            }
        }

        private static void CuotasModelToDto(List<Cuota> cuotas, List<CuotaDto> cuotasDto)
        {
            foreach (var item in cuotas)
            {
                var cuotaDto = new CuotaDto();
                cuotaDto.Id = item.Id;
                cuotaDto.PrestamoAprobadoId = item.PrestamoAprobadoId;
                cuotaDto.NumeroCuota = item.NumeroCuota;
                cuotaDto.FechaVencimiento = item.FechaVencimiento;
                cuotaDto.MontoTotalCuota = item.MontoCapital + item.MontoInteres + item.MontoGastos + item.MontoSeguros + item.MontoMora + item.MontoCastigo;
                cuotaDto.CuotaTotalPagado = item.CuotasPagos.Sum(x => x.MontoCapital) + item.CuotasPagos.Sum(x => x.MontoInteres) + item.CuotasPagos.Sum(x => x.MontoGastos) + item.CuotasPagos.Sum(x => x.MontoSeguros) + item.CuotasPagos.Sum(x => x.MontoMora) + item.CuotasPagos.Sum(x => x.MontoCastigo);
                cuotaDto.CuotaTotalDeuda = cuotaDto.MontoTotalCuota - cuotaDto.CuotaTotalPagado;
                cuotasDto.Add(cuotaDto);
            }
        }

        private static void PagosModelToDto(List<CuotaPago> cuotaPagos, List<PagoDto> pagosDto)
        {
            foreach (var item in cuotaPagos)
            {
                var pagoDto = new PagoDto();
                pagoDto.Id = item.Id;
                pagoDto.PrestamoAprobadoId = item.Cuota.PrestamoAprobadoId;
                pagoDto.NumeroCuota = item.Cuota.NumeroCuota;
                pagoDto.FechaPago = item.Pago.FechaPago;
                pagoDto.MontoPago = item.Pago.MontoPago;
                pagosDto.Add(pagoDto);
            }
        }
    
        private static void PagoModelToDto(CuotaPago? cuotaPago, PagoDto pagoDto, FormaPago? formaDePago)
        {
            pagoDto.Id = cuotaPago.Id;
            pagoDto.CuotaId = cuotaPago.CuotaId;
            pagoDto.PrestamoAprobadoId = cuotaPago.Cuota.PrestamoAprobadoId;
            pagoDto.NumeroCuota = cuotaPago.Cuota.NumeroCuota;
            pagoDto.FechaPago = cuotaPago.Pago.FechaPago;
            pagoDto.MontoPago = cuotaPago.Pago.MontoPago;
            pagoDto.PersonaId = cuotaPago.Pago.PersonaId;
            pagoDto.FormaPagoId = cuotaPago.Pago.FormaPagoId;
            pagoDto.FormaPagoDescripcion = formaDePago.Descripcion;
        }

        private static void PagoDtoToModel(PagoDto pagoDto, Pago pago)
        {
            pago.PrestamoAprobadoId = pagoDto.PrestamoAprobadoId;
            pago.FechaPago = DateTime.Now;
            pago.MontoPago = pagoDto.MontoPago;
            pago.FormaPagoId = pagoDto.FormaPagoId;
            pago.PersonaId = pagoDto.PersonaId;
            pago.UrlDocumento = "-";
        }

        private async Task CuotaPagoCalculo(PagoDto pagoDto, CuotaPago cuotaPago)
        {
            //Calcular Cuota Pago
            var cuota = await _context.Cuotas
                .Include(c => c.PrestamoAprobado)
                .Where(x => x.Id == pagoDto.CuotaId).FirstOrDefaultAsync();

            decimal montoPago = pagoDto.MontoPago;
            decimal castigo = 0;
            decimal mora = 0;
            decimal seguro = 0;
            decimal gasto = 0;
            decimal interes = 0;
            decimal capital = 0;

            if (cuota != null)
            {
                castigo = cuota.MontoCastigo;
                mora = cuota.MontoMora;
                seguro = cuota.MontoSeguros;
                gasto = cuota.MontoGastos;
                interes = cuota.MontoInteres;
                capital = cuota.MontoCapital;
                cuotaPago.CuotaId = cuota.Id;
            }

            var cuotasPagos = await _context.CuotasPagos
                .Where(x => x.CuotaId == pagoDto.CuotaId)
                .ToListAsync();
            if (cuotasPagos != null)
            {
                castigo -= cuotasPagos.Sum(c => c.MontoCastigo);
                mora -= cuotasPagos.Sum(c => c.MontoMora);
                seguro -= cuotasPagos.Sum(c => c.MontoSeguros);
                gasto -= cuotasPagos.Sum(c => c.MontoGastos);
                interes -= cuotasPagos.Sum(c => c.MontoInteres);
                capital -= cuotasPagos.Sum(c => c.MontoCapital);
            }

            cuotaPago.MontoCastigo = 0;
            cuotaPago.MontoMora = 0;
            cuotaPago.MontoSeguros = 0;
            cuotaPago.MontoGastos = 0;
            cuotaPago.MontoInteres = 0;
            cuotaPago.MontoCapital = 0;

            // validar pago segun orden de prelacion
            if(castigo > 0)
            {
                if (castigo >= montoPago)
                {
                    cuotaPago.MontoCastigo = montoPago;
                    return;
                }
                else
                {
                    montoPago -= castigo;
                    cuotaPago.MontoCastigo = castigo;
                }
            }
            if(mora > 0)
            {
                if(mora >= montoPago)
                {
                    cuotaPago.MontoMora = montoPago;
                    return;
                }
                else
                {
                    montoPago -= mora;
                    cuotaPago.MontoMora = mora;
                }
            }
            if(seguro > 0)
            {
                if(seguro >= montoPago)
                {
                    cuotaPago.MontoSeguros = montoPago;
                    return;
                }
                else
                {
                    montoPago -= seguro;
                    cuotaPago.MontoSeguros= seguro;
                }
            }
            if(gasto > 0)
            {
                if(gasto >= montoPago)
                {
                    cuotaPago.MontoGastos = montoPago;
                    return;
                }
                else
                {
                    montoPago -= gasto;
                    cuotaPago.MontoGastos = gasto;
                }
            }
            if(interes > 0)
            {
                if(interes >= montoPago)
                {
                    cuotaPago.MontoInteres = montoPago;
                    return;
                }
                else
                {
                    montoPago -= interes;
                    cuotaPago.MontoInteres = interes;
                }
            }
            if(capital > 0)
            {
                if (capital >= montoPago)
                {
                    cuotaPago.MontoCapital = montoPago;
                    return;
                }
            }
        }


        private bool PagoExists(int id)
        {
            return (_context.Pagos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
