using ApiSwagger.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Models;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    // Controller de clientes: delega el CRUD al service y mantiene los contratos usados por el frontend.
    public class CustomerController : Controller
    {

        private readonly ICustomerServices _customerServices;
        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(long id)
        {
            Customer customerEntity = await _customerServices.GetCustomer(id);
            return Ok(customerEntity.ToDto());
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCustomerAll()
        {
            var response = await _customerServices.GetCustomerAll();
            return new OkObjectResult(response);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            await _customerServices.DeleteCustomers(id);
            return Ok();
        }
        [Authorize]
        [HttpPost]
        // Crea clientes con DTO de entrada para evitar depender de la entidad completa en el alta.
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customer)
        {
            Customer customerEntity = await _customerServices.CreateCustomer(customer); 
            return new CreatedResult($"api/customer/{customerEntity.Id}", customerEntity.Id);
        }
        [Authorize]
        [HttpPut]
        // Update mantiene el contrato historico que recibe Customer como payload.
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer customer)
        {
            await _customerServices.UpdateCustomer(customer);
            return new OkObjectResult(customer);
        }
    }
}
