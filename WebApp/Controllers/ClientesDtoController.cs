﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Controllers
{

    [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE,COLABORADOR")]
    public class ClientesDtoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesDtoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                .Include(c => c.EstadoCivil)
                .Include(c => c.Persona)
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var clientesDto = new List<ClienteDto>();

            ClientesModelToDto(clientes, clientesDto);

            return View(clientesDto);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.
                Include(c => c.Persona).
                Include(c => c.OrigenesIngresoClientes).
                Include(c => c.EstadoCivil).
                Include(c => c.Conyuges).
                FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var personaConyuge = new Persona();
            
            //Recuperar datos de las descripciones de campos foraneos
            var genero = await _context.Generos.ToListAsync();
            var nacionalidad = await _context.Nacionalidades.ToListAsync();
            var estadoCivil = await _context.EstadosCivil.ToListAsync();
            var tipoactividad = await _context.TiposActividad.ToListAsync();

            ViewData["Genero"] = genero.FirstOrDefault(x  => x.Id == cliente.Persona.GeneroId).Descripcion;
            ViewData["Nacionalidad"] = nacionalidad.FirstOrDefault(x => x.Id == cliente.Persona.NacionalidadId).Descripcion;
            ViewData["EstadoCivil"] = estadoCivil.FirstOrDefault(x => x.Id == cliente.EstadoCivilId).Descripcion;
            ViewData["TipoActividad"] = tipoactividad.FirstOrDefault(x => x.Id == cliente.OrigenesIngresoClientes.FirstOrDefault().TipoActividadId).Descripcion;

            if (cliente.EstadoCivil.RequiereDatosConyuge)
            {
                var conyuge = cliente.Conyuges.FirstOrDefault();
                personaConyuge = await _context.Personas.FirstOrDefaultAsync(c => c.Id == conyuge.PersonaId);

                ViewData["ConyugeGenero"] = genero.FirstOrDefault(x => x.Id == personaConyuge.GeneroId).Descripcion;
                ViewData["ConyugeNacionalidad"] = nacionalidad.FirstOrDefault(x => x.Id == personaConyuge.NacionalidadId).Descripcion;
            }

            var clienteDto = new ClienteDto();
            ClienteModelToDto(cliente, clienteDto, personaConyuge);

            return View(clienteDto);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion");
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Descripcion");
            ViewData["ConyugeGeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["ConyugeNacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");

            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, PersonaId, CedulaIdentidad, Nombre, Apellido, FechaNacimiento, GeneroId, Domicilio, CorreoElectronico, Telefono, NacionalidadId, DomicilioAlternativo, TelefonoLaboral, PersonaPoliticamenteExpuesta, OrigenIngresoId, TipoActividadId, FechaInicioActividad, FechaFinActividad, MontoLiquidoPercibido, EstadoCivilId, RequiereDatosConyuge, ConyugeId, ConyugePersonaId, ConyugeCedulaIdentidad, ConyugeNombre, ConyugeApellido, ConyugeFechaNacimiento, ConyugeGeneroId, ConyugeDomicilio, ConyugeCorreoElectronico, ConyugeTelefono, ConyugeNacionalidadId, Scoring, DatosVerificados")] ClienteDto clienteDto)
        {
            if (ModelState.IsValid)
            {
                ReglasNegocioCliente(clienteDto);
            }

            if (ModelState.IsValid)
            {
                bool existePersonaCliente = false;
                bool requiereDatosConyuge = false;
                bool existePersonaConyuge = false;

                //evaluar si existe Persona Cliente
                existePersonaCliente = await _context.Personas
                    .FirstOrDefaultAsync(m => m.CedulaIdentidad == clienteDto.CedulaIdentidad) == null ? false : true;
                if (existePersonaCliente)
                {
                    ModelState.AddModelError(string.Empty,
                    "Ya existe un Cliente con misma Cedula Identidad. " + clienteDto.CedulaIdentidad);
                }
                else
                {
                    //evaluar Estado Civil
                    var estadoCivil = await _context.EstadosCivil
                    .FirstOrDefaultAsync(m => m.Id == clienteDto.EstadoCivilId);
                    requiereDatosConyuge = estadoCivil.RequiereDatosConyuge;
                    if (requiereDatosConyuge)
                    {
                        //evaluar si existe Persona Conyuge
                        existePersonaConyuge = await _context.Personas
                        .FirstOrDefaultAsync(m => m.CedulaIdentidad == clienteDto.ConyugeCedulaIdentidad) == null ? false : true;

                        ReglasNegocioConyuge(clienteDto, existePersonaConyuge);
                    }
                }

                if (ModelState.IsValid)
                {
                    //Crear Persona Cliente
                    var personaCliente = new Persona();
                    PersonaClienteDtoToModel(clienteDto, personaCliente);
                    _context.Add(personaCliente);
                    await _context.SaveChangesAsync();

                    //Crear Cliente
                    var cliente = new Cliente();
                    ClienteDtoToModel(clienteDto, personaCliente, cliente);
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    //Ingresos Origen Ingreso Cliente
                    var origenIngresoCliente = new OrigenIngresoCliente();
                    OrigenIngresoClienteDtoToModel(clienteDto, cliente, origenIngresoCliente);
                    _context.Add(origenIngresoCliente);
                    await _context.SaveChangesAsync();

                    if (requiereDatosConyuge)
                    {
                        //Crear Persona Conyuge
                        var personaConyuge = new Persona();
                        PersonaConyugeDtoToModel(clienteDto, personaConyuge);
                        _context.Add(personaConyuge);
                        await _context.SaveChangesAsync();

                        //Crear Conyuge
                        var conyuge = new Conyuge();
                        ConguyeDtoToModel(cliente, personaConyuge, conyuge);
                        _context.Add(conyuge);
                        await _context.SaveChangesAsync();
                    }
                    //Termino OK
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion");
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Descripcion");
            ViewData["ConyugeGeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewData["ConyugeNacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion");

            return View(clienteDto);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.
                Include(c => c.Persona).
                Include(c => c.OrigenesIngresoClientes).
                Include(c => c.EstadoCivil).
                Include(c => c.Conyuges).
                FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var personaConyuge = new Persona();
            
            if (cliente.EstadoCivil.RequiereDatosConyuge)
            {
                var conyuge = cliente.Conyuges.FirstOrDefault();
                personaConyuge = await _context.Personas.FirstOrDefaultAsync(c => c.Id == conyuge.PersonaId);
            }

            var clienteDto = new ClienteDto();
            ClienteModelToDto(cliente, clienteDto, personaConyuge);

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion", clienteDto.GeneroId);
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion", clienteDto.NacionalidadId);
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion", clienteDto.EstadoCivilId);
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Descripcion", clienteDto.TipoActividadId);
            ViewData["ConyugeGeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion", clienteDto.ConyugeGeneroId);
            ViewData["ConyugeNacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion", clienteDto.ConyugeNacionalidadId);

            return View(clienteDto);
        }


        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, PersonaId, CedulaIdentidad, Nombre, Apellido, FechaNacimiento, GeneroId, Domicilio, CorreoElectronico, Telefono, NacionalidadId, DomicilioAlternativo, TelefonoLaboral, PersonaPoliticamenteExpuesta, OrigenIngresoId, TipoActividadId, FechaInicioActividad, FechaFinActividad, MontoLiquidoPercibido, EstadoCivilId, RequiereDatosConyuge, ConyugeId, ConyugePersonaId, ConyugeCedulaIdentidad, ConyugeNombre, ConyugeApellido, ConyugeFechaNacimiento, ConyugeGeneroId, ConyugeDomicilio, ConyugeCorreoElectronico, ConyugeTelefono, ConyugeNacionalidadId, Scoring, DatosVerificados")] ClienteDto clienteDto)
        {
            if (id != clienteDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ReglasNegocioCliente(clienteDto);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool existePersonaCliente = false;
                    bool requiereDatosConyuge = false;
                    bool existePersonaConyuge = false;

                    //evaluar si existe Persona Cliente
                    existePersonaCliente = await _context.Personas
                        .FirstOrDefaultAsync(m => m.Id != clienteDto.PersonaId
                                               && m.CedulaIdentidad == clienteDto.CedulaIdentidad) == null ? false : true;

                    if (existePersonaCliente)
                    {
                        ModelState.AddModelError(string.Empty,
                        "Ya existe otro Cliente con misma Cedula Identidad. " + clienteDto.CedulaIdentidad);
                    }
                    else
                    {
                        //evaluar Estado Civil
                        var estadoCivil = await _context.EstadosCivil
                        .FirstOrDefaultAsync(m => m.Id == clienteDto.EstadoCivilId);
                        requiereDatosConyuge = estadoCivil.RequiereDatosConyuge;
                        if (requiereDatosConyuge)
                        {
                            //evaluar si existe Persona Conyuge
                            existePersonaConyuge = await _context.Personas
                            .FirstOrDefaultAsync(m => m.Id != clienteDto.ConyugePersonaId
                                                   && m.CedulaIdentidad == clienteDto.ConyugeCedulaIdentidad) == null ? false : true;

                            ReglasNegocioConyuge(clienteDto, existePersonaConyuge);

                        }
                    }

                    if (ModelState.IsValid)
                    {
                        //Persona Cliente
                        var personaCliente = await _context.Personas
                        .FirstOrDefaultAsync(m => m.Id == clienteDto.PersonaId);

                        //Evaluar si trajo datos la consulta de la BD
                        if (personaCliente != null)
                        {
                            //Actualizar Persona Cliente
                            PersonaClienteDtoToModel(clienteDto, personaCliente);
                            _context.Update(personaCliente);
                        }
                        else
                        {
                            //Crear Persona Conyuge
                            personaCliente = new Persona();
                            PersonaClienteDtoToModel(clienteDto, personaCliente);
                            _context.Add(personaCliente);
                        }
                        await _context.SaveChangesAsync();

                        //Actualizar Cliente
                        var cliente = await _context.Clientes
                        .FirstOrDefaultAsync(m => m.Id == clienteDto.Id);
                        ClienteDtoToModel(clienteDto, personaCliente, cliente);
                        _context.Update(cliente);
                        await _context.SaveChangesAsync();

                        //Origen Ingreso Cliente
                        var origenIngresoCliente = await _context.OrigenesIngresoCliente
                        .FirstOrDefaultAsync(m => m.Id == clienteDto.OrigenIngresoId);
                        //Evaluar si trajo datos la consulta de la BD
                        if (origenIngresoCliente != null)
                        {
                            //Actualizar Origen Ingreso Cliente
                            OrigenIngresoClienteDtoToModel(clienteDto, cliente, origenIngresoCliente);
                            _context.Update(origenIngresoCliente);
                        }
                        else
                        {
                            //Crear Origen Ingreso Cliente
                            origenIngresoCliente = new OrigenIngresoCliente();
                            OrigenIngresoClienteDtoToModel(clienteDto, cliente, origenIngresoCliente);
                            _context.Add(origenIngresoCliente);
                        }
                        await _context.SaveChangesAsync();

                        if (requiereDatosConyuge)
                        {
                            //Persona Conyuge
                            var personaConyuge = await _context.Personas
                            .FirstOrDefaultAsync(m => m.Id == clienteDto.ConyugePersonaId);
                            //Evaluar si trajo datos la consulta de la BD
                            if(personaConyuge != null)
                            {
                                //Actualizar Persona Conyuge
                                PersonaConyugeDtoToModel(clienteDto, personaConyuge);
                                _context.Update(personaConyuge);
                            }
                            else
                            {
                                //Crear Persona Conyuge
                                personaConyuge = new Persona();
                                PersonaConyugeDtoToModel(clienteDto, personaConyuge);
                                _context.Add(personaConyuge);
                            }
                            await _context.SaveChangesAsync();

                            //Conyuge
                            var conyuge = await _context.Conyuges
                            .FirstOrDefaultAsync(m => m.Id == clienteDto.ConyugeId);
                            //Evaluar si trajo datos la consulta de la BD
                            if (conyuge != null)
                            {
                                //Actualizar Conyuge
                                ConguyeDtoToModel(cliente, personaConyuge, conyuge);
                                _context.Update(conyuge);
                            }
                            else
                            {
                                //Crear Conyuge
                                conyuge = new Conyuge();
                                ConguyeDtoToModel(cliente, personaConyuge, conyuge);
                                _context.Add(conyuge);
                            }
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            //Conyuge
                            var conyuge = await _context.Conyuges
                            .FirstOrDefaultAsync(m => m.Id == clienteDto.ConyugeId);
                            
                            //Evaluar si trajo datos la consulta de la BD
                            if (conyuge != null)
                            {
                                //Eliminar Conyuge
                                _context.Conyuges.Remove(conyuge);
                                await _context.SaveChangesAsync();
                            }

                            //Persona Conyuge
                            var personaConyuge = await _context.Personas
                            .FirstOrDefaultAsync(m => m.Id == clienteDto.ConyugePersonaId);
                            //Evaluar si trajo datos la consulta de la BD
                            if (personaConyuge != null)
                            {
                                PersonaConyugeDtoToModel(clienteDto, personaConyuge);
                                //Eliminar Conyuge
                                _context.Personas.Remove(personaConyuge);
                                await _context.SaveChangesAsync();
                            }

                        }
                        //Termino OK
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(clienteDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["GeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion", clienteDto.GeneroId);
            ViewData["NacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion", clienteDto.NacionalidadId);
            ViewData["EstadoCivilId"] = new SelectList(_context.EstadosCivil, "Id", "Descripcion", clienteDto.EstadoCivilId);
            ViewData["TipoActividadId"] = new SelectList(_context.TiposActividad, "Id", "Descripcion", clienteDto.TipoActividadId);
            ViewData["ConyugeGeneroId"] = new SelectList(_context.Generos, "Id", "Descripcion", clienteDto.ConyugeGeneroId);
            ViewData["ConyugeNacionalidadId"] = new SelectList(_context.Nacionalidades, "Id", "Descripcion", clienteDto.ConyugeNacionalidadId);

            return View(clienteDto);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clientes == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.
                Include(c => c.Persona).
                Include(c => c.OrigenesIngresoClientes).
                Include(c => c.EstadoCivil).
                Include(c => c.Conyuges).
                FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var personaConyuge = new Persona();

            //Recuperar datos de las descripciones de campos foraneos
            var genero = await _context.Generos.ToListAsync();
            var nacionalidad = await _context.Nacionalidades.ToListAsync();
            var estadoCivil = await _context.EstadosCivil.ToListAsync();
            var tipoactividad = await _context.TiposActividad.ToListAsync();

            ViewData["Genero"] = genero.FirstOrDefault(x => x.Id == cliente.Persona.GeneroId).Descripcion;
            ViewData["Nacionalidad"] = nacionalidad.FirstOrDefault(x => x.Id == cliente.Persona.NacionalidadId).Descripcion;
            ViewData["EstadoCivil"] = estadoCivil.FirstOrDefault(x => x.Id == cliente.EstadoCivilId).Descripcion;
            ViewData["TipoActividad"] = tipoactividad.FirstOrDefault(x => x.Id == cliente.OrigenesIngresoClientes.FirstOrDefault().TipoActividadId).Descripcion;

            if (cliente.EstadoCivil.RequiereDatosConyuge)
            {
                var conyuge = cliente.Conyuges.FirstOrDefault();
                personaConyuge = await _context.Personas.FirstOrDefaultAsync(c => c.Id == conyuge.PersonaId);

                ViewData["ConyugeGenero"] = genero.FirstOrDefault(x => x.Id == personaConyuge.GeneroId).Descripcion;
                ViewData["ConyugeNacionalidad"] = nacionalidad.FirstOrDefault(x => x.Id == personaConyuge.NacionalidadId).Descripcion;
            }

            var clienteDto = new ClienteDto();
            ClienteModelToDto(cliente, clienteDto, personaConyuge);

            return View(clienteDto);
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
            var cliente = await _context.Clientes
                .Include(c => c.Persona)
                .Include(c => c.OrigenesIngresoClientes)
                .Include(c => c.EstadoCivil)
                .Include(c => c.Conyuges)
                .FirstOrDefaultAsync(x => x.Id == id);

            //Eliminar Cliente, OrigenesIngresoCliente, Conyuges
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            if (cliente != null)
            {
                //evaluar Estado Civil
                if (cliente.EstadoCivil.RequiereDatosConyuge)
                {
                    //Eliminar Personas Conyuge
                    foreach (var conyuge in cliente.Conyuges)
                    {
                        var personaConyuge = await _context.Personas.FindAsync(conyuge.PersonaId);

                        if (personaConyuge != null)
                        {
                            _context.Personas.Remove(personaConyuge);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                //Eliminar Persona Cliente
                var personaCliente = await _context.Personas.FindAsync(cliente.PersonaId);
                if (personaCliente != null)
                {
                    _context.Personas.Remove(personaCliente);
                    await _context.SaveChangesAsync();
                }

            }

            //var personaCliente = await _context.Personas.FindAsync(cliente.PersonaId);
            //if (personaCliente != null)
            //{
            //    _context.Personas.Remove(personaCliente);
            //    await _context.SaveChangesAsync();
            //}

            //var origenIngresoCliente = await _context.OrigenesIngresoCliente.FirstOrDefaultAsync(x => x.ClienteId == id);
            //if (origenIngresoCliente != null)
            //{
            //    _context.OrigenesIngresoCliente.Remove(origenIngresoCliente);
            //    await _context.SaveChangesAsync();
            //}

            ////evaluar Estado Civil
            //var estadoCivil = await _context.EstadosCivil
            //.FirstOrDefaultAsync(m => m.Id == cliente.EstadoCivilId);
            //bool requiereDatosConyuge = estadoCivil.RequiereDatosConyuge;
            //if (requiereDatosConyuge)
            //{
            //    var conyuge = await _context.Conyuges.FindAsync(cliente.Conyuges.FirstOrDefault().Id);
            //    if (cliente != null)
            //    {
            //        _context.Conyuges.Remove(conyuge);
            //        await _context.SaveChangesAsync();
            //    }

            //    var personaConyuge = await _context.Personas.FindAsync(conyuge.PersonaId);
            //    if (personaConyuge != null)
            //    {
            //        _context.Personas.Remove(personaConyuge);
            //        await _context.SaveChangesAsync();
            //    }
            //}

            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static void PersonaClienteDtoToModel(ClienteDto clienteDto, Persona persona)
        {
            persona.CedulaIdentidad = clienteDto.CedulaIdentidad;
            persona.Nombre = clienteDto.Nombre;
            persona.Apellido = clienteDto.Apellido;
            persona.FechaNacimiento = clienteDto.FechaNacimiento;
            persona.GeneroId = clienteDto.GeneroId;
            persona.Domicilio = clienteDto.Domicilio;
            persona.CorreoElectronico = clienteDto.CorreoElectronico;
            persona.Telefono = clienteDto.Telefono;
            persona.NacionalidadId = clienteDto.NacionalidadId;
            persona.UrlImagen = "-";
            persona.UrlDocumento = "-";
            persona.DatosVerificados = clienteDto.DatosVerificados;
        }

        private static void PersonaConyugeDtoToModel(ClienteDto clienteDto, Persona persona)
        {
            persona.CedulaIdentidad = clienteDto.ConyugeCedulaIdentidad;
            persona.Nombre = clienteDto.ConyugeNombre;
            persona.Apellido = clienteDto.ConyugeApellido;
            persona.FechaNacimiento = clienteDto.ConyugeFechaNacimiento;
            persona.GeneroId = clienteDto.ConyugeGeneroId;
            persona.Domicilio = clienteDto.ConyugeDomicilio;
            persona.CorreoElectronico = clienteDto.ConyugeCorreoElectronico;
            persona.Telefono = clienteDto.ConyugeTelefono;
            persona.NacionalidadId = clienteDto.ConyugeNacionalidadId;
            persona.UrlImagen = "-";
            persona.UrlDocumento = "-";
            persona.DatosVerificados = true;
        }

        private static void ClienteDtoToModel(ClienteDto clienteDto, Persona persona, Cliente cliente)
        {
            cliente.PersonaId = persona.Id;
            cliente.DomicilioAlternativo = clienteDto.DomicilioAlternativo;
            cliente.TelefonoLaboral = clienteDto.TelefonoLaboral;
            cliente.PersonaPoliticamenteExpuesta = clienteDto.PersonaPoliticamenteExpuesta;
            cliente.EstadoCivilId = clienteDto.EstadoCivilId;
            cliente.Scoring = clienteDto.Scoring;
            cliente.UrlDocumento = "-";
        }

        private static void OrigenIngresoClienteDtoToModel(ClienteDto clienteDto, Cliente cliente, OrigenIngresoCliente origenIngresoCliente)
        {
            origenIngresoCliente.ClienteId = cliente.Id;
            origenIngresoCliente.TipoActividadId = clienteDto.TipoActividadId;
            origenIngresoCliente.FechaInicioActividad = clienteDto.FechaInicioActividad;
            origenIngresoCliente.FechaFinActividad = clienteDto.FechaFinActividad;
            origenIngresoCliente.MontoLiquidoPercibido = clienteDto.MontoLiquidoPercibido;
            origenIngresoCliente.UrlDocumento = "-";
        }

        private static void ConguyeDtoToModel(Cliente cliente, Persona persona, Conyuge conyuge)
        {
            conyuge.PersonaId = persona.Id;
            conyuge.ClienteId = cliente.Id;
            conyuge.UrlDocumento = "-";
        }

        private static void ClientesModelToDto(List<Cliente>? clientes, List<ClienteDto> clientesDto)
        {
            foreach (var cliente in clientes)
            {
                var clienteDto = new ClienteDto();

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
                clienteDto.DatosVerificados = cliente.Persona.DatosVerificados;
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
                clientesDto.Add(clienteDto);
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
            clienteDto.DatosVerificados = cliente.Persona.DatosVerificados;
            //PersonaConyugeConyugeId
            if(cliente.EstadoCivil.RequiereDatosConyuge)
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

        private void ReglasNegocioCliente(ClienteDto clienteDto)
        {
            if (clienteDto.FechaNacimiento >= DateTime.Today)
            {
                ModelState.AddModelError(string.Empty,
                "Fecha Nacimiento del Cliente no puede ser mayor o igual a la fecha de día. " + clienteDto.FechaNacimiento.Date);
            }
            DateTime fechaMinima = new DateTime();
            DateTime.TryParse("1900-01-01", out fechaMinima);
            if (clienteDto.FechaNacimiento < fechaMinima)
            {
                ModelState.AddModelError(string.Empty,
                "Fecha Nacimiento del Cliente no puede ser menor 1900-01-01");
            }

            if (clienteDto.FechaInicioActividad >= DateTime.Today)
            {
                ModelState.AddModelError(string.Empty,
                "Fecha Inicio Actividad del Cliente no puede ser mayor o igual a la fecha de día. " + clienteDto.FechaInicioActividad.Date);
            }

            if (clienteDto.FechaInicioActividad < clienteDto.FechaNacimiento)
            {
                ModelState.AddModelError(string.Empty,
                "Fecha Inicio Actividad del Cliente no puede ser mayor o igual a la fecha de nacimiento del cliente. " + clienteDto.FechaInicioActividad.Date);
            }

            if (clienteDto.FechaInicioActividad < fechaMinima)
            {
                ModelState.AddModelError(string.Empty,
                "Fecha Inicio Actividad del Cliente no puede ser menor 1900-01-01");
            }

            if (clienteDto.FechaInicioActividad > clienteDto.FechaFinActividad)
            {
                ModelState.AddModelError(string.Empty,
                "Fecha Fin Actividad del Cliente no puede ser mayor a la Fecha Inicio Actividad. " + clienteDto.FechaFinActividad.Date);
            }
        }

        private void ReglasNegocioConyuge(ClienteDto clienteDto, bool existePersonaConyuge)
        {
            if (existePersonaConyuge)
            {
                ModelState.AddModelError(string.Empty,
                "Ya existe un Conyuge con misma Cedula Identidad. " + clienteDto.ConyugeCedulaIdentidad);
            }
            if (clienteDto.CedulaIdentidad == clienteDto.ConyugeCedulaIdentidad)
            {
                ModelState.AddModelError(string.Empty,
                "Cliente y Conyuge no pueden tener la misma Cedula Identidad. " + clienteDto.ConyugeCedulaIdentidad);
            }

            if (clienteDto.ConyugeFechaNacimiento >= DateTime.Today)
            {
                ModelState.AddModelError(string.Empty,
                "Fecha Nacimiento del Conyuge no puede ser mayor o igual a la fecha de día. " + clienteDto.ConyugeFechaNacimiento);
            }
        }
    }
}
