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
    [Authorize]

    public class CustomerController : Controller
    {

        private readonly ICustomerServices _customerServices;
        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(long id)
        {
            Customer customerEntity = await _customerServices.GetCustomer(id);
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
            Customer customerEntity = await _customerServices.CreateCustomer(customer);
            return new CreatedResult($"http://localhost:7184/api/customer/{customerEntity.Id}", null);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer customer)
        {
            await _customerServices.UpdateCustomer(customer);
            return new OkObjectResult(customer);
        }
    }
}
