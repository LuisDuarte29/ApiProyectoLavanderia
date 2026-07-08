using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PracticaJWTcore.Authorization;
using PracticaJWTcore.Repositorios;

namespace PracticaJWTcore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaginaBaseController : Controller
    {
        public readonly ICustomerModal _customerModal;

        public PaginaBaseController(ICustomerModal customerModal)
        {
            _customerModal = customerModal;
        }

        [HttpGet("customer")]
        [Authorize]
        [Permiso("ListaCustomer", "Leer")]
        public async Task<IActionResult> CustomerModalGetAll()
        {
            var customerModalsList = await _customerModal.CustomerModalGetAll();
            return new OkObjectResult(customerModalsList);
        }
    }
}
