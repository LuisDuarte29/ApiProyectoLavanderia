namespace PracticaJWTcore.Models
{
    public class RolesPermisos
    {
        public long RolesPermisosId { get; set; }
        public long RoleId { get; set; }
        public long PermisoId { get; set; }
        public Permisos Permisos { get; set; }
        public Roles Roles { get; set; }
    }
}
