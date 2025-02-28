namespace PracticaJWTcore.Models
{
    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        // Relación con UsuariosRoles
        public ICollection<UsuariosRoles> UsuariosRoles { get; set; } = new List<UsuariosRoles>();

        // Relación con RolesPermisos
        public ICollection<RolesPermisos> RolesPermisos { get; set; } = new List<RolesPermisos>();
    }

}
