
namespace PracticaJWTcore.Dtos

{
    // Response de usuarios con datos ya proyectados de rol y cliente.
    public class UsuarioDTO
    {
        public int? IdUsuario { get; set; }
        public string? Correo { get; set; }
        public int? RoleId { get; set; }
        public string Roles { get; set; }
        public long? CustomerID { get; set; }
        public string Customer { get; set; }
    }
}
