namespace PracticaJWTcore.Dtos
{
    // DTO devuelto por login: token JWT y rol asociado para el frontend.
    public class TokenRolDTO
    {
        public string Token { get; set; }
        public int RolId { get; set; }
    }
}
