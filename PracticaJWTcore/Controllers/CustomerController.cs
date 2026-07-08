using ApiSwagger.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Authorization;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    // Controller de clientes: delega el CRUD al service y mantiene los contratos usados por el frontend.
    public class CustomerController : Controller
    {

        private readonly ICustomerServices _customerServices;
        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }
        [Authorize]
        [Permiso("ListaCustomer", "Leer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(long id)
        {
            Customer? customerEntity = await _customerServices.GetCustomer(id);
            if (customerEntity == null)
                return NotFound();

            return Ok(customerEntity.ToDto());
        }
        [Authorize]
        [Permiso("ListaCustomer", "Leer")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerAll()
        {
            var response = await _customerServices.GetCustomerAll();
            return new OkObjectResult(response);
        }
        [Authorize]
        [Permiso("ListaCustomer", "Eliminar")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            var result = await _customerServices.DeleteCustomers(id);
            if (!result.Success)
                return result.Code == "CUSTOMER_NOT_FOUND" ? NotFound() : BadRequest(result.Message);

            return NoContent();
        }
        [Authorize]
        [Permiso("ListaCustomer", "Crear")]
        [HttpPost]
        // Crea clientes con DTO de entrada para evitar depender de la entidad completa en el alta.
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customer)
        {
            if (!ClienteValido(customer.FirstName, customer.Email, out var error))
                return BadRequest(error);

            Customer customerEntity = await _customerServices.CreateCustomer(customer); 
            return new CreatedResult($"api/customer/{customerEntity.Id}", customerEntity.Id);
        }
        [Authorize]
        [Permiso("ListaCustomer", "Actualizar")]
        [HttpPut]
        // Update mantiene el contrato historico que recibe Customer como payload.
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer customer)
        {
            if (!ClienteValido(customer.FirstName, customer.Email, out var error))
                return BadRequest(error);

            if (await _customerServices.GetCustomer(customer.Id) == null)
                return NotFound();

            await _customerServices.UpdateCustomer(customer);
            return new OkObjectResult(customer);
        }

        private static bool ClienteValido(string? nombre, string? email, out string error)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                error = "FirstName es requerido";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
            {
                error = "Email invalido";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
