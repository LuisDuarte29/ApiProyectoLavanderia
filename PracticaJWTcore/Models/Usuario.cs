using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PracticaJWTcore.Models;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Entities;

namespace PracticaJWTcore.Models
{
// Entidad de Usuarios; se vincula con clientes y roles para login/permisos.
public class Usuarios
    {
    public int? IdUsuario { get; set; }
    public string? correo { get; set; }
    public string? clave { get; set; }
        public int RoleId { get; set; }
        public long CustomerID { get; set; }
        public Customer? Customer { get; set; } = new Customer();
        public Roles? Role { get; set; }

        // Relación con UsuariosRoles

    }
}
