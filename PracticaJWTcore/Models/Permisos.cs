namespace PracticaJWTcore.Models
{
    public class Permisos
    {
        public long PermisoId { get; set; }
        public string PermisoNombre { get; set; }

        public List<RolesPermisos> RolesPermisos { get; set; }
    }
}
