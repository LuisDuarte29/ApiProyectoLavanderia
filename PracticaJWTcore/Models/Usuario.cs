using System;
using System.Collections.Generic;

namespace PracticaJWTcore.Models;

public partial class Usuario
{
    public string? Correo { get; set; }

    public byte[]? Clave { get; set; }

    public int IdUsuario { get; set; }

    public ICollection<UsuariosRoles> UsuariosRoles { get; set; }
}
