namespace PracticaJWTcore.Models
{
    public class Roles
    {

        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UsuariosRoles> UsuariosRoles { get; set; }
        public ICollection<RolesPermisos> RolesPermisos { get; set; }

    }
}
