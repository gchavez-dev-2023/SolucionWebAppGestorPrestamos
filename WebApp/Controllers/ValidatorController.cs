using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApp.Data;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class ValidatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ValidatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE,COLABORADOR")]
        [HttpPost]
        public JsonResult RequiereDatosConyuge(int estadoCivilId)
        {
            bool requiereDatosConyuge = false;

            if (estadoCivilId > 0)
            {
                var estadosCivil = _context.EstadosCivil.Where(x => x.Id == estadoCivilId).FirstOrDefault();
                requiereDatosConyuge = estadosCivil.RequiereDatosConyuge;

            }

            EstadoCivil estadoCivil = new EstadoCivil
            {
                Id = estadoCivilId,
                RequiereDatosConyuge = requiereDatosConyuge
            };

            return Json(estadoCivil);
        }

        [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE,COLABORADOR")]
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

        [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE,COLABORADOR")]
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

        [Authorize(Roles = "SUPERUSER,ADMINISTRADOR,GERENTE,COLABORADOR")]
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
        public JsonResult EvalSimulacion(int productoId, decimal montoSolicitado, int cantidadCuotas)
        {
            decimal ctf = 0;
            decimal montoCuota = 0;

            if (montoSolicitado > 0 && cantidadCuotas > 0)
            {
                //Solo rescatar los que se pueden ver en linsea
                var producto = _context.Productos
                    .Where(x => x.Id == productoId)
                    .Include(p => p.Terminos)
                    .FirstOrDefault(c => c.Beneficios.SolicitudEnLinea);

                //Verificar que existe producto
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
    }
}