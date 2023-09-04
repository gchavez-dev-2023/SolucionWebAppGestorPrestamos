using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Dtos;

namespace WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Aval> Avales { get; set; } = null!;
        public virtual DbSet<Beneficio> Beneficios { get; set; } = null!;
        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<Conyuge> Conyuges { get; set; } = null!;
        public virtual DbSet<Cuota> Cuotas { get; set; } = null!;
        public virtual DbSet<Documento> Documentos { get; set; } = null!;
        public virtual DbSet<EstadoCivil> EstadosCivil { get; set; } = null!;
        public virtual DbSet<FormaPago> FormasPago { get; set; } = null!;
        public virtual DbSet<Genero> Generos { get; set; } = null!;
        public virtual DbSet<OrigenIngresoCliente> OrigenesIngresoCliente { get; set; } = null!;
        public virtual DbSet<Nacionalidad> Nacionalidades { get; set; } = null!;
        public virtual DbSet<Pago> Pagos { get; set; } = null!;
        public virtual DbSet<Persona> Personas { get; set; } = null!;
        public virtual DbSet<PrestamoAprobado> PrestamosAprobado { get; set; } = null!;
        public virtual DbSet<Producto> Productos { get; set; } = null!;
        public virtual DbSet<Requisito> Requisitos { get; set; } = null!;
        public virtual DbSet<SolicitudPrestamo> SolicitudesPrestamo { get; set; } = null!;
        public virtual DbSet<Termino> Terminos { get; set; } = null!;
        public virtual DbSet<TipoActividad> TiposActividad { get; set; } = null!;
        public virtual DbSet<TipoDocumento> TiposDocumento { get; set; } = null!;
        public DbSet<WebApp.Dtos.SolicitudPrestamoDto>? SolicitudPrestamoDto { get; set; }
    }
}