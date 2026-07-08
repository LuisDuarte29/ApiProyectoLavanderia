using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Entities;

namespace PracticaJWTcore.Models;

// DbContext principal: centraliza los DbSet y mapeos EF Core contra SQL Server.
public partial class PracticaJWTcoreContext : DbContext
{
    public PracticaJWTcoreContext()
    {
    }

    public PracticaJWTcoreContext(DbContextOptions<PracticaJWTcoreContext> options)
        : base(options)
    {
    }

    // DbSets: cada propiedad representa una tabla o conjunto persistido que usa la API.
    public virtual DbSet<Customer> Customer { get; set; }

    public DbSet<Customer> CustomerEntity { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }


    public virtual DbSet<Permisos> Permisos { get; set; }
    public virtual DbSet<RolesPermisos> RolesPermisos { get; set; }
    public virtual DbSet<Articulos> Articulos { get; set; }
    public virtual DbSet<Venta> Ventas { get; set; }
    public virtual DbSet<VentaDetalle> VentaDetalles { get; set; }
    public virtual DbSet<StockMovimiento> StockMovimientos { get; set; }
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<DocumentoElectronico> DocumentoElectronicos { get; set; }

    public virtual DbSet<ComponentsForm> ComponentsForm { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapeos criticos: llaves, nombres de columnas, relaciones y tipos SQL se definen aqui.
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(u => u.IdUsuario);  // Define the primary key

            entity.ToTable("Usuarios");

            entity.Property(e => e.clave)
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)")
                .HasColumnName("clave");
            entity.Property(e => e.correo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdUsuario).ValueGeneratedOnAdd(); // Auto-generate on add

            entity.HasOne(u => u.Role)
              .WithMany()                 // o .WithMany(r => r.Usuarios) si Roles tiene colección
              .HasForeignKey(u => u.RoleId)
              .OnDelete(DeleteBehavior.Restrict);


        });
 


        // 📌 Configuración de RolesPermisos
        modelBuilder.Entity<RolesPermisos>()
            .HasKey(rp => rp.RolePermisoId);

        modelBuilder.Entity<RolesPermisos>()
            .HasOne(rp => rp.Roles)
            .WithMany(r => r.RolesPermisos)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolesPermisos>()
            .HasOne(rp => rp.Permisos)
            .WithMany(p => p.RolesPermisos)
            .HasForeignKey(rp => rp.PermisoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolesPermisos>()
     .HasOne(rp => rp.ComponentsForm)
     .WithMany(p => p.RolesPermisos)
     .HasForeignKey(rp => rp.ComponentsId)
     .OnDelete(DeleteBehavior.Cascade);



        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(r => r.RoleId);
            entity.ToTable("Roles");
            entity.Ignore(r => r.usuario);

            entity.Property(r => r.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RoleName");
        });

        modelBuilder.Entity<ComponentsForm>(entity =>
        {
            entity.HasKey(r => r.ComponentsId);
            entity.ToTable("ComponentsForm");

            entity.Property(r => r.ComponentsName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ComponentsName");
        });

        // 📌 Configuración de Permisos
        modelBuilder.Entity<Permisos>(entity =>
        {
            entity.HasKey(p => p.PermisoId);
            entity.ToTable("Permisos");

            entity.Property(p => p.PermisoNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PermisoNombre");
        });
        // Articulos mapping -> tablas existentes
        // Articulos, ventas y stock son tablas sensibles porque sostienen inventario y facturacion.
        modelBuilder.Entity<Articulos>(entity =>
        {
            entity.HasKey(p => p.IdArticulo);
            entity.ToTable("Articulos");

            entity.Property(p => p.IdArticulo).HasColumnName("IdArticulo");
            entity.Property(p => p.NombreArticulo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NombreArticulo");
            entity.Property(p => p.Precio)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("Precio");
            entity.Property(p => p.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Codigo");
            entity.Property(p => p.CodigoBarra)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CodigoBarra");
            entity.Property(p => p.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Descripcion");
            entity.Property(p => p.PrecioCosto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PrecioCosto");
            entity.Property(p => p.PrecioVenta)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PrecioVenta");
            entity.Property(p => p.StockActual)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("StockActual");
            entity.Property(p => p.StockMinimo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("StockMinimo");
            entity.Property(p => p.Activo)
                .HasColumnName("Activo");
            entity.Property(p => p.IdCategoria)
                .HasColumnName("IdCategoria");

            entity.HasOne<Categoria>()
                .WithMany()
                .HasForeignKey(p => p.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta);
            entity.ToTable("Ventas");

            entity.Property(e => e.IdVenta).HasColumnName("IdVenta");
            entity.Property(e => e.FechaVenta).HasColumnType("datetime").HasColumnName("FechaVenta");
            entity.Property(e => e.IdCliente).HasColumnName("IdCliente");
            entity.Property(e => e.IdUsuario).HasColumnName("IdUsuario");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)").HasColumnName("SubTotal");
            entity.Property(e => e.IvaTotal).HasColumnType("decimal(18,2)").HasColumnName("IvaTotal");
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)").HasColumnName("Total");
            entity.Property(e => e.MetodoPago).HasMaxLength(50).IsUnicode(false).HasColumnName("MetodoPago");
            entity.Property(e => e.Estado).HasMaxLength(30).IsUnicode(false).HasColumnName("Estado");
            entity.Property(e => e.FechaAnulacion).HasColumnType("datetime").HasColumnName("FechaAnulacion");
            entity.Property(e => e.MotivoAnulacion).HasMaxLength(255).IsUnicode(false).HasColumnName("MotivoAnulacion");
            entity.Property(e => e.IdUsuarioAnulacion).HasColumnName("IdUsuarioAnulacion");
        });

        modelBuilder.Entity<VentaDetalle>(entity =>
        {
            entity.HasKey(e => e.IdVentaDetalle);
            entity.ToTable("VentaDetalles");

            entity.Property(e => e.IdVentaDetalle).HasColumnName("IdVentaDetalle");
            entity.Property(e => e.IdVenta).HasColumnName("IdVenta");
            entity.Property(e => e.IdArticulo).HasColumnName("IdArticulo");
            entity.Property(e => e.Cantidad).HasColumnType("decimal(18,2)").HasColumnName("Cantidad");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)").HasColumnName("PrecioUnitario");
            entity.Property(e => e.PorcentajeIva).HasColumnType("decimal(5,2)").HasColumnName("PorcentajeIva");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)").HasColumnName("SubTotal");

            entity.HasOne<Articulos>()
                .WithMany()
                .HasForeignKey(d => d.IdArticulo)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Venta>()
                .WithMany(v => v.VentaDetalles)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StockMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdStockMovimiento);
            entity.ToTable("StockMovimientos");

            entity.Property(e => e.IdStockMovimiento).HasColumnName("IdStockMovimiento");
            entity.Property(e => e.FechaMovimiento).HasColumnType("datetime").HasColumnName("FechaMovimiento");
            entity.Property(e => e.IdArticulo).HasColumnName("IdArticulo");
            entity.Property(e => e.TipoMovimiento).HasMaxLength(50).IsUnicode(false).HasColumnName("TipoMovimiento");
            entity.Property(e => e.Cantidad).HasColumnType("decimal(18,2)").HasColumnName("Cantidad");
            entity.Property(e => e.StockAnterior).HasColumnType("decimal(18,2)").HasColumnName("StockAnterior");
            entity.Property(e => e.StockNuevo).HasColumnType("decimal(18,2)").HasColumnName("StockNuevo");
            entity.Property(e => e.Referencia).HasMaxLength(100).IsUnicode(false).HasColumnName("Referencia");
            entity.Property(e => e.Observacion).HasMaxLength(255).IsUnicode(false).HasColumnName("Observacion");

            entity.HasOne<Articulos>()
                .WithMany()
                .HasForeignKey(d => d.IdArticulo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria);
            entity.ToTable("Categorias");
            entity.Property(e => e.IdCategoria).HasColumnName("IdCategoria");
            entity.Property(e => e.NombreCategoria).HasMaxLength(100).IsUnicode(false).HasColumnName("NombreCategoria");
            entity.Property(e => e.Activo).HasColumnName("Activo");
        });

        modelBuilder.Entity<DocumentoElectronico>(entity =>
        {
            entity.HasKey(e => e.IdDocumentoElectronico);
            entity.ToTable("DocumentosElectronicos");

            entity.Property(e => e.IdDocumentoElectronico).HasColumnName("IdDocumentoElectronico");
            entity.Property(e => e.IdVenta).HasColumnName("IdVenta");
            entity.Property(e => e.TipoDocumento).HasMaxLength(50).IsUnicode(false).HasColumnName("TipoDocumento");
            entity.Property(e => e.NumeroDocumento).HasMaxLength(50).IsUnicode(false).HasColumnName("NumeroDocumento");
            entity.Property(e => e.EstadoFiscal).HasMaxLength(50).IsUnicode(false).HasColumnName("EstadoFiscal");
            entity.Property(e => e.XmlContenido).HasColumnName("XmlContenido");
            entity.Property(e => e.XmlFirmado).HasColumnName("XmlFirmado");
            entity.Property(e => e.CodigoRespuesta).HasMaxLength(50).IsUnicode(false).HasColumnName("CodigoRespuesta");
            entity.Property(e => e.MensajeRespuesta).HasColumnName("MensajeRespuesta");
            entity.Property(e => e.FechaAprobacion).HasColumnType("datetime").HasColumnName("FechaAprobacion");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime").HasColumnName("FechaCreacion");

            entity.HasOne<Venta>()
                .WithMany()
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
