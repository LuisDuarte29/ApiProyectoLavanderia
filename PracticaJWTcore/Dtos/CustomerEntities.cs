using ApiSwagger.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticaJWTcore.Entities
{
  

    public class CustomerEntity
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public static CustomerEntity FromDto(CustomerDto dto)
        {
            return new CustomerEntity
            {
                Id = dto.Id,
				FirstName = dto.FirstName,
                Email = dto.Email,
				Phone = dto.Phone,
                Address = dto.Address
            };
        }

        public CustomerDto ToDto()
        {
            return new CustomerDto
            {
                Id = this.Id,
				FirstName = this.FirstName,
                Email = this.Email,
				Phone = this.Phone,
                Address = this.Address
            };
        }
    }
}
