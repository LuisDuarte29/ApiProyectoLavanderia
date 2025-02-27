namespace PracticaJWTcore.Models
{
    public class UsuariosRoles
    {
        public long UsuariosRolesId { get; set; }
        public Usuario Usuario { get; set; }
        public long UsuarioId { get; set; }
        public long RolId { get; set; }
        public Roles Rol { get; set; }
    }
}
