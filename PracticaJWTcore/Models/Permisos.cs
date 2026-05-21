using System.ComponentModel.DataAnnotations;

namespace PracticaJWTcore.Models
{
    // Entidad de Permisos; se relaciona con roles por componente de pantalla.
    public class Permisos
    {
        [Key]
        public int PermisoId { get; set; }
        public string PermisoNombre { get; set; }

        public List<RolesPermisos> RolesPermisos { get; set; }
    }
}
