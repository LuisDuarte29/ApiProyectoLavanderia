using ApiSwagger.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Entities;
using PracticaJWTcore.Services;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : Controller
    {
      
        private readonly CustomerServices _customerServices;
        public CustomerController(CustomerServices customerServices)
        {
            _customerServices = customerServices;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(long id) 
        {
            CustomerEntity customerEntity = await _customerServices.GetCustomer(id);
           return Ok(customerEntity.ToDto());
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerAll()
        {
            var response = await _customerServices.GetCustomerAll();
            return new OkObjectResult(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            await _customerServices.DeleteCustomers(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customer)
        {
            CustomerEntity customerEntity = await _customerServices.CreateCustomer(customer);
            return new CreatedResult($"http://localhost:7184/api/customer/{customerEntity.Id}", null);
        }
    }
}
