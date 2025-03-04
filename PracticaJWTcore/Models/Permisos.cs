using System.ComponentModel.DataAnnotations;

namespace PracticaJWTcore.Models
{
    public class Permisos
    {
        [Key]
        public int PermisoId { get; set; }
        public string PermisoNombre { get; set; }

        public List<RolesPermisos> RolesPermisos { get; set; }
    }
}
