namespace ApiSwagger.Dtos
{
    // Request de alta de cliente; separa el payload de la entidad Customer.
    public class CreateCustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
