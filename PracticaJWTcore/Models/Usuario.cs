using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PracticaJWTcore.Models
{
public class Usuarios
    {
    public int IdUsuario { get; set; }
    public string? correo { get; set; }
    public byte[] clave { get; set; }
        public int RoleId { get; set; }
        public long CustomerID { get; set; }
        public Customer Customer { get; set; } = new Customer();
        public Roles Roles { get; set; } = new Roles();

        // Relación con UsuariosRoles
        public ICollection<UsuariosRoles> UsuariosRoles { get; set; } = new List<UsuariosRoles>();
    }
}
