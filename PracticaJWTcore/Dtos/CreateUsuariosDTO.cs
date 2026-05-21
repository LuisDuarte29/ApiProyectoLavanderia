namespace PracticaJWTcore.Dtos
{
    // Request para crear usuarios; clave es opcional para conservar el alta historica con clave inicial.
    public class CreateUsuariosDTO
    {
        public string? correo { get; set; }
        public string? clave { get; set; }
        public int RoleId { get; set; }
        public long CustomerID { get; set; }

    }
}
