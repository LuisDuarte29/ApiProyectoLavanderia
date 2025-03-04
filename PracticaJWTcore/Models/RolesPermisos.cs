namespace PracticaJWTcore.Models
{
    public class RolesPermisos
    {
        public int RolePermisoId { get; set; }
        public int RoleId { get; set; }
        public int PermisoId { get; set; }
        public Permisos Permisos { get; set; }
        public Roles Roles { get; set; }
    }
}
