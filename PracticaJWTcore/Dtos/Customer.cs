using ApiSwagger.Dtos;
using PracticaJWTcore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticaJWTcore.Entities
{
  

    public class Customer
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public Usuarios Usuarios { get; set; }

        public static Customer FromDto(CustomerDto dto)
        {
            return new Customer
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
