namespace PracticaJWTcore.Models
{
 
        public class UsuariosRoles
        {
            public int UsuariosRolesId { get; set; }

            public int UsuarioId { get; set; }
            public Usuario Usuario { get; set; }

            public int RolId { get; set; }
            public Roles Rol { get; set; }
        }

    
}
