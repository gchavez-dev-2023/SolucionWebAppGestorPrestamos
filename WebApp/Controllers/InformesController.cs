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
    public class InformesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InformesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrestamosAprobadoDto
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult PrestamosAprobadosXFecha()
        {
            var resultado = (from p in _context.PrestamosAprobado
                                      group p by p.FechaAprobacion.Date into g
                                      select new 
                                      {
                                          fechaAprobacion = g.Key.ToString("dd/MM/yyyy"),
                                          cantidad = g.Count(),
                                      }).ToList();

            return Json(resultado);
        }

        [HttpPost]
        public JsonResult PrestamosAprobadosXProducto()
        {
            var resultado = (from a in _context.PrestamosAprobado
                                      join s in _context.SolicitudesPrestamo on a.SolicitudPrestamoId equals s.Id
                                      join p in _context.Productos on s.ProductoId equals p.Id
                                      group p by p.Descripcion into g
                                      select new
                                      {
                                          nombre = g.Key,
                                          cantidad = g.Count(),
                                      });
            return Json(resultado);
        }

        [HttpPost]
        public JsonResult PagosXPrestamo()
        {
            var resultado = (from p in _context.Pagos
                             join a in _context.PrestamosAprobado on p.PrestamoAprobadoId equals a.Id
                             group p by p.FechaPago.Date into g
                             select new
                             {
                                 fechaPago = g.Key.ToString("dd/MM/yyyy"),
                                 cantidad = g.Count(),
                             });
            return Json(resultado);
        }
    }
}
