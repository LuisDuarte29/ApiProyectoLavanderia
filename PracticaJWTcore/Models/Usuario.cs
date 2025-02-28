using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PracticaJWTcore.Models
{
public class Usuario
    {
    public int IdUsuario { get; set; }
    public string? Correo { get; set; }
    public string? Clave { get; set; }

    // Relación con UsuariosRoles
    public ICollection<UsuariosRoles> UsuariosRoles { get; set; } = new List<UsuariosRoles>();
    }
}
