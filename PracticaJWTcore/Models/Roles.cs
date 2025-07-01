using System.Text.Json.Serialization;

namespace PracticaJWTcore.Models
{
    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public Usuarios usuario { get; set; }

        // Relación con UsuariosRoles

        [JsonIgnore]
        public ICollection<UsuariosRoles> UsuariosRoles { get; set; } = new List<UsuariosRoles>();

        // Relación con RolesPermisos
        [JsonIgnore]
        public ICollection<RolesPermisos> RolesPermisos { get; set; } = new List<RolesPermisos>();
    }

}
