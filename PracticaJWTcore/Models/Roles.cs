using System.Text.Json.Serialization;

namespace PracticaJWTcore.Models
{
    // Entidad de Roles; participa en JWT y en la asignacion de permisos.
    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public Usuarios usuario { get; set; }

        // Relación con UsuariosRoles


        // Relación con RolesPermisos
        [JsonIgnore]
        public ICollection<RolesPermisos> RolesPermisos { get; set; } = new List<RolesPermisos>();
    }

}
