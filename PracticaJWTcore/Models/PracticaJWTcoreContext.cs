using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PracticaJWTcore.Models;

public partial class PracticaJWTcoreContext : DbContext
{
    public PracticaJWTcoreContext()
    {
    }

    public PracticaJWTcoreContext(DbContextOptions<PracticaJWTcoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<Customer> Customer { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }
    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<UsuariosRoles> UsuariosRoles { get; set; }

    public virtual DbSet<Permisos> Permisos { get; set; }
    public virtual DbSet<RolesPermisos> RolesPermisos { get; set; }
    public virtual DbSet<AppointmentService> AppointmentServices { get; set; }

    public virtual DbSet<Articulos> Articulos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentService>(entity =>
        {
            entity.HasKey(e => e.IdAppointmentServices);

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Estado).HasColumnName("Estado");
        });

        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<UsuariosRoles>()
      .HasKey(ur => ur.UsuariosRolesId);

        modelBuilder.Entity<UsuariosRoles>()
            .HasOne(ur => ur.Usuario)
            .WithMany(u => u.UsuariosRoles)  // 👈 Esto permite hacer Include()
            .HasForeignKey(ur => ur.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UsuariosRoles>()
            .HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuariosRoles)
            .HasForeignKey(ur => ur.RolId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.AppointmentDate).HasColumnType("datetime");
            entity.Property(e => e.Comments)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Appointments_Customers");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK_Appointments_Vehicles");

         
        });

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

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(u => u.IdUsuario);  // Define the primary key

            entity.ToTable("Usuarios");

            entity.Property(e => e.clave).HasColumnName("clave");
            entity.Property(e => e.correo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdUsuario).ValueGeneratedOnAdd(); // Auto-generate on add
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


        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(r => r.RoleId);
            entity.ToTable("Roles");

            entity.Property(r => r.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RoleName");
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
        modelBuilder.Entity<Articulos>(entity =>
        {
            entity.HasKey(p => p.IdArticulo);
            entity.ToTable("Articulos");

            entity.Property(p => p.NombreArticulo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NombreArticulo");
            entity.Property(p=>p.Precio)
            .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Make)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OwnerName)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
