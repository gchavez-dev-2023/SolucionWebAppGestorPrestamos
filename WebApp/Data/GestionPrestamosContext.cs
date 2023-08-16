using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebApp.Models;

namespace WebApp.Data
{
    public partial class GestionPrestamosContext : DbContext
    {
        public GestionPrestamosContext()
        {
        }

        public GestionPrestamosContext(DbContextOptions<GestionPrestamosContext> options)
            : base(options)
        {
        }
        /*
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        */
        public virtual DbSet<Aval> Avales { get; set; } = null!;
        public virtual DbSet<Beneficio> Beneficios { get; set; } = null!;
        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<Conyuge> Conyuges { get; set; } = null!;
        public virtual DbSet<Cuota> Cuotas { get; set; } = null!;
        public virtual DbSet<Documento> Documentos { get; set; } = null!;
        //public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; } = null!;
        public virtual DbSet<EstadoCivil> EstadosCivils { get; set; } = null!;
        public virtual DbSet<FormaPago> FormasPagos { get; set; } = null!;
        public virtual DbSet<Genero> Generos { get; set; } = null!;
        public virtual DbSet<OrigenIngresoCliente> OrigenesIngresoClientes { get; set; } = null!;
        public virtual DbSet<Pago> Pagos { get; set; } = null!;
        public virtual DbSet<Persona> Personas { get; set; } = null!;
        public virtual DbSet<PrestamoAprobado> PrestamosAprobados { get; set; } = null!;
        public virtual DbSet<Producto> Productos { get; set; } = null!;
        public virtual DbSet<Requisito> Requisitos { get; set; } = null!;
        public virtual DbSet<SolicitudPrestamo> SolicitudesPrestamos { get; set; } = null!;
        public virtual DbSet<Termino> Terminos { get; set; } = null!;
        public virtual DbSet<TipoActividad> TiposActividads { get; set; } = null!;
        public virtual DbSet<TipoDocumento> TiposDocumentos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //                optionsBuilder.UseMySql("server=192.168.1.21;port=3306;database=gestion_creditos;user=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.14-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {/*
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.AccessFailedCount).HasColumnType("int(11)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.LockoutEnd).HasMaxLength(6);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Aval>(entity =>
            {
                entity.HasIndex(e => e.PersonaId, "IX_Avales_PersonaId");

                entity.HasIndex(e => e.SolicitudPrestamoId, "IX_Avales_SolicitudPrestamoId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PersonaId).HasColumnType("int(11)");

                entity.Property(e => e.SolicitudPrestamoId).HasColumnType("int(11)");

                entity.HasOne(d => d.Persona)
                    .WithMany(p => p.Avales)
                    .HasForeignKey(d => d.PersonaId);

                entity.HasOne(d => d.SolicitudPrestamo)
                    .WithMany(p => p.Avales)
                    .HasForeignKey(d => d.SolicitudPrestamoId);
            });

            modelBuilder.Entity<Beneficio>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasIndex(e => e.EstadoCivilId, "IX_Clientes_EstadoCivilId");

                entity.HasIndex(e => e.PersonaId, "IX_Clientes_PersonaId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.EstadoCivilId).HasColumnType("int(11)");

                entity.Property(e => e.PersonaId).HasColumnType("int(11)");

                entity.Property(e => e.Scoring).HasColumnType("int(11)");

                entity.Property(e => e.TelefonoLaboral).HasColumnType("int(11)");

                entity.HasOne(d => d.EstadoCivil)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.EstadoCivilId);

                entity.HasOne(d => d.Persona)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.PersonaId);
            });

            modelBuilder.Entity<Conyuge>(entity =>
            {
                entity.HasIndex(e => e.ClienteId, "IX_Conyuges_ClienteId");

                entity.HasIndex(e => e.PersonaId, "IX_Conyuges_PersonaId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClienteId).HasColumnType("int(11)");

                entity.Property(e => e.PersonaId).HasColumnType("int(11)");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.Conyuges)
                    .HasForeignKey(d => d.ClienteId);

                entity.HasOne(d => d.Persona)
                    .WithMany(p => p.Conyuges)
                    .HasForeignKey(d => d.PersonaId);
            });

            modelBuilder.Entity<Cuota>(entity =>
            {
                entity.HasIndex(e => e.PrestamoAprobadoId, "IX_Cuotas_PrestamoAprobadoId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FechaVencimiento).HasMaxLength(6);

                entity.Property(e => e.NumeroCuota).HasColumnType("int(11)");

                entity.Property(e => e.PrestamoAprobadoId).HasColumnType("int(11)");

                entity.HasOne(d => d.PrestamoAprobado)
                    .WithMany(p => p.Cuota)
                    .HasForeignKey(d => d.PrestamoAprobadoId);
            });

            modelBuilder.Entity<Documento>(entity =>
            {
                entity.HasIndex(e => e.SolicitudPrestamoId, "IX_Documentos_SolicitudPrestamoId");

                entity.HasIndex(e => e.TipoDocumentoId, "IX_Documentos_TipoDocumentoId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.SolicitudPrestamoId).HasColumnType("int(11)");

                entity.Property(e => e.TipoDocumentoId).HasColumnType("int(11)");

                entity.HasOne(d => d.SolicitudPrestamo)
                    .WithMany(p => p.Documentos)
                    .HasForeignKey(d => d.SolicitudPrestamoId);

                entity.HasOne(d => d.TipoDocumento)
                    .WithMany(p => p.Documentos)
                    .HasForeignKey(d => d.TipoDocumentoId);
            });
            
            modelBuilder.Entity<EfmigrationsHistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__EFMigrationsHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });
            

            modelBuilder.Entity<EstadoCivil>(entity =>
            {
                entity.ToTable("EstadosCivil");

                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            modelBuilder.Entity<FormaPago>(entity =>
            {
                entity.ToTable("FormasPago");

                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Genero>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            modelBuilder.Entity<OrigenIngresoCliente>(entity =>
            {
                entity.ToTable("OrigenesIngresoCliente");

                entity.HasIndex(e => e.ClienteId, "IX_OrigenesIngresoCliente_ClienteId");

                entity.HasIndex(e => e.TipoActividadId, "IX_OrigenesIngresoCliente_TipoActividadId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClienteId).HasColumnType("int(11)");

                entity.Property(e => e.FechaFinActividad).HasMaxLength(6);

                entity.Property(e => e.FechaInicioActividad).HasMaxLength(6);

                entity.Property(e => e.TipoActividadId).HasColumnType("int(11)");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.OrigenesIngresoClientes)
                    .HasForeignKey(d => d.ClienteId);

                entity.HasOne(d => d.TipoActividad)
                    .WithMany(p => p.OrigenesIngresoClientes)
                    .HasForeignKey(d => d.TipoActividadId);
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasIndex(e => e.CuotaId, "IX_Pagos_CuotaId");

                entity.HasIndex(e => e.FormaPagoId, "IX_Pagos_FormaPagoId");

                entity.HasIndex(e => e.PersonaId, "IX_Pagos_PersonaId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CuotaId).HasColumnType("int(11)");

                entity.Property(e => e.FechaPago).HasMaxLength(6);

                entity.Property(e => e.FormaPagoId).HasColumnType("int(11)");

                entity.Property(e => e.PersonaId).HasColumnType("int(11)");

                entity.HasOne(d => d.Cuota)
                    .WithMany(p => p.Pagos)
                    .HasForeignKey(d => d.CuotaId);

                entity.HasOne(d => d.FormaPago)
                    .WithMany(p => p.Pagos)
                    .HasForeignKey(d => d.FormaPagoId);

                entity.HasOne(d => d.Persona)
                    .WithMany(p => p.Pagos)
                    .HasForeignKey(d => d.PersonaId);
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FechaNacimiento).HasMaxLength(6);

                entity.Property(e => e.GeneroId).HasColumnType("int(11)");

                entity.Property(e => e.Telefono).HasColumnType("int(11)");
            });

            modelBuilder.Entity<PrestamoAprobado>(entity =>
            {
                entity.ToTable("PrestamosAprobado");

                entity.HasIndex(e => e.SolicitudPrestamoId, "IX_PrestamosAprobado_SolicitudPrestamoId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CantidadCuotas).HasColumnType("int(11)");

                entity.Property(e => e.FechaAprobacion).HasMaxLength(6);

                entity.Property(e => e.FechaDesembolso).HasMaxLength(6);

                entity.Property(e => e.FechaPrimerVencimiento).HasMaxLength(6);

                entity.Property(e => e.SolicitudPrestamoId).HasColumnType("int(11)");

                entity.HasOne(d => d.SolicitudPrestamo)
                    .WithMany(p => p.PrestamosAprobados)
                    .HasForeignKey(d => d.SolicitudPrestamoId);
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasIndex(e => e.BeneficiosId, "IX_Productos_BeneficiosId");

                entity.HasIndex(e => e.RequisitosId, "IX_Productos_RequisitosId");

                entity.HasIndex(e => e.TerminosId, "IX_Productos_TerminosId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.BeneficiosId).HasColumnType("int(11)");

                entity.Property(e => e.FechaFinVigencia).HasMaxLength(6);

                entity.Property(e => e.FechaInicioVigencia).HasMaxLength(6);

                entity.Property(e => e.RequisitosId).HasColumnType("int(11)");

                entity.Property(e => e.TerminosId).HasColumnType("int(11)");

                entity.HasOne(d => d.Beneficios)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.BeneficiosId);

                entity.HasOne(d => d.Requisitos)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.RequisitosId);

                entity.HasOne(d => d.Terminos)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.TerminosId);
            });

            modelBuilder.Entity<Requisito>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CantidadAvales).HasColumnType("int(11)");

                entity.Property(e => e.CantidadRecibosSueldo).HasColumnType("int(11)");

                entity.Property(e => e.EdadMaxima).HasColumnType("int(11)");

                entity.Property(e => e.EdadMinima).HasColumnType("int(11)");

                entity.Property(e => e.ScoringMinimo).HasColumnType("int(11)");
            });

            modelBuilder.Entity<SolicitudPrestamo>(entity =>
            {
                entity.ToTable("SolicitudesPrestamo");

                entity.HasIndex(e => e.ClienteId, "IX_SolicitudesPrestamo_ClienteId");

                entity.HasIndex(e => e.ProductoId, "IX_SolicitudesPrestamo_ProductoId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CantidadCuotas).HasColumnType("int(11)");

                entity.Property(e => e.ClienteId).HasColumnType("int(11)");

                entity.Property(e => e.FechaSolicitud).HasMaxLength(6);

                entity.Property(e => e.ProductoId).HasColumnType("int(11)");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.SolicitudesPrestamos)
                    .HasForeignKey(d => d.ClienteId);

                entity.HasOne(d => d.Producto)
                    .WithMany(p => p.SolicitudesPrestamos)
                    .HasForeignKey(d => d.ProductoId);
            });

            modelBuilder.Entity<Termino>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.PlazoMaximo).HasColumnType("int(11)");

                entity.Property(e => e.PlazoMinimo).HasColumnType("int(11)");
            });

            modelBuilder.Entity<TipoActividad>(entity =>
            {
                entity.ToTable("TiposActividad");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.VerificarUif).HasColumnName("VerificarUIF");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.ToTable("TiposDocumento");

                entity.Property(e => e.Id).HasColumnType("int(11)");
            });

            OnModelCreatingPartial(modelBuilder);
            */
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
