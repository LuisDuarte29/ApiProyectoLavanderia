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

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

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

            entity.HasMany(d => d.Services).WithMany(p => p.Appointments)
                .UsingEntity<Dictionary<string, object>>(
                    "AppointmentService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AppointmentServices_Services"),
                    l => l.HasOne<Appointment>().WithMany()
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AppointmentServices_Appointments"),
                    j =>
                    {
                        j.HasKey("AppointmentId", "ServiceId");
                        j.ToTable("AppointmentServices");
                        j.IndexerProperty<long>("AppointmentId").HasColumnName("AppointmentID");
                        j.IndexerProperty<long>("ServiceId").HasColumnName("ServiceID");
                    });
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

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.IdUsuario);  // Define the primary key

            entity.ToTable("Usuario");

            entity.Property(e => e.Clave).HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.IdUsuario).ValueGeneratedOnAdd(); // Auto-generate on add
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
