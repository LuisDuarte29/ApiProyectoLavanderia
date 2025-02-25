using System.ComponentModel.DataAnnotations;

namespace ApiSwagger.Dtos
{

    //Se usa para poder dar como respuesta los datos ya creados desde la base de datos
    public class CustomerDto
    {
        public long Id {  get; set; }
        [Required(ErrorMessage = "Debe de tener el primer nombre")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Debe de tener el primer apellido")]
        public string LastName { get; set; }
        public string Email { get; set; }   
        public string Phone { get; set; }
        public string Address { get; set; }

    }
}
