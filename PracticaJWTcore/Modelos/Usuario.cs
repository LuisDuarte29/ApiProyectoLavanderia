using System.ComponentModel.DataAnnotations.Schema;

namespace ApiSwagger.Modelos
{
 
    public class Usuario
    {
        public string correo {  get; set; }
        public string clave { get; set; }
    }
}
